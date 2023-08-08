using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineCoursePlatform.Core.DTOs.Course;
using OnlineCoursePlatform.DataLayer.Entities.Course;

namespace OnlineCoursePlatform.Core.Services.Interfaces
{
    public interface ICourseService
    {
        #region Group

        List<CourseGroup> GetAllGroups();
        List<SelectListItem> GetGroupForManageCourse();
        List<SelectListItem> GetSubGroupForManageCourse(int groupId);
        List<SelectListItem> GetTeachers();
        List<SelectListItem> GetStatus();
        List<SelectListItem> GetLevels();
        void AddGroup(CourseGroup group);
        void UpdateGroup(CourseGroup group);
        CourseGroup GetGroupById(int groupId);
        void DeleteGroup(int groupId);
        List<CourseGroup> GetDeletedGroups();

        #endregion

        #region Course

        int InsertCourse(Course course, IFormFile imgCourse, IFormFile courseDemo);
        Tuple<List<ShowCourseForAdminViewModel>, int> GetCoursesForAdmin(string filterCourse, int pageId = 1);
        Course GetCourseById(int courseId);
        void UpdateCourse(Course course, IFormFile imgCourse, IFormFile courseDemo);
        Tuple<List<ShowCourseListItemViewModel>, int> GetCourses(int pageId = 1, string filter = null, string getType = "all",
            string orederByType = "date", int startPrice = 0,
            int endPrice = 0, List<int> selectedGroups = null, int take = 0);
        List<ShowCourseListItemViewModel> GetPopularCourses();
        void DeleteCourse(int courseId);
        bool IsCourseFree(int courseId);
        List<string> GetCourseTitlesForSearch(string term);
        List<Course> GetAllMasterCourses(string userName);

        #endregion

        #region Episode

        int InsertEpisode(CourseEpisode courseEpisode, IFormFile episodeFile);
        bool InsertEpisodeForMasterPanel(AddEpisodeViewModel episode, string userName);
        bool CheckFileExistence(string fileName);
        List<CourseEpisode> GetCourseEpisodes(int courseId);
        CourseEpisode GetEpisodeById(int episodeId);
        void UpdateEpisode(CourseEpisode courseEpisode, IFormFile? episodeFile);
        Course GetCourseDetailsById(int courseId);
        void DeleteEpisode(int episodeId);
        List<CourseEpisode> GetCourseEpisodesForMasterPanel(int courseId);

        #endregion

        #region Comment

        void AddComment(CourseComment comment);
        Tuple<List<CourseComment>, int> GetComments(int courseId, int pageId = 1);

        #endregion

        #region Course Vote

        void AddVote(int userId, int courseId, bool vote);
        Tuple<int, int> GetCourseVotes(int courseId);

        #endregion

        void DeleteFile(string path);
        void SaveFile(IFormFile file, string path);
        void Save();
    }
}
