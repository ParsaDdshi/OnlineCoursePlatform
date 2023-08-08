using OnlineCoursePlatform.DataLayer.Entities.User;
using System.ComponentModel.DataAnnotations;

namespace OnlineCoursePlatform.DataLayer.Entities.Order
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }

        [Display(Name = "کد تخفیف")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [MaxLength(150)]
        public string DiscountCode { get; set; }

        [Display(Name = "درصد تخفیف")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public int Percent { get; set; }

        public int? UsableCount { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsDelete { get; set; }


        #region Relations

        public List<UserDiscountCode>? UserDiscountCodes { get; set; }

        #endregion
    }
}
