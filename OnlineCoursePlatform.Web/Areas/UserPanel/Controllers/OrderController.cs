using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCoursePlatform.Core.DTOs.Order;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Order;

namespace OnlineCoursePlatform.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService) => _orderService = orderService;

        public IActionResult Index()
        {
            return View(_orderService.GetUserOrdersByUserName(User.Identity.Name));
        }

        public IActionResult ShowOrder(int orderId, bool isPay = false, string type="")
        {

            Order order = _orderService.GetOrderForUserPanel(User.Identity.Name, orderId);
            if(order == null) 
                return NotFound();

            ViewBag.DiscountType = type;

            ViewBag.IsPay = isPay;
            return View(order);
        }

        public IActionResult PayOrder(int orderId)
        {
            if(_orderService.PayOrder(orderId, User.Identity.Name))
            {
                return Redirect("/UserPanel/Order/ShowOrder?orderId=" + orderId + "&isPay=true");
            }

            return BadRequest();
        }

        public IActionResult UseDiscount(int orderId, string discountCode)
        {
            DiscountUseType type = _orderService.UseDiscount(orderId, discountCode);
            return Redirect("/UserPanel/Order/ShowOrder?orderId="
                + orderId + "&type=" + type.ToString());
        }

        public IActionResult DeleteDetail(int id, int orderId)
        {
            _orderService.DeleteCourseFromOrder(id);
            _orderService.UpdateOrderSum(orderId);
            return Redirect("/UserPanel/Order/ShowOrder?orderId=" + orderId);
        }
    }
}
