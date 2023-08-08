using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineCoursePlatform.Core.Convertors;
using OnlineCoursePlatform.Core.DTOs.Course;
using OnlineCoursePlatform.Core.Generators;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Context;
using OnlineCoursePlatform.DataLayer.Entities.Course;

namespace OnlineCoursePlatform.Core.Services
{
    public class CourseService : ICourseService
    {
        private readonly OCPContext _context;
        private readonly IUserService _userService;
        public CourseService(OCPContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        } 

        public void AddComment(CourseComment comment)
        {
            _context.CourseComments.Add(comment);
            Save();
        }

        public void AddVote(int userId, int courseId, bool vote)
        {
            CourseVote userVote = _context.CourseVotes
                .FirstOrDefault(v => v.UserId == userId && v.CourseId == courseId);

            if (userVote != null) userVote.Vote = vote;
            else _context.CourseVotes.Add(new CourseVote()
            {
                CourseId = courseId,
                UserId = userId,
                Vote = vote
            });

            Save();
        }

        public bool CheckFileExistence(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/courseFiles/", fileName);
            return File.Exists(filePath);
        }

        public void DeleteCourse(int courseId)
        {
            Course course = GetCourseById(courseId);
            _context.CourseComments.Where(c => c.CourseId == courseId)
                .ToList().ForEach(c => _context.CourseComments.Remove(c));

            if(course.CourseImageName != "CourseDefault.png")
            {
                string imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/img", course.CourseImageName);
                DeleteFile(imgPath);

                string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/thumb", course.CourseImageName);
                DeleteFile(thumbPath);
            }

            if(course.DemoFileName != "demo.mp4")
            {
                string demoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/demo", course.DemoFileName);
                DeleteFile(demoPath);
            }

            if (course.CoursePrice != 0) _context.OrderDetails.Where(d => d.CourseId == courseId)
                .ToList().ForEach(d => _context.OrderDetails.Remove(d));


            List<CourseEpisode> episodes = GetCourseEpisodes(courseId);
            foreach (CourseEpisode episode in episodes)
            {
                DeleteEpisode(episode.EpisodeId);
            }
            course.IsDelete = true;
            Save();
        }

        public void DeleteEpisode(int episodeId)
        {
            CourseEpisode episode = GetEpisodeById(episodeId);
            if (episode.EpisodeFileName != "courseFile.rar" && CheckFileExistence(episode.EpisodeFileName))
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/courseFiles/", episode.EpisodeFileName);
                DeleteFile(filePath);
            }
            _context.CourseEpisodes.Remove(episode);
            Save();
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public void DeleteGroup(int groupId)
        {
            List<CourseGroup> subs = _context.CourseGroups.Where(g => g.ParentId == groupId).ToList();
            foreach (CourseGroup sub in subs)
                sub.IsDelete = true;

            CourseGroup group = GetGroupById(groupId);
            group.IsDelete = true;

            Save();
        }

        public List<CourseGroup> GetAllGroups() => _context.CourseGroups
            .Include(g => g.CourseGroups).ToList();

        public List<Course> GetAllMasterCourses(string userName) => _context.Courses
            .Where(c => c.TeacherId == _userService.GetUserIdByUserName(userName))
            .Include(c => c.CourseStatus).Include(c => c.CourseEpisodes).ToList();

        public Tuple<List<CourseComment>, int> GetComments(int courseId, int pageId = 1) 
        {
            int take = 5;
            int skip = (pageId - 1) * take;
            int pageCount = _context.CourseComments
                .Where(c => c.CourseId == courseId).Count() / take;

            if((pageCount % 2) != 0)
            {
                pageCount++;
            }

            return Tuple.Create(
                _context.CourseComments
                .Where(c => c.CourseId == courseId)
                .OrderByDescending(c => c.CreateDate).Include(c => c.User)
                .Skip(skip).Take(take).ToList()
                , pageCount);
        } 


        public Course GetCourseById(int courseId) => _context.Courses.Find(courseId);

