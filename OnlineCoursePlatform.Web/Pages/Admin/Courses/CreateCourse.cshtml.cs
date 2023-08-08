using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Course;

namespace OnlineCoursePlatform.Web.Pages.Admin.Courses
{
    [PermissionChecker(18)]
    public class CreateCourseModel : PageModel
    {
        private readonly ICourseService _courseService;
        public CreateCourseModel(ICourseService courseService) => _courseService = courseService;

        [BindProperty]
        public Course Course { get; set; }

        public void OnGet()
        {
            List<SelectListItem> groups = _courseService.GetGroupForManageCourse();
            ViewData["Gruops"] = new SelectList(groups, "Value", "Text");

            List<SelectListItem> subGroups = _courseService.GetSubGroupForManageCourse(int.Parse(groups.First().Value));
            ViewData["SubGruops"] = new SelectList(subGroups, "Value", "Text");

            List<SelectListItem> teachers = _courseService.GetTeachers();
            ViewData["Teachers"] = new SelectList(teachers, "Value", "Text");

            List<SelectListItem> levels = _courseService.GetLevels();
            ViewData["Levels"] = new SelectList(levels, "Value", "Text");

            List<SelectListItem> status = _courseService.GetStatus();
            ViewData["Status"] = new SelectList(status, "Value", "Text");
        }

        public IActionResult OnPost(IFormFile? imgCourseUp, IFormFile? demoUp)
        {
            if (!ModelState.IsValid)
            {
                List<SelectListItem> groups = _courseService.GetGroupForManageCourse();
                ViewData["Gruops"] = new SelectList(groups, "Value", "Text");

                List<SelectListItem> subGroups = new List<SelectListItem>()
                {
                    new SelectListItem(){Text = "لطفا یک زیرگروه را انتخاب کنید", Value = null}
                };
                subGroups.AddRange(_courseService.GetSubGroupForManageCourse(Course.GroupId));
                ViewData["SubGruops"] = new SelectList(subGroups, "Value", "Text");

                List<SelectListItem> teachers = _courseService.GetTeachers();
                ViewData["Teachers"] = new SelectList(teachers, "Value", "Text");

                List<SelectListItem> levels = _courseService.GetLevels();
                ViewData["Levels"] = new SelectList(levels, "Value", "Text");

                List<SelectListItem> status = _courseService.GetStatus();
                ViewData["Status"] = new SelectList(status, "Value", "Text");

                return Page();
            }

            _courseService.InsertCourse(Course, imgCourseUp, demoUp);

            return Redirect("/Admin/Index");
        }
    }
}
