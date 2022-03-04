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
    public class AnswerManagerService : IAnswerManagerService
    {
        private readonly ApplicationContext _context;

        public AnswerManagerService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AnswerDTO>> GetAnswers(int questionId)
        {
            var answers = await _context
                .Answers
                .Include(a => a.User)
                .Where(a => a.QuestionId == questionId)
                .ProjectToType<AnswerDTO>()
                .ToListAsync();

            return answers;
        }

        public async Task CreateAnswer(AnswerDTO answerDto, int questionId)
        {
            var answer = new Answer()
            {
                Text = answerDto.Text,
                CreateTime = DateTime.Now,
                QuestionId = questionId,
                UserId = answerDto.User.Id
            };

            _context.Answers.Add(answer);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAnswer(AnswerDTO answerDto)
        {
            var answer = await _context
                .Answers
                .FindAsync(answerDto.Id);

            if (answerDto.Text != null)
            {
                answer.Text = answerDto.Text;
            }

            answer.UpdateTime = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnswer(int answerId)
        {
            var answer = await _context
                .Answers
                .FindAsync(answerId);

            var answerComments = await _context.Comments.Where(c => c.AnswerId == answerId).ToListAsync();
            foreach (var comment in answerComments)
            {
                _context.Comments.Remove(comment);
            }

            _context.Answers.Remove(answer);

            await _context.SaveChangesAsync();
        }
        
        public async Task MarkAnswer(int userId, int answerId, int mark)
        {
            var answerMark = await _context.AnswerMarks.FindAsync(userId, answerId);

            if (answerMark != null && answerMark.Mark == mark)
            {
                throw new Exception("This user have already voted such.");
            }
			
            if (answerMark == null)
            {
                answerMark = new AnswerMark()
                {
                    AnswerId = answerId,
                    UserId = userId
                };
                _context.AnswerMarks.Add(answerMark);
            }

            answerMark.Mark += mark;
            var answer = await _context.Answers.FindAsync(answerId);
            var user = await _context.Users.FindAsync(answer.UserId);
            answer.Rate += mark;
            user.Rate += mark;

            await _context.SaveChangesAsync();
        }
    }
}