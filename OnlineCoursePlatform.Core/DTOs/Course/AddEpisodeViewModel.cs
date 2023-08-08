using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OnlineCoursePlatform.Core.DTOs.Course
{
    public class AddEpisodeViewModel
    {
        public int CourseId { get; set; }

        [Display(Name = "عنوان بخش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [MaxLength(400, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string EpisodeTitle { get; set; }

        [Display(Name = "زمان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public TimeSpan EpisodeTime { get; set; }

        [Display(Name = "رایگان")]
        public bool IsFree { get; set; }

        [Display(Name = "فایل بخش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public IFormFile EpisodeFile { get; set; }
    }
}
