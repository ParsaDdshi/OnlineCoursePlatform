using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Course;

namespace OnlineCoursePlatform.Web.Pages.Admin.Courses
{
    [PermissionChecker(19)]
    public class EditCourseModel : PageModel
    {
        private readonly ICourseService _courseService;
        public EditCourseModel(ICourseService courseService) => _courseService = courseService;

        [BindProperty]
        public Course Course { get; set; }
        public void OnGet(int id)
        {
            Course = _courseService.GetCourseById(id);

            List<SelectListItem> groups = _courseService.GetGroupForManageCourse();
            ViewData["Gruops"] = new SelectList(groups, "Value", "Text", Course.GroupId);

            List<SelectListItem> subGroups = new List<SelectListItem>()
            {
                new SelectListItem(){Text = "لطفا یک زیرگروه را انتخاب کنید", Value = null}
            };
            subGroups.AddRange(_courseService.GetSubGroupForManageCourse(Course.GroupId));
            string selectedSubGroup = null;
            if(Course.SubGroup != null)
            {
                selectedSubGroup = Course.SubGroup.ToString();
            }          
            ViewData["SubGruops"] = new SelectList(subGroups, "Value", "Text", Course.SubGroup);

            List<SelectListItem> teachers = _courseService.GetTeachers();
            ViewData["Teachers"] = new SelectList(teachers, "Value", "Text", Course.TeacherId);

            List<SelectListItem> levels = _courseService.GetLevels();
            ViewData["Levels"] = new SelectList(levels, "Value", "Text", Course.LevelId);

            List<SelectListItem> status = _courseService.GetStatus();
            ViewData["Status"] = new SelectList(status, "Value", "Text", Course.StatusId);
        }

        public IActionResult OnPost(IFormFile? imgCourseUp, IFormFile? demoUp)
        {
            if(!ModelState.IsValid)
            {
                List<SelectListItem> groups = _courseService.GetGroupForManageCourse();
                ViewData["Gruops"] = new SelectList(groups, "Value", "Text", Course.GroupId);

                List<SelectListItem> subGroups = new List<SelectListItem>()
            {
                new SelectListItem(){Text = "لطفا یک زیرگروه را انتخاب کنید", Value = null}
            };
                subGroups.AddRange(_courseService.GetSubGroupForManageCourse(Course.GroupId));
                string selectedSubGroup = null;
                if (Course.SubGroup != null)
                {
                    selectedSubGroup = Course.SubGroup.ToString();
                }
                ViewData["SubGruops"] = new SelectList(subGroups, "Value", "Text", Course.SubGroup ?? 0);

                List<SelectListItem> teachers = _courseService.GetTeachers();
                ViewData["Teachers"] = new SelectList(teachers, "Value", "Text", Course.TeacherId);

                List<SelectListItem> levels = _courseService.GetLevels();
                ViewData["Levels"] = new SelectList(levels, "Value", "Text", Course.LevelId);

                List<SelectListItem> status = _courseService.GetStatus();
                ViewData["Status"] = new SelectList(status, "Value", "Text", Course.StatusId);

                return Page();
            }

            _courseService.UpdateCourse(Course, imgCourseUp, demoUp);

            return Redirect("/Admin");
        }
    }
}
