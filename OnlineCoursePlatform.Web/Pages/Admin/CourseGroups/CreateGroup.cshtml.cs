using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Course;

namespace OnlineCoursePlatform.Web.Pages.Admin.CourseGroups
{
    [PermissionChecker(13)]
    public class CreateGroupModel : PageModel
    {
        private readonly ICourseService _courseService;
        public CreateGroupModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        public CourseGroup Group { get; set; }
        public void OnGet(int? id)
        {
            Group = new CourseGroup() { ParentId = id};
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            _courseService.AddGroup(Group);

            return RedirectToPage("Index");
        }
    }
}
