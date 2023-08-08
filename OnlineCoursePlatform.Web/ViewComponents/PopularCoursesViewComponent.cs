using Microsoft.AspNetCore.Mvc;
using OnlineCoursePlatform.Core.Services.Interfaces;

namespace OnlineCoursePlatform.Web.ViewComponents
{
    public class PopularCoursesViewComponent : ViewComponent
    {
        private readonly ICourseService _courseService;
        public PopularCoursesViewComponent(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("PopularCourses", _courseService.GetPopularCourses()));
        }
    }
}
