using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineCoursePlatform.Core.Generators;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Wallet;
using ZarinpalSandbox;

namespace OnlineCoursePlatform.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;
        public HomeController(IUserService userService, ICourseService courseService) 
        {
            _userService = userService;
            _courseService = courseService;
        }
        public IActionResult Index() 
        {
            ViewBag.PopularCourses = _courseService.GetPopularCourses();
            return View();
        } 

        [Route("OnlinePayment/{id}")]
        public IActionResult OnlinePayment(int id)
        {
            if (HttpContext.Request.Query["Status"] != "" &&
                HttpContext.Request.Query["Status"].ToString().ToLower() == "ok" &&
                HttpContext.Request.Query["Authority"] != "")
            {
                string authority = HttpContext.Request.Query["Authority"];
                Wallet wallet = _userService.GetWalletByWalletId(id);
                Payment payment = new Payment(wallet.Amount);
                var res = payment.Verification(authority).Result;
                if(res.Status == 100)
                {
                    ViewBag.Code = res.RefId;
                    ViewBag.IsSuccess = true;
                    wallet.IsPay = true;
                    _userService.Save();
                }
            }

            return View();
        }

        public IActionResult GetSubGroups(int id)
        {
            List<SelectListItem> subGroups = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "انتخاب کنید", Value = ""}
            };
            subGroups.AddRange(_courseService.GetSubGroupForManageCourse(id));
            return Json(new SelectList(subGroups, "Value", "Text"));
        }

        [HttpPost]
        [Route("file-upload")]
        public IActionResult UploadImage(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            if (upload.Length <= 0) return null;

            var fileName = NameGenerator.GenerateUniqCode() + Path.GetExtension(upload.FileName).ToLower();

            var path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/MyImages",
                fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                upload.CopyTo(stream);
            }

            var url = $"{"/MyImages/"}{fileName}";

            return Json(new { uploaded = true, url });
        }

        [Route("Contact")]
        public IActionResult Contact() => View();

        [Route("About")]
        public IActionResult About() => View();

        public IActionResult Error404() => View();
    }
}