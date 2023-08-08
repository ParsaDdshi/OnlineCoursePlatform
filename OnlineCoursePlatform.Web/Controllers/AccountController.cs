using Microsoft.AspNetCore.Mvc;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Convertors;
using OnlineCoursePlatform.Core.Generators;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.User;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using OnlineCoursePlatform.Core.Senders;
using OnlineCoursePlatform.Core.DTOs.User;

namespace OnlineCoursePlatform.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IViewRenderService _viewRenderService;

        public AccountController(IUserService userService, IViewRenderService viewRenderService)
        {
            _userService = userService;
            _viewRenderService = viewRenderService;
        }

        #region Register

        [Route("Register")]
        public IActionResult Register() => View();

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(RegisterViewModel registerViewModel)
        {
            if(!ModelState.IsValid)
                return View(registerViewModel);

            if (_userService.IsUserNameExist(registerViewModel.UserName))
            {
                ModelState.AddModelError("UserName", "نام کاریری معتبر نمی باشد");
                return View(registerViewModel);
            }

            if(_userService.IsEmailExist(FixedText.FixedEmail(registerViewModel.Email)))
            {
                ModelState.AddModelError("Email", "ایمیل معتبر نمی باشد");
                return View(registerViewModel);
            }

            User user = new User()
            {
                ActiveCode = NameGenerator.GenerateUniqCode(),
                Email = registerViewModel.Email,
                Password = PasswordHelper.EncodePasswordMd5(registerViewModel.Password),
                RegisterDate = DateTime.Now,
                UserName = registerViewModel.UserName,
                IsActive = false,
                UserAvatar = "DefaultAvatar.png"
            };

            _userService.InsertUser(user);

            #region Send Activation Email
            string emailBody = _viewRenderService.RenderToStringAsync("_ActivationEmail", user);
            SendEmail.Send(user.Email, "فعالسازی حساب کاربری", emailBody);
            #endregion

            return View("SuccessRegister", user);
        }

        #endregion

        #region Login

        [Route("Login")]
        public IActionResult Login(bool isEmailChanged = false, bool editProfile = false)
        {
            if (isEmailChanged)
                return View("EmailChanged");

            ViewBag.EditProfile = editProfile;
            return View();
        }


        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginViewModel loginViewModel, string ReturnUrl = "/")
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            User user = _userService.LoginUser(FixedText.FixedEmail(loginViewModel.Email),
                PasswordHelper.EncodePasswordMd5(loginViewModel.Password));

            if(user != null)
            {
                if(user.IsActive)
                {
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim (ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim (ClaimTypes.Name, user.UserName.ToString())
                    };

                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        IsPersistent = loginViewModel.RememberMe
                    };

                    HttpContext.SignInAsync(principal, properties);

                    if(ReturnUrl != "/")
                        return Redirect(ReturnUrl);

                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ModelState.AddModelError("Email", "حساب کاربری شما فعال نمی باشد");
                    return View(loginViewModel);
                }
            }

            ModelState.AddModelError("Email", "کاربری با مشخصات وارد شده یافت نشد");
            return View(loginViewModel);
        }

        #endregion

        #region Active Account
        public IActionResult ActiveAccount(string id) // id = active code
        {
            ViewBag.IsActive = _userService.ActiveAccount(id);
            return View();
        }
        #endregion

        #region Logout

        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        #endregion

        #region Forget Password

        [Route("ForgetPassword")]
        public IActionResult ForgetPassword() => View();

        [HttpPost]
        [Route("ForgetPassword")]
        public IActionResult ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            User user = _userService.GetUserByEmail(FixedText.FixedEmail(forgetPasswordViewModel.Email));

            if(user == null)
            {
                ModelState.AddModelError("Email", "کاربری یافت نشد");
                return View(forgetPasswordViewModel);
            }

            string emailBody = _viewRenderService.RenderToStringAsync("_ForgetPassword", user);
            SendEmail.Send(user.Email, "بازیابی کلمه عبور", emailBody);
            ViewBag.IsSuccess = true;

            return View();
        }

        #endregion

        #region Reset Password

        // id = active code
        public IActionResult ResetPassword(string id) => View(new ResetPasswordViewModel()
        {
            ActiveCode = id
        });

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordViewModel);

            User user = _userService.GetUserByActiveCode(resetPasswordViewModel.ActiveCode);

            if (user == null)
                return NotFound();

            string hashNewPassword = PasswordHelper.EncodePasswordMd5(resetPasswordViewModel.Password);
            user.Password = hashNewPassword;
            _userService.Save();

            return RedirectToAction("Login", "Account");
        }
        #endregion
    }
}
