using System.Threading.Tasks;
using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IMarkManagerService
    {
        public Task SetMarkAsync(Mark mark, int newMarkValue);
        public Task<QuestionMark> GetQuestionMarkAsync(int userId, int questionId);
        public Task<AnswerMark> GetAnswerMarkAsync(int userId, int answerId);
        public Task<CommentMark> GetCommentMarkAsync(int userId, int commentId);
    }
}