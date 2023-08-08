using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.DTOs.Order;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;

namespace OnlineCoursePlatform.Web.Pages.Admin.Discounts
{
    [PermissionChecker(29)]
    public class DeletedDiscountCodesModel : PageModel
    {
        private readonly IOrderService _orderService;
        public DeletedDiscountCodesModel(IOrderService orderService)
        {
            _orderService = orderService;    
        }

        public AdminDiscountViewModel DeletedDiscounts { get; set; }
        public void OnGet(int pageId = 1)
        {
            DeletedDiscounts = _orderService.GetDeletedDiscounts(pageId);
        }
    }
}
