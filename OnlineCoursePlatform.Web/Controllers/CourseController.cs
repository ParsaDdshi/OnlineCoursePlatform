using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Course;
using SharpCompress.Archives;

namespace OnlineCoursePlatform.Web.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        public CourseController(ICourseService courseService, IOrderService orderService, IUserService userService)
        {
            _courseService = courseService;
            _orderService = orderService;
            _userService = userService;
        }

        public IActionResult Index(int pageId = 1, string filter = null,
            string getType = "all", string orederByType = "date", int startPrice = 0,
            int endPrice = 0, List<int> selectedGroups = null)
        {
            ViewBag.filter = filter;
            ViewBag.getType = getType;
            ViewBag.selectedGroups = selectedGroups;
            ViewBag.pageId = pageId;

            ViewBag.Groups = _courseService.GetAllGroups();

            return View(_courseService.GetCourses(pageId, filter, getType, orederByType, startPrice, 
                endPrice, selectedGroups, 9));
        }

        [Route("/ShowCourse/{courseId}")]
        public IActionResult ShowCourse(int courseId, int episodeId = 0)
        {
            Course course = _courseService.GetCourseDetailsById(courseId);

            if (course == null)
                return NotFound();

            if (episodeId != 0 && User.Identity.IsAuthenticated)
                if (course.CourseEpisodes.All(e => e.EpisodeId != episodeId)) return NotFound();

            if (episodeId != 0 && !course.CourseEpisodes.Single(e => e.EpisodeId == episodeId).IsFree)
                if (!_orderService.IsUserInCourse(User.Identity.Name, courseId)) return NotFound();

            CourseEpisode? episode = null;
            string filePath = "";
            string checkFilePath = "";

            if (episodeId != 0) episode = course.CourseEpisodes.First(e => e.EpisodeId == episodeId);
            ViewBag.Episode = episode;


            if (episodeId != 0 && episode.IsFree)
            {
                filePath = "/freeCourseOnline/" + episode.EpisodeFileName.Replace(".rar", ".mp4");
                checkFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/freeCourseOnline",
                    episode.EpisodeFileName.Replace(".rar", ".mp4"));
            }
           
            else if(episodeId != 0)
            {
                filePath = "/courseOnline/" + episode.EpisodeFileName.Replace(".rar", ".mp4");
                checkFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/courseOnline",
                    episode.EpisodeFileName.Replace(".rar", ".mp4"));
            }
            

            if(episodeId != 0 && !System.IO.File.Exists(checkFilePath)) 
            {
                string targetPath = Directory.GetCurrentDirectory();

                if(episode.IsFree && episodeId != 0) targetPath = Path.Combine(targetPath, "wwwroot/freeCourseOnline");

                else if(episodeId != 0) targetPath = Path.Combine(targetPath, "wwwroot/courseOnline");

                string rarPath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/courseFiles", episode.EpisodeFileName);

                IArchive archive = ArchiveFactory.Open(rarPath);
                var entries = archive.Entries.OrderBy(x => x.Key.Length);
                foreach(var entry in entries)
                {
                    if (Path.GetExtension(entry.Key) == ".mp4")
                        entry.WriteTo(System.IO.File.Create(Path.Combine(targetPath
                            , episode.EpisodeFileName.Replace(".rar", ".mp4"))));
                }
            }
            ViewBag.FilePath = filePath;

            return View(course);
        }

        [Authorize]
        public IActionResult BuyCourse(int courseId)
        {
            int orderId = _orderService.InsertOrder(User.Identity.Name, courseId);
            return Redirect("/UserPanel/Order/ShowOrder?orderId=" + orderId);
        }

        [Route("DownloadFile/{episodeId}")]
        public IActionResult DownloadFile(int episodeId)
        {
            CourseEpisode episode = _courseService.GetEpisodeById(episodeId);
            string fileName = episode.EpisodeFileName;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/courseFiles", fileName);
            if(episode.IsFree)
            {
                byte[] file = System.IO.File.ReadAllBytes(filePath);
                return File(file, "application/force-download", fileName);
            }

            if (User.Identity.IsAuthenticated)
            {
                if(_orderService.IsUserInCourse(User.Identity.Name, episode.CourseId) 
                    || episode.Course.TeacherId == _userService.GetUserIdByUserName(User.Identity.Name))
                {
                    byte[] file = System.IO.File.ReadAllBytes(filePath);
                    return File(file, "application/force-download", fileName);
                }
            }
            return Forbid();
        }

        [HttpPost]
        public IActionResult CreateComment(CourseComment comment)
        {
            if(comment.Comment != null)
            {
                comment.CreateDate = DateTime.Now;
                comment.UserId = _userService.GetUserIdByUserName(User.Identity.Name);
                _courseService.AddComment(comment);
            }
            return View("ShowComment", _courseService.GetComments(comment.CourseId));
        }

        public IActionResult ShowComment(int id, int pageId = 1)
        {
            return View(_courseService.GetComments(id, pageId));
        }

        [Authorize]
        public IActionResult CourseVote(int courseId)
        {
            if (!_courseService.IsCourseFree(courseId)
                && !_orderService.IsUserInCourse(User.Identity.Name, courseId)) ViewBag.AccessDenied = true;
            return PartialView(_courseService.GetCourseVotes(courseId));
        }

        [Authorize]
        public IActionResult AddVote(int id, bool vote)
        {
            _courseService.AddVote(_userService.GetUserIdByUserName(User.Identity.Name), id, vote);
            return PartialView("CourseVote", _courseService.GetCourseVotes(id));
        }
    }
}
