using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Order;

namespace OnlineCoursePlatform.Web.Pages.Admin.Discounts
{
    [PermissionChecker(28)]
    public class DeleteDiscountModel : PageModel
    {
        private readonly IOrderService _orderService;
        public DeleteDiscountModel(IOrderService orderService)
        {
            _orderService = orderService;    
        }

        [BindProperty]
        public Discount Discount { get; set; }
        public void OnGet(int id)
        {
            Discount = _orderService.GetDiscountById(id);
        }

        public IActionResult OnPost() 
        { 
            _orderService.DeleteDiscount(Discount.DiscountId);
            return RedirectToPage("Index");
        }
    }
}
