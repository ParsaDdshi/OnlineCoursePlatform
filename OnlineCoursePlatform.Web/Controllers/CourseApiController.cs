using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineCoursePlatform.Core.Services.Interfaces;
using System.Web;
using System;

namespace OnlineCoursePlatform.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseApiController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseApiController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [Produces("application/json")]
        [HttpGet("search")]
        public async Task<IActionResult> Search()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                List<string> result = _courseService.GetCourseTitlesForSearch(term);
                return Ok(result);
            }
            catch { return BadRequest(); }
        }
    }
}
