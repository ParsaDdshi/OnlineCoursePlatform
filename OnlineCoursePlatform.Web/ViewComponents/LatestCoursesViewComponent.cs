using Microsoft.AspNetCore.Mvc;
using OnlineCoursePlatform.Core.Services.Interfaces;
using System.Xml.Serialization;

namespace OnlineCoursePlatform.Web.ViewComponents
{
    public class LatestCoursesViewComponent : ViewComponent
    {
        private readonly ICourseService _courseService;
        public LatestCoursesViewComponent(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("LatestCourses", _courseService.GetCourses().Item1));
        }
    }
}
