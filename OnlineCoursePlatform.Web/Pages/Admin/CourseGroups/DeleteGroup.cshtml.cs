using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Course;

namespace OnlineCoursePlatform.Web.Pages.Admin.CourseGroups
{
    [PermissionChecker(15)]
    public class DeleteGroupModel : PageModel
    {
        private readonly ICourseService _courseService;
        public DeleteGroupModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        public CourseGroup Group { get; set; }
        public void OnGet(int id)
        {
            Group = _courseService.GetGroupById(id);
        }

        public IActionResult OnPost()
        {
            _courseService.DeleteGroup(Group.GroupId);
            return RedirectToPage("Index");
        }
    }
}
