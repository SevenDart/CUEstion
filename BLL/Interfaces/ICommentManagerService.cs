using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.ModelsDTO;

namespace BLL.Interfaces
{
    public interface ICommentManagerService
    {
        public Task<IEnumerable<CommentDto>> GetCommentsAsync(int? questionId, int? answerId);
        public Task<CommentDto> GetCommentAsync(int commentId);
        public Task CreateCommentAsync(CommentDto commentDto, int? questionId, int? answerId);
        public Task UpdateCommentAsync(CommentDto commentDto);
        public Task DeleteCommentAsync(int commentId);
        public Task MarkCommentAsync(int userId, int commentId, int newMarkValue);
    }
}