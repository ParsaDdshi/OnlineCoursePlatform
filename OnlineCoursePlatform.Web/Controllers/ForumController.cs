using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.Course;
using OnlineCoursePlatform.DataLayer.Entities.Question;
using System.Security.Claims;

namespace OnlineCoursePlatform.Web.Controllers
{
    public class ForumController : Controller
    {
        private readonly IForumService _forumService;
        private readonly IOrderService _orderService;
        public ForumController(IForumService forumService, IOrderService orderService)
        {
            _forumService = forumService;
            _orderService = orderService;

        }

        [Authorize]
        public IActionResult Index(int? courseId, string filter = "", int pageId = 1) 
        {
            ViewBag.CourseId = courseId;
            ViewBag.Filter = filter;
            return View(_forumService.GetQuestions(courseId, filter, pageId));
        } 

        #region Create Question

        [Authorize]
        public IActionResult CreateQuestion(int courseId) => View(new Question()
        {
            CourseId = courseId
        });

        [Authorize]
        [HttpPost]
        public IActionResult CreateQuestion(Question question)
        {
            if (!ModelState.IsValid) return View(question);

            HtmlSanitizer sanitizer = new HtmlSanitizer();
            question.Body = sanitizer.Sanitize(question.Body);

            question.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
            int questionId = _forumService.AddQuestion(question);
            return RedirectToAction("ShowQuestion", new { questionId = questionId, courseId = question.CourseId});
        }

        #endregion

        #region Show Question

        public IActionResult ShowQuestion(int questionId, int courseId)
        {
            ViewBag.CourseId = courseId;
            return View(_forumService.ShowQuestion(questionId));
        }

        #endregion

        #region Answer

        [Authorize]
        public IActionResult CreateAnswer(int questionId, string body)
        {
            if (!string.IsNullOrEmpty(body))
            {
                HtmlSanitizer sanitizer = new HtmlSanitizer();
                body = sanitizer.Sanitize(body);

                Answer answer = new Answer()
                {
                    AnswerBody = body,
                    QuestionId = questionId,
                    CreateDate = DateTime.Now,
                    UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString())
                };
                _forumService.AddAnswer(answer);
            } 
            return RedirectToAction("ShowQuestion", new { questionId = questionId,courseId = _forumService.GetQuestionCourseId(questionId)});
        }

        [Authorize]
        public IActionResult UpdateTrueAnswer(int answerId, int questionId)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
            Question question = _forumService.ShowQuestion(questionId);

            if(question.UserId == currentUserId 
                || question.Course.TeacherId == currentUserId) _forumService.UpdateAnswerQualification(answerId, questionId);

            return RedirectToAction("ShowQuestion", new { questionId = questionId, courseId = _forumService.GetQuestionCourseId(questionId)});
        }

        #endregion
    }
}
