using OnlineCoursePlatform.DataLayer.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCoursePlatform.Core.DTOs.Order
{
    public class AdminDiscountViewModel
    {
        public List<Discount> Discounts { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
    }

    public class DiscountCodeDates 
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public string DiscountCode { get; set; }
    }
}
