using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.DTOs.Order;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Order;

namespace OnlineCoursePlatform.Web.Pages.Admin.Discounts
{
    [PermissionChecker(25)]
    public class IndexModel : PageModel
    {
        private readonly IOrderService _orderService;
        public IndexModel(IOrderService orderService) => _orderService = orderService;
        public AdminDiscountViewModel Discounts { get; set; }
        public void OnGet(int pageId = 1) => Discounts = _orderService.GetAllDiscounts(pageId);
    }
}