        public Course GetCourseDetailsById(int courseId) => _context.Courses
            .Include(c => c.CourseEpisodes).Include(c => c.CourseLevel)
            .Include(c => c.CourseStatus).Include(c => c.User)
            .Include(c => c.UserCourses).FirstOrDefault(c => c.CourseId == courseId);

        public List<CourseEpisode> GetCourseEpisodes(int courseId) => _context.CourseEpisodes
            .Where(e => e.CourseId == courseId).ToList();

        public List<CourseEpisode> GetCourseEpisodesForMasterPanel(int courseId) => _context.CourseEpisodes
            .Where(e => e.CourseId == courseId).Include(e => e.Course).ToList();

        public Tuple<List<ShowCourseListItemViewModel>, int> GetCourses(int pageId = 1, string filter = null,
            string getType = "all", string orederByType = "date", int startPrice = 0, 
            int endPrice = 0, List<int> selectedGroups = null, int take = 0)
        {
            IQueryable<Course> result = _context.Courses.Include(c => c.CourseEpisodes);

            if (take == 0)
                take = 8;

            if (!string.IsNullOrEmpty(filter))
                result = result.Where(c => c.CourseTitle.Contains(filter) || c.Tags.Contains(filter));

            switch(getType)
            {
                case "all":
                    break;

                case "buy":
                {
                    result = result.Where(c => c.CoursePrice != 0);
                    break;
                }


                case "free":
                {
                    result = result.Where(c => c.CoursePrice == 0);
                    break;
                }                   
            }

            switch(orederByType)
            {
                case "date":
                {
                    result = result.OrderByDescending(c => c.CreateDate);
                    break;
                }

                case "updateDate":
                {
                    result = result.OrderByDescending(c => c.UpdateDate);
                    break;
                }
            }

            if (startPrice > 0)
                result = result.Where(c => c.CoursePrice > startPrice);

            if (endPrice > 0)
                result = result.Where(c => c.CoursePrice < endPrice);

            if (selectedGroups != null && selectedGroups.Any())
            {
                List<Course> courses = new List<Course>();
                foreach (int groupId in selectedGroups)
                {
                    IQueryable<Course> groupCourses = result.Where(c => c.GroupId == groupId || c.SubGroup == groupId);
                    foreach(Course course in groupCourses)
                    {
                        courses.Add(course);
                    }
                }
                result = courses.Distinct().AsQueryable();
            }

            int skip = (pageId - 1) * take;

            List<ShowCourseListItemViewModel> list = new List<ShowCourseListItemViewModel>();
            foreach(Course course in result)
            {
                list.Add(new ShowCourseListItemViewModel()
                {
                    CourseId =  course.CourseId,
                    ImageName = course.CourseImageName,
                    Price = course.CoursePrice,
                    Title = course.CourseTitle,
                    TotalTime = new TimeSpan(course.CourseEpisodes.Sum(e => e.EpisodeTime.Ticks))
                });
            }

            int pageCount = list.Count() / take + 1;

            return Tuple.Create(list.Skip(skip).Take(take).ToList(), pageCount);
        }

        public Tuple<List<ShowCourseForAdminViewModel>, int> GetCoursesForAdmin(string filterCourse, int pageId = 1)
        {
            IQueryable<Course> courses = _context.Courses.Include(c => c.CourseEpisodes);

            int take = 5;
            int skip = (pageId - 1) * take;
            int pageCount = courses.Count() / take + 1;

            if (!string.IsNullOrEmpty(filterCourse))
                courses = courses.Where(c => c.CourseTitle.Contains(filterCourse));

            List<ShowCourseForAdminViewModel> list = new List<ShowCourseForAdminViewModel>();
            foreach (Course course in courses)
            {
                ShowCourseForAdminViewModel model = new ShowCourseForAdminViewModel();
                model.CourseId = course.CourseId;
                model.Title = course.CourseTitle;
                model.EpisodeCount = course.CourseEpisodes.Count;
                model.ImageName = course.CourseImageName;

                list.Add(model);
            }

            return Tuple.Create(list.Skip(skip).Take(take).ToList(), pageCount);
        }

