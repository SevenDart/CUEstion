using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.ModelsDTO;

namespace BLL.Interfaces
{
    public interface ICommentManagerService
    {
        public Task<IEnumerable<CommentDto>> GetComments(int? questionId, int? answerId);
        public Task CreateComment(CommentDto commentDto, int? questionId, int? answerId);
        public Task UpdateComment(CommentDto commentDto);
        public Task DeleteComment(int commentId);
        public Task MarkComment(int userId, int commentId, int newMarkValue);
    }
}