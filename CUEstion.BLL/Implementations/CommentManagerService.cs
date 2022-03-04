using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CUEstion.BLL.Interfaces;
using CUEstion.BLL.ModelsDTO;
using CUEstion.DAL.EF;
using CUEstion.DAL.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CUEstion.BLL.Implementations
{
    public class CommentManagerService : ICommentManagerService
    {
        private readonly ApplicationContext _context;

        public CommentManagerService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CommentDTO>> GetComments(int? questionId, int? answerId)
        {
            var comments = await _context
                .Comments
                .Include(c => c.User)
                .Where(c => c.QuestionId == questionId && c.AnswerId == answerId)
                .ProjectToType<CommentDTO>()
                .ToListAsync();

            return comments;
        }

        public async Task CreateComment(CommentDTO commentDto, int? questionId, int? answerId)
        {
            var comment = new Comment()
            {
                Text = commentDto.Text,
                CreateTime = DateTime.Now,
                QuestionId = questionId,
                AnswerId = answerId,
                UserId = commentDto.User.Id
            };

            _context.Comments.Add(comment);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateComment(CommentDTO commentDto)
        {
            var comment = await _context
                .Comments
                .FindAsync(commentDto.Id);

            if (commentDto.Text != null)
            {
                comment.Text = commentDto.Text;
            }

            comment.UpdateTime = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteComment(int commentId)
        {
            var comment = await _context
                .Comments
                .FindAsync(commentId);

            _context.Comments.Remove(comment);

            await _context.SaveChangesAsync();
        }

        public async Task MarkComment(int userId, int commentId, int mark)
        {
            var commentMark = await _context.CommentMarks.FindAsync(userId, commentId);

            if (commentMark != null && commentMark.Mark == mark)
            {
                throw new Exception("This user have already voted such.");
            }

            if (commentMark == null)
            {
                commentMark = new CommentMark()
                {
                    CommentId = commentId,
                    UserId = userId
                };
                _context.CommentMarks.Add(commentMark);
            }

            commentMark.Mark += mark;
            var comment = await _context.Comments.FindAsync(commentId);
            var user = await _context.Users.FindAsync(comment.UserId);
            comment.Rate += mark;
            user.Rate += mark;

            await _context.SaveChangesAsync();
        }
    }
}