        public List<string> GetCourseTitlesForSearch(string term) => _context.Courses
            .Where(c => c.CourseTitle.Contains(term)).Select(c => c.CourseTitle).ToList();

        public Tuple<int, int> GetCourseVotes(int courseId)
        {
            List<bool> votes = _context.CourseVotes.Where(v => v.CourseId == courseId)
                .Select(v => v.Vote).ToList();

            return Tuple.Create(votes.Count(v => v), votes.Count(v => !v));
        }

        public List<CourseGroup> GetDeletedGroups() => _context.CourseGroups.IgnoreQueryFilters()
            .Where(g => g.IsDelete == true).ToList();

        public CourseEpisode GetEpisodeById(int episodeId) => _context.CourseEpisodes.Include(e => e.Course).Single(e => e.EpisodeId == episodeId);

        public CourseGroup GetGroupById(int groupId) => _context.CourseGroups.Find(groupId);

        public List<SelectListItem> GetGroupForManageCourse() => _context.CourseGroups
            .Where(g => g.ParentId == null).Select(g => new SelectListItem()
            {
                Text = g.GroupTitle,
                Value = g.GroupId.ToString()
            }).ToList();

        public List<SelectListItem> GetLevels() => _context.CourseLevels
            .Select(l => new SelectListItem()
            {
                Text = l.LevelTitle,
                Value = l.LevelId.ToString()
            }).ToList();

        public List<ShowCourseListItemViewModel> GetPopularCourses()
        {
            IQueryable<Course> popularCourses = _context.Courses
            .Include(c => c.CourseEpisodes)
            .Include(c => c.OrderDetails)
            .Where(d => d.OrderDetails.Any())
            .OrderByDescending(o => o.OrderDetails.Count)
            .Take(8);

            List<ShowCourseListItemViewModel> result = new List<ShowCourseListItemViewModel>();
            foreach (Course course in popularCourses)
            {
                result.Add(new ShowCourseListItemViewModel()
                {
                    CourseId = course.CourseId,
                    ImageName = course.CourseImageName,
                    Price = course.CoursePrice,
                    Title = course.CourseTitle,
                    TotalTime = new TimeSpan(course.CourseEpisodes.Sum(e => e.EpisodeTime.Ticks))
                });
            }

            return result;
        }

        public List<SelectListItem> GetStatus() => _context.CourseStatuses
            .Select(s => new SelectListItem()
            {
                Text = s.StatusTitle,
                Value= s.StatusId.ToString()
            }).ToList();

        public List<SelectListItem> GetSubGroupForManageCourse(int groupId ) => _context.CourseGroups
            .Where(g => g.ParentId == groupId).Select(g => new SelectListItem()
            {
                Text = g.GroupTitle,
                Value = g.GroupId.ToString()
            }).ToList();

        public List<SelectListItem> GetTeachers() => _context.UsersRoles
            .Where(r => r.RoleId == 2).Include(r => r.User)
            .Select(u => new SelectListItem()
            {
                Text = u.User.UserName,
                Value = u.UserId.ToString()
            }).ToList();

        public int InsertCourse(Course course, IFormFile imgCourse, IFormFile courseDemo)
        {
            course.CreateDate = DateTime.Now;
            course.CourseImageName = "CourseDefault.png";

            if(imgCourse != null && imgCourse.IsImage())
            {
                course.CourseImageName = NameGenerator.GenerateUniqCode() + Path.GetExtension(imgCourse.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/img", course.CourseImageName);
                SaveFile(imgCourse, imagePath);

                ImageConvertor imgResizer = new ImageConvertor();
                string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/thumb", course.CourseImageName);
                imgResizer.Image_resize(imagePath, thumbPath, 500);
            }

            if (courseDemo != null)
            {
                course.DemoFileName = NameGenerator.GenerateUniqCode() + Path.GetExtension(courseDemo.FileName);
                string demoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/demo", course.DemoFileName);
                SaveFile(courseDemo, demoPath);
            }

            _context.Courses.Add(course);
            Save();

            return course.CourseId;
        }

        public int InsertEpisode(CourseEpisode courseEpisode, IFormFile episodeFile)
        {
            if(episodeFile != null)
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/courseFiles/", courseEpisode.EpisodeFileName);
                SaveFile(episodeFile, filePath);
            }

            _context.CourseEpisodes.Add(courseEpisode);
            Save();

            return courseEpisode.EpisodeId;
        }

