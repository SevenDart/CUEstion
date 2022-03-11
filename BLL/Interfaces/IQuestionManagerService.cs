using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.ModelsDTO;

namespace BLL.Interfaces
{
    public interface IQuestionManagerService
    {
        public Task<IEnumerable<QuestionDto>> GetAllQuestions();
        public Task<IEnumerable<QuestionDto>> GetNewestQuestions(int count);
        public Task<QuestionDto> GetQuestion(int questionId);
        public Task<IEnumerable<QuestionDto>> Search(string query, string[] tags);
        public Task<IEnumerable<QuestionDto>> FilterQuestions(string[] tags);
        public Task<IEnumerable<QuestionDto>> GetUsersQuestions(int userId);
        public Task CreateQuestion(QuestionDto questionDto);
        public Task UpdateQuestion(QuestionDto questionDto);
        public Task DeleteQuestion(int questionId);
        public Task MarkQuestion(int userId, int questionId, int markValue);
    }
}