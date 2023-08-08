using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Course;

namespace OnlineCoursePlatform.Web.Pages.Admin.CourseGroups
{
    [PermissionChecker(14)]
    public class EditGroupModel : PageModel
    {
        private readonly ICourseService _courseService;
        public EditGroupModel(ICourseService courseService)
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
            if (!ModelState.IsValid) return Page();

            _courseService.UpdateGroup(Group);

            return RedirectToPage("Index");
        }
    }
}
