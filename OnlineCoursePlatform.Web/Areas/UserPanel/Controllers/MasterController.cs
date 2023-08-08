using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCoursePlatform.Core.DTOs.Course;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Course;

namespace OnlineCoursePlatform.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    [PermissionChecker(17)]
    public class MasterController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;
        public MasterController(ICourseService courseService, IUserService userService)
        {
            _courseService = courseService;
            _userService = userService;
        }

        public IActionResult MasterCoursesList() => View(_courseService.GetAllMasterCourses(User.Identity.Name));

        [PermissionChecker(21)]
        public IActionResult EpisodeList(int courseId)
        {
            Course course = _courseService.GetCourseById(courseId);
            if (course == null) return NotFound();

            if (course.TeacherId != _userService.GetUserIdByUserName(User.Identity.Name))
                return RedirectToAction("MasterCoursesList", "Master");

            ViewBag.CourseId = courseId;

            return View(_courseService.GetCourseEpisodesForMasterPanel(courseId));
        }

        [PermissionChecker(22)]
        public IActionResult AddEpisode(int courseId)
        {
            Course course = _courseService.GetCourseById(courseId);
            if (course == null) return NotFound();

            if (course.TeacherId != _userService.GetUserIdByUserName(User.Identity.Name))
                return RedirectToAction("MasterCoursesList", "Master");

            AddEpisodeViewModel episode = new AddEpisodeViewModel()
            {
                CourseId = courseId,
            };

            return View(episode);
        }

        [HttpPost]
        [PermissionChecker(22)]
        public IActionResult AddEpisode(AddEpisodeViewModel episode)
        {
            if (!ModelState.IsValid) return View(episode);

            bool result = _courseService.InsertEpisodeForMasterPanel(episode, User.Identity.Name);
            if (result) return RedirectToAction("EpisodeList", "Master", new { courseId = episode.CourseId });

            return View(episode);
        }
    }
}
