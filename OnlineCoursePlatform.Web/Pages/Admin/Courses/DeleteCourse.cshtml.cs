using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging.Abstractions;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Course;

namespace OnlineCoursePlatform.Web.Pages.Admin.Courses
{
    [PermissionChecker(20)]
    public class DeleteCourseModel : PageModel
    {
        private readonly ICourseService _courseService;
        public DeleteCourseModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        public Course Course { get; set; }
        public void OnGet(int id)
        {
            Course = _courseService.GetCourseById(id);
        }

        public IActionResult OnPost()
        {
            _courseService.DeleteCourse(Course.CourseId);
            return Redirect("/Admin");
        }
    }
}
