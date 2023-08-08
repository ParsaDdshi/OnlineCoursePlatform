using OnlineCoursePlatform.DataLayer.Entities.Question;

namespace OnlineCoursePlatform.Core.Services.Interfaces
{
    public interface IForumService
    {
        #region Question

        int AddQuestion(Question question);
        Question ShowQuestion(int questionId);
        Tuple<List<Question>, int> GetQuestions(int? courseId, string filter = "", int pageId = 1);
        int GetQuestionCourseId(int questionId);

        #endregion

        #region Answer

        void AddAnswer(Answer answer);
        void UpdateAnswerQualification(int answerId, int questionId);

        #endregion

        void Save();
    }
}
