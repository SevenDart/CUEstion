using System.Collections.Generic;
using System.Threading.Tasks;
using CUEstion.BLL.ModelsDTO;

namespace CUEstion.BLL.Interfaces
{
    public interface ICommentManagerService
    {
        public Task<IEnumerable<CommentDTO>> GetComments(int? questionId, int? answerId);
        public Task CreateComment(CommentDTO commentDto, int? questionId, int? answerId);
        public Task UpdateComment(CommentDTO commentDto);
        public Task DeleteComment(int commentId);
        public Task MarkComment(int userId, int commentId, int mark);
    }
}