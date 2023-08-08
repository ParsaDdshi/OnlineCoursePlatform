using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Convertors;
using OnlineCoursePlatform.Core.DTOs.Order;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Order;
using System.Globalization;

namespace OnlineCoursePlatform.Web.Pages.Admin.Discounts
{
    [PermissionChecker(27)]
    public class EditDiscountModel : PageModel
    {
        private readonly IOrderService _orderService;
        public EditDiscountModel(IOrderService orderService)
        {
            _orderService = orderService; 
        }

        [BindProperty]
        public Discount Discount { get; set; }
        public void OnGet(int id)
        {
            Discount = _orderService.GetDiscountById(id);
        }

        public IActionResult OnPost(int id, string startDate = "", string endDate = "")
        {
            DiscountCodeDates codedates = _orderService.GetDiscountDates(id);
            if (!string.IsNullOrEmpty(startDate) && startDate != codedates.StartDate?.ToShamsi())
            {
                string[] stDate = startDate.Split('/');
                Discount.StartDate = new DateTime(int.Parse(stDate[0]),
                    int.Parse(stDate[1]),
                    int.Parse(stDate[2]),
                    new PersianCalendar());
            }
            else Discount.StartDate = codedates.StartDate;   

            if (!string.IsNullOrEmpty(endDate) && endDate != codedates.EndDate?.ToShamsi())
            {
                string[] edDate = endDate.Split('/');
                Discount.EndDate = new DateTime(int.Parse(edDate[0]),
                    int.Parse(edDate[1]),
                    int.Parse(edDate[2]),
                    new PersianCalendar());
            }
            else Discount.EndDate = codedates.EndDate;
                

            if (Discount.Percent > 100)
            {
                ModelState.AddModelError("Percent", "درصد تخفیف نمی تواند بیشتر از 100 باشد.");
                return Page();
            }

            if (Discount.DiscountCode != codedates.DiscountCode  && _orderService.IsDiscountCodeExist(Discount.DiscountCode))
            {
                ModelState.AddModelError("DiscountCode", "کد تخفیف تکراری است.");
                return Page();
            }

            if (!ModelState.IsValid)
                return Page();

            _orderService.UpdateDiscount(Discount);

            return RedirectToPage("Index");
        }
    }
}