        public bool InsertEpisodeForMasterPanel(AddEpisodeViewModel episode, string userName)
        {
            Course course = GetCourseById(episode.CourseId);
            int userId = _userService.GetUserIdByUserName(userName);
            if (course == null || course.TeacherId != userId || episode.EpisodeFile == null) return false;

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/courseFiles");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string finalPath = Path.Combine(path, episode.EpisodeFile.FileName);

            SaveFile(episode.EpisodeFile, finalPath);


            _context.CourseEpisodes.Add(new CourseEpisode()
            {
                CourseId = episode.CourseId,
                IsFree = episode.IsFree,
                EpisodeTitle = episode.EpisodeTitle,
                EpisodeTime = episode.EpisodeTime,
                EpisodeFileName = episode.EpisodeFile.FileName
            });
            Save();

            return true;
        }

        public bool IsCourseFree(int courseId) => _context.Courses.Where(c => c.CourseId == courseId)
            .Select(c => c.CoursePrice).Single() == 0;

        public void Save() => _context.SaveChanges();

        public void SaveFile(IFormFile file, string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
                file.CopyTo(stream);
        }

        public void UpdateCourse(Course course, IFormFile imgCourse, IFormFile courseDemo)
        {
            course.UpdateDate = DateTime.Now;

            if (imgCourse != null && imgCourse.IsImage())
            {
                if(course.CourseImageName != "CourseDefault.png")
                {
                    string deleteImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/img",
                        course.CourseImageName);
                    DeleteFile(deleteImagePath);

                    string deleteThumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/thumb",
                       course.CourseImageName);
                    DeleteFile(deleteThumbPath);
                }
                course.CourseImageName = NameGenerator.GenerateUniqCode() + Path.GetExtension(imgCourse.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/img", course.CourseImageName);
                SaveFile(imgCourse, imagePath);

                ImageConvertor imgResizer = new ImageConvertor();
                string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/thumb", course.CourseImageName);
                imgResizer.Image_resize(imagePath, thumbPath, 500);
            }

            if (courseDemo != null)
            {
                if(course.DemoFileName != null)
                {
                    string deleteDemoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/demo",
                        course.DemoFileName);
                    DeleteFile(deleteDemoPath);
                }
                course.DemoFileName = NameGenerator.GenerateUniqCode() + Path.GetExtension(courseDemo.FileName);
                string demoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/demo", course.DemoFileName);
                SaveFile(courseDemo, demoPath);
            }

            _context.Courses.Update(course);
            Save();

        }

        public void UpdateEpisode(CourseEpisode courseEpisode, IFormFile? episodeFile)
        {
            if(episodeFile != null)
            {
                string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/courseFiles/", courseEpisode.EpisodeFileName);
                DeleteFile(deletePath);

                courseEpisode.EpisodeFileName = episodeFile.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/courseFiles/", courseEpisode.EpisodeFileName);
                SaveFile(episodeFile, filePath);
            }

            _context.CourseEpisodes.Update(courseEpisode);
            Save();
        }

        public void UpdateGroup(CourseGroup group)
        {
            _context.CourseGroups.Update(group);
            Save();
        }

        void ICourseService.AddGroup(CourseGroup group)
        {
            _context.CourseGroups.Add(group);
            Save();
        }
    }
}