using Microsoft.AspNetCore.Mvc;
using OnlineCoursePlatform.Core.Services.Interfaces;

namespace OnlineCoursePlatform.Web.ViewComponents
{
    public class CourseGroupComponent : ViewComponent
    {
        private readonly ICourseService _courseService;
        public CourseGroupComponent(ICourseService courseService) => _courseService = courseService;
       

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult) View("CourseGroup", _courseService.GetAllGroups()));
        }
    }
}
