using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineCoursePlatform.DataLayer.Entities.Wallet
{
    public class Wallet
    {
        public Wallet() { }

        [Key]
        public int WalletId { get; set; }

        [Display(Name = "نوع تراکنش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [ForeignKey("WalletType")]
        public int TypeId { get; set; }

        [Display(Name = "کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public int UserId { get; set; }

        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public int Amount { get; set; }

        [Display(Name = "تایید شده")]
        public bool IsPay { get; set; }

        [Display(Name = "شرح")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string Description { get; set; }

        [Display(Name = "تاریخ و ساعت")]
        public DateTime CreateDate { get; set; }


        #region Relations

        public virtual User.User User { get; set; }
        public virtual WalletType WalletType { get; set; }

        #endregion
    }
}
