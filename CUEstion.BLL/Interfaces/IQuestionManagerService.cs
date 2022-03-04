using System;
using System.Collections.Generic;
using CUEstion.BLL.ModelsDTO;
using System.Threading.Tasks;

namespace CUEstion.BLL.Interfaces
{
    public interface IQuestionManagerService
    {
        public Task<IEnumerable<QuestionDTO>> GetAllQuestions();
        public Task<IEnumerable<QuestionDTO>> GetNewestQuestions(int count);
        public Task<QuestionDTO> GetQuestion(int questionId);
        public Task<IEnumerable<QuestionDTO>> Search(string query, string[] tags);
        public Task<IEnumerable<QuestionDTO>> FilterQuestions(string[] tags);
        public Task<IEnumerable<QuestionDTO>> GetUsersQuestions(int userId);
        public Task CreateQuestion(QuestionDTO questionDto);
        public Task UpdateQuestion(QuestionDTO questionDto);
        public Task DeleteQuestion(int questionId);
        public Task SubscribeToQuestion(int questionId, int userId);
        public Task MarkQuestion(int userId, int questionId, int mark);
        public Task<bool> IsSubscribedToQuestion(int questionId, int userId);
        public Task UnsubscribeFromQuestion(int questionId, int userId);
    }
}