using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Course;

namespace OnlineCoursePlatform.Web.Pages.Admin.Courses
{
    [PermissionChecker(21)]
    public class EpisodeIndexModel : PageModel
    {
        private readonly ICourseService _courseService;
        public EpisodeIndexModel(ICourseService courseService) => _courseService = courseService;

        public List<CourseEpisode> CourseEpisodes { get; set; }
        public void OnGet(int id)
        {
            ViewData["CourseId"] = id;
            CourseEpisodes = _courseService.GetCourseEpisodes(id);
        }
    }
}
