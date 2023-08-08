using Microsoft.EntityFrameworkCore;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Context;
using OnlineCoursePlatform.DataLayer.Entities.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCoursePlatform.Core.Services
{
    public class ForumService : IForumService
    {
        private readonly OCPContext _context;

        public ForumService(OCPContext context)
        {
            _context = context;
        }

        public void AddAnswer(Answer answer)
        {
            _context.Answers.Add(answer);
            Save();
        }

        public int AddQuestion(Question question)
        {
            question.CreateDate = DateTime.Now;
            question.ModifiedDate = DateTime.Now;
            _context.Questions.Add(question);
            Save();
            return question.QuestionId;
        }

        public int GetQuestionCourseId(int questionId) => _context.Questions.Find(questionId).CourseId;

        public Tuple<List<Question>, int> GetQuestions(int? courseId, string filter = "", int pageId = 1)
        {
            IQueryable<Question> result = _context.Questions;

            if (courseId != null) result = result.Where(q => q.CourseId == courseId);
            if (!string.IsNullOrEmpty(filter)) result = result.Where(q => EF.Functions.Like(q.Title, $"%{filter}%"));

            int take = 5;
            int skip = (pageId - 1) * take;
            int pageCount = result.Count() / take;

            return Tuple.Create(result.Include(q => q.Course).Include(q => q.User)
                .Skip(skip).Take(take).ToList(), pageCount);
        }

        public bool IsUserCreateQuestion(int userId, int questionId) => _context.Questions
            .Any(q => q.UserId == userId && q.QuestionId == questionId);

        public void Save() => _context.SaveChanges();


        public Question ShowQuestion(int questionId) => _context.Questions
            .Include(q => q.User).Include(q => q.Answers).ThenInclude(a => a.User).SingleOrDefault(q => q.QuestionId == questionId);

        public void UpdateAnswerQualification(int answerId, int questionId)
        {
            List<Answer> answers = _context.Answers.Where(a => a.QuestionId == questionId).ToList();
            foreach(Answer answer in answers)
            {
                answer.IsTrue = false;
                if(answer.AnswerId == answerId) answer.IsTrue = true;
            }
            Save();
        }
    }
}
