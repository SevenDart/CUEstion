using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.ModelsDTO;

namespace BLL.Interfaces
{
    public interface IQuestionManagerService
    {
        public Task<IEnumerable<QuestionDto>> GetAllQuestionsAsync(int? workspaceId);
        public Task<IEnumerable<QuestionDto>> GetNewestQuestionsAsync(int count, int? workspaceId);
        public Task<QuestionDto> GetQuestionAsync(int questionId);
        public Task<IEnumerable<QuestionDto>> Search(string query, string[] tags, int? workspaceId);
        public Task<IEnumerable<QuestionDto>> GetQuestionsCreatedByUser(int userId, int? workspaceId);
        public Task CreateQuestion(QuestionDto questionDto);
        public Task UpdateQuestion(QuestionDto questionDto);
        public Task DeleteQuestion(int questionId);
        public Task MarkQuestion(int userId, int questionId, int markValue);
    }
}