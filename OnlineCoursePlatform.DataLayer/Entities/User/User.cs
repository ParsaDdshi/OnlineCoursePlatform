using OnlineCoursePlatform.DataLayer.Entities.Course;
using OnlineCoursePlatform.DataLayer.Entities.Question;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineCoursePlatform.DataLayer.Entities.User;

public class User
{
    public User()
    {

    }

    [Key]
    public int UserId { get; set; }

    [Display(Name = "نام کاربری")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
    [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
    public string UserName { get; set; }

    [Display(Name = "ایمیل")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
    [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
    [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد.")]
    public string Email { get; set; }

    [Display(Name = "کلمه عبور")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
    [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
    public string Password { get; set; }

    [Display(Name = "کد فعالسازی")]
    [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
    public string ActiveCode { get; set; }

    [Display(Name = "وضعیت")]
    public bool IsActive { get; set; }

    [Display(Name = "تاریخ ثبت نام")]
    public DateTime RegisterDate { get; set; }

    [Display(Name = "آواتار")]
    [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
    public string UserAvatar { get; set; }

    public bool IsDelete { get; set; }

    #region Relations

    public virtual List<UserRole> UsersRoles { get; set; }
    public virtual List<Wallet.Wallet> Wallets { get; set; }
    public virtual List<Course.Course> Courses { get; set; }
    public virtual List<Order.Order> Orders { get; set; }
    public List<UserCourse> UserCourses { get; set; }
    public List<UserDiscountCode> UserDiscountCodes { get; set; }
    public List<CourseComment> CourseComments { get; set; }
    public List<CourseVote>? CourseVotes { get; set; }
    public List<Question.Question>? Questions { get; set; }
    public List<Answer>? Answers { get; set; }

    #endregion
}