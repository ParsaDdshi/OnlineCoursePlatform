using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.DTOs.Course;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;

namespace OnlineCoursePlatform.Web.Pages.Admin
{
    [PermissionChecker(1)]
    [PermissionChecker(17)]
    public class IndexModel : PageModel
    {
        private readonly ICourseService _courseService;
        public IndexModel(ICourseService courseService) => _courseService = courseService;

        public List<ShowCourseForAdminViewModel> Courses { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }

        public void OnGet(string filterCourse, int pageId = 1)
        {
            Tuple<List<ShowCourseForAdminViewModel>, int> model = _courseService.GetCoursesForAdmin(filterCourse, pageId);
            int take = 5;
            Courses = model.Item1;
            PageCount = model.Item2;
            CurrentPage = pageId;
        }
    }
}
