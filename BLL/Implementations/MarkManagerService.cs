using System;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.EF;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

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
            mark.MarkValue += newMarkValue;
            
            var user = await _context.Users.FindAsync(mark.UserId);
            user.Rate += newMarkValue;

            await _context.SaveChangesAsync();
        }

        public async Task<QuestionMark> GetQuestionMarkAsync(int userId, int questionId)
        {
            var mark = await _context
                .QuestionMarks
                .FirstOrDefaultAsync(qm => qm.UserId == userId && qm.QuestionId == questionId);
            
            return mark;
        }

        public async Task<AnswerMark> GetAnswerMarkAsync(int userId, int answerId)
        {
            var mark = await _context
                .AnswerMarks
                .FirstOrDefaultAsync(qm => qm.UserId == userId && qm.AnswerId == answerId);
            
            return mark;
        }

        public async Task<CommentMark> GetCommentMarkAsync(int userId, int commentId)
        {
            var mark = await _context
                .CommentMarks
                .FirstOrDefaultAsync(qm => qm.UserId == userId && qm.CommentId == commentId);
            
            return mark;
        }
    }
}