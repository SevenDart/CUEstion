using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.ModelsDTO;
using DAL.EF;
using DAL.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementations
{
    public class CommentManagerService : ICommentManagerService
    {
        private readonly ApplicationContext _context;

        public CommentManagerService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CommentDto>> GetComments(int? questionId, int? answerId)
        {
            IEnumerable<CommentDto> comments;
            if (questionId != null)
            {
                comments = await _context
                    .QuestionComments
                    .Include(c => c.User)
                    .Where(c => c.QuestionId == questionId)
                    .ProjectToType<CommentDto>()
                    .ToListAsync();
            }
            else
            {
                comments = await _context
                    .AnswerComments
                    .Include(c => c.User)
                    .Where(c => c.AnswerId == answerId)
                    .ProjectToType<CommentDto>()
                    .ToListAsync();
            }

            return comments;
        }

        public async Task CreateComment(CommentDto commentDto, int? questionId, int? answerId)
        {
            Comment comment;
            if (questionId != null)
            {
                comment = new QuestionComment()
                {
                    Text = commentDto.Text,
                    CreateTime = DateTime.Now,
                    QuestionId = questionId,
                    UserId = commentDto.User.Id,
                };
            }
            else
            {
                comment = new AnswerComment()
                {
                    Text = commentDto.Text,
                    CreateTime = DateTime.Now,
                    AnswerId = answerId,
                    UserId = commentDto.User.Id,
                };
            }
            
            _context.Comments.Add(comment);
            
            await _context.SaveChangesAsync();
        }

        public async Task UpdateComment(CommentDto commentDto)
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
            var commentMark = await _context
                .CommentMarks
                .FindAsync(userId, commentId);

            if (commentMark != null && commentMark.Mark == mark)
            {
                throw new ApplicationException("This user have already voted such.");
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

            //TODO add trigger
            commentMark.Mark += mark;
            var comment = await _context.Comments.FindAsync(commentId);
            var user = await _context.Users.FindAsync(comment.UserId);
            comment.Rate += mark;
            user.Rate += mark;

            await _context.SaveChangesAsync();
        }
    }
}