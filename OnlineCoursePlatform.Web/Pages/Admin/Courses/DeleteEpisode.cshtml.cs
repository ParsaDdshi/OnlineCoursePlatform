using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Course;

namespace OnlineCoursePlatform.Web.Pages.Admin.Courses
{
    [PermissionChecker(24)]
    public class DeleteEpisodeModel : PageModel
    {
        private readonly ICourseService _courseService;
        public DeleteEpisodeModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        public CourseEpisode Episode { get; set; }
        public void OnGet(int id)
        {
            Episode = _courseService.GetEpisodeById(id);
        }

        public IActionResult OnPost()
        {
            _courseService.DeleteEpisode(Episode.EpisodeId);
            return Redirect("/Admin/Courses/EpisodeIndex/" + Episode.CourseId);
        }
    }
}
