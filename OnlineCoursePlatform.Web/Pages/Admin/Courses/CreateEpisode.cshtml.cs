using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Course;

namespace OnlineCoursePlatform.Web.Pages.Admin.Courses
{
    [PermissionChecker(22)]
    public class CreateEpisodeModel : PageModel
    {
        private readonly ICourseService _courseService;
        public CreateEpisodeModel(ICourseService courseService) => _courseService = courseService;

        [BindProperty]
        public CourseEpisode CourseEpisode { get; set; }
        public void OnGet(int id)
        {
            CourseEpisode = new CourseEpisode();
            CourseEpisode.CourseId = id;
        }

        public IActionResult OnPost(IFormFile episodeFile)
        {
            CourseEpisode.EpisodeFileName = episodeFile.FileName.ToString();
            if(!ModelState.IsValid || episodeFile == null)
                return Page();

            if (_courseService.CheckFileExistence(episodeFile.FileName))
            {
                ViewData["FileExistence"] = true;
                return Page();
            }

            _courseService.InsertEpisode(CourseEpisode, episodeFile);

            return Redirect("/Admin/Courses/EpisodeIndex/" + CourseEpisode.CourseId);
        }
    }
}
