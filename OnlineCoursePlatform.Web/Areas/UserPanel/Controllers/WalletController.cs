using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCoursePlatform.Core.DTOs.User;
using OnlineCoursePlatform.Core.Services.Interfaces;
using ZarinpalSandbox;

namespace OnlineCoursePlatform.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class WalletController : Controller
    {
        private readonly IUserService _userService;
        public WalletController(IUserService userService) => _userService = userService;
        
        [Route("UserPanel/Wallet")]
        public IActionResult Index()
        {
            ViewBag.WalletList = _userService.GetUserWallet(User.Identity.Name);
            return View();
        }

        [Route("UserPanel/Wallet")]
        [HttpPost]
        public IActionResult Index(ChargeWalletViewModel chargeWalletViewModel)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.WalletList = _userService.GetUserWallet(User.Identity.Name);
                return View(chargeWalletViewModel);
            }
            int walletId = _userService.ChargeWallet
                (User.Identity.Name, chargeWalletViewModel.Amount, "شارژ حساب");


            #region Online Payment

            Payment payment = new Payment(chargeWalletViewModel.Amount);
            var res = payment.PaymentRequest("شارژ حساب کاربری", "https://localhost:7008/OnlinePayment/" + walletId);
            if(res.Result.Status == 100)
            {
                return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + res.Result.Authority);
            }

            #endregion
            return null;
        }
    }
}
