using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.ModelsDTO;

namespace BLL.Interfaces
{
    public interface IAnswerManagerService
    {
        public Task<IEnumerable<AnswerDto>> GetAnswers(int questionId);
        public Task CreateAnswer(AnswerDto answerDto, int questionId);
        public Task UpdateAnswer(AnswerDto answerDto);
        public Task DeleteAnswer(int answerId);
        public Task MarkAnswer(int userId, int answerId, int mark);
    }
}