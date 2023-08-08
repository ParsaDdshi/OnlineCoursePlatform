using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Order;
using System.Globalization;

namespace OnlineCoursePlatform.Web.Pages.Admin.Discounts
{
    [PermissionChecker(26)]
    public class CreateDiscountModel : PageModel
    {
        private readonly IOrderService _orderService;
        public CreateDiscountModel(IOrderService orderService) => _orderService = orderService;

        [BindProperty]
        public Discount Discount { get; set; }
        public void OnGet()
        {
        }

        public IActionResult OnPost(string startDate="", string endDate="")
        {
            if(!string.IsNullOrEmpty(startDate))
            {
                string[] stDate = startDate.Split('/');
                Discount.StartDate = new DateTime(int.Parse(stDate[0]),
                    int.Parse(stDate[1]),
                    int.Parse(stDate[2]),
                    new PersianCalendar());
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                string[] edDate = endDate.Split('/');
                Discount.EndDate = new DateTime(int.Parse(edDate[0]),
                    int.Parse(edDate[1]),
                    int.Parse(edDate[2]),
                    new PersianCalendar());
            }

            if(Discount.Percent > 100)
            {
                ModelState.AddModelError("Percent", "درصد تخفیف نمی تواند بیشتر از 100 باشد.");
                return Page();
            }

            if (_orderService.IsDiscountCodeExist(Discount.DiscountCode))
            {
                ModelState.AddModelError("DiscountCode", "کد تخفیف تکراری است.");
                return Page();
            }

            if (!ModelState.IsValid)
                return Page();

            _orderService.AddDiscount(Discount);

            return RedirectToPage("Index");
        }

        public IActionResult OnGetCheckCode(string code)
        {
            return Content(_orderService.IsDiscountCodeExist(code).ToString());
        }
    }
}
