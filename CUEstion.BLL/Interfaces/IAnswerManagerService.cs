using System.Collections.Generic;
using System.Threading.Tasks;
using CUEstion.BLL.ModelsDTO;

namespace CUEstion.BLL.Interfaces
{
    public interface IAnswerManagerService
    {
        public Task<IEnumerable<AnswerDTO>> GetAnswers(int questionId);
        public Task CreateAnswer(AnswerDTO answerDto, int questionId);
        public Task UpdateAnswer(AnswerDTO answerDto);
        public Task DeleteAnswer(int answerId);
        public Task MarkAnswer(int userId, int answerId, int mark);
    }
}