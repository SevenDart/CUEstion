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
    public class AnswerManagerService : IAnswerManagerService
    {
        private readonly ApplicationContext _context;
        private readonly IMarkManagerService _markManagerService;

        public AnswerManagerService(ApplicationContext context, IMarkManagerService markManagerService)
        {
            _context = context;
            _markManagerService = markManagerService;
        }

        public async Task<IEnumerable<AnswerDto>> GetAnswersAsync(int questionId)
        {
            var answers = await _context
                .Answers
                .Include(a => a.User)
                .Where(a => a.QuestionId == questionId)
                .ProjectToType<AnswerDto>()
                .ToListAsync();

            return answers;
        }

        public async Task<AnswerDto> GetAnswerAsync(int answerId)
        {
            var answer = await _context
                .Answers
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == answerId);
            
            return answer.Adapt<AnswerDto>();
        }

        public async Task CreateAnswerAsync(AnswerDto answerDto, int questionId)
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

        public async Task UpdateAnswer(AnswerDto answerDto)
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

            _context.Answers.Remove(answer);

            await _context.SaveChangesAsync();
        }
        
        public async Task MarkAnswerAsync(int userId, int answerId, int newMarkValue)
        {
            var answerMark = await _context
                .AnswerMarks
                .FindAsync(userId, answerId);
            
            if (answerMark == null)
            {
                answerMark = new AnswerMark()
                {
                    AnswerId = answerId,
                    UserId = userId,
                    MarkValue = 0
                };
                _context.AnswerMarks.Add(answerMark);
            }
            
            await _markManagerService.SetMarkAsync(answerMark, newMarkValue);
            
            var answer = await _context.Answers.FindAsync(answerId);
            answer.Rate += newMarkValue;

            await _context.SaveChangesAsync();
        }
    }
}