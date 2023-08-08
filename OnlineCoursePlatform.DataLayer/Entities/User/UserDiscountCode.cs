using OnlineCoursePlatform.DataLayer.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCoursePlatform.DataLayer.Entities.User
{
    public class UserDiscountCode
    {
        public int UserDiscountCodeId { get; set; }

        public int UserId { get; set; }

        public int DiscountId { get; set; }

        #region Relations

        public User User { get; set; }
        public Discount Discount { get; set; }

        #endregion
    }
}
