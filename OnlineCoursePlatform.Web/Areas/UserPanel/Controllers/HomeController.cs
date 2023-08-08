using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.Core.Convertors;
using OnlineCoursePlatform.DataLayer.Entities.User;
using OnlineCoursePlatform.Core.Senders;
using OnlineCoursePlatform.Core.DTOs.User;

namespace OnlineCoursePlatform.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IViewRenderService _viewRenderService;
        public HomeController(IUserService userService, IViewRenderService viewRenderService)
        {
            _userService= userService;
            _viewRenderService= viewRenderService;
        }

        public IActionResult Index() => View(_userService.GetUserInformation(User.Identity.Name));

        [Route("UserPanel/EditProfile")]
        public IActionResult EditProfile() => View(_userService.GetDataForUserProfileEdit(User.Identity.Name));

        [Route("UserPanel/EditProfile")]
        [HttpPost]
        public IActionResult EditProfile(EditProfileViewModel editProfileViewModel)
        {
            if (!ModelState.IsValid)
                return View(editProfileViewModel);

            User user = _userService.GetUserByUserName(User.Identity.Name);
            bool isEmailChanged = false;

            if((user.Email == editProfileViewModel.Email && user.UserName == editProfileViewModel.UserName) && editProfileViewModel.UserAvatar == null)
            {
                ModelState.AddModelError("Email", "لطفا اطلاعات وارد شده را تغییر دهید");
                return View(editProfileViewModel);
            }

            if (user.UserName != editProfileViewModel.UserName)
            {
                if (_userService.IsUserNameExist(editProfileViewModel.UserName))
                {
                    ModelState.AddModelError("UserName", "نام کاریری تکراری است ");
                    return View(editProfileViewModel);
                }
            }

            if (user.Email != editProfileViewModel.Email)
            {
                if (_userService.IsEmailExist(FixedText.FixedEmail(editProfileViewModel.Email)))
                {
                    ModelState.AddModelError("Email", "ایمیل تکراری است");
                    return View(editProfileViewModel);
                }
                _userService.DeActiveAccount(User.Identity.Name);
                isEmailChanged = true;
                string emailBody = _viewRenderService.RenderToStringAsync("_EmailChanged", user);
                SendEmail.Send(editProfileViewModel.Email, "فعالسازی حساب کاربری", emailBody);
            }

            _userService.EditProfile(User.Identity.Name, editProfileViewModel);

            // Logout User
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login","Account", new {editProfile = true, isEmailChanged});
        }

        [Route("UserPanel/ChangePassword")]
        public IActionResult ChangePassword() => View();

        [Route("UserPanel/ChangePassword")]
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            string userName = User.Identity.Name;
            if (!ModelState.IsValid)
                return View(changePasswordViewModel);

            if(!_userService.CompareOldPassword(userName, changePasswordViewModel.OldPassword))
            {
                ModelState.AddModelError("OldPassword", "کلمه عبور فعلی صحیح نمیباشد");
                return View(changePasswordViewModel);
            }

            _userService.ChangeUserPassword(userName, changePasswordViewModel.Password);
            ViewBag.IsSuccess = true;

            return View();
        }
    }
}
