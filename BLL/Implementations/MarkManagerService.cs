using System;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.EF;
using DAL.Entities;

namespace BLL.Implementations
{
    public class MarkManagerService: IMarkManagerService
    {
        private readonly ApplicationContext _context;

        public MarkManagerService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task SetMarkAsync(Mark mark, int newMarkValue)
        {
            mark!.MarkValue += newMarkValue;
            
            var user = await _context.Users.FindAsync(mark.MarkValue);
            user.Rate += newMarkValue;

            await _context.SaveChangesAsync();
        }

        public async Task<QuestionMark> GetQuestionMarkAsync(int userId, int questionId)
        {
            var mark = await _context.QuestionMarks.FindAsync(userId, questionId);
            return mark;
        }

        public async Task<AnswerMark> GetAnswerMarkAsync(int userId, int answerId)
        {
            var mark = await _context.AnswerMarks.FindAsync(userId, answerId);
            return mark;
        }

        public async Task<CommentMark> GetCommentMarkAsync(int userId, int commentId)
        {
            var mark = await _context.CommentMarks.FindAsync(userId, commentId);
            return mark;
        }
    }
}