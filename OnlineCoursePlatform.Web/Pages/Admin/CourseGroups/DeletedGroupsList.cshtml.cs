using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Course;

namespace OnlineCoursePlatform.Web.Pages.Admin.CourseGroups
{
    [PermissionChecker(16)]
    public class DeletedGroupsListModel : PageModel
    {
        private readonly ICourseService _courseService;
        public DeletedGroupsListModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public List<CourseGroup> CourseGroups { get; set; }
        public void OnGet()
        {
            CourseGroups = _courseService.GetDeletedGroups();
        }
    }
}
