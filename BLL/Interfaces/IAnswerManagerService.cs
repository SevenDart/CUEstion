using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.ModelsDTO;

namespace BLL.Interfaces
{
    public interface IAnswerManagerService
    {
        public Task<IEnumerable<AnswerDto>> GetAnswersAsync(int questionId);
        public Task<AnswerDto> GetAnswerAsync(int answerId);
        public Task CreateAnswerAsync(AnswerDto answerDto, int questionId);
        public Task UpdateAnswer(AnswerDto answerDto);
        public Task DeleteAnswer(int answerId);
        public Task MarkAnswerAsync(int userId, int answerId, int newMarkValue);
    }
}