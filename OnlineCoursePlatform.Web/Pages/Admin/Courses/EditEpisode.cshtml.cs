using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Course;

namespace OnlineCoursePlatform.Web.Pages.Admin.Courses
{
    [PermissionChecker(23)]
    public class EditEpisodeModel : PageModel
    {
        private readonly ICourseService _courseService;
        public EditEpisodeModel(ICourseService courseService) => _courseService = courseService;

        [BindProperty]
        public CourseEpisode CourseEpisode { get; set; }
        public void OnGet(int id)
        {
            CourseEpisode = _courseService.GetEpisodeById(id);
        }

        public IActionResult OnPost(IFormFile? episodeFile)
        {
            if (!ModelState.IsValid)
                return Page();

            if(episodeFile != null)
            {
                if(_courseService.CheckFileExistence(episodeFile.FileName))
                {
                    ViewData["FileExistence"] = true;
                    return Page();
                }
            }
            _courseService.UpdateEpisode(CourseEpisode, episodeFile);

            return Redirect("/Admin/Courses/EpisodeIndex/" + CourseEpisode.CourseId);
        }
    }
}
