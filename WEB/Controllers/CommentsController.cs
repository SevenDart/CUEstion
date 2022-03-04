using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WEB.Controllers
{
    public class CommentsController : ControllerBase
    {
        private readonly ICommentManagerService _commentManagerService;

        public CommentsController(ICommentManagerService commentManagerService)
        {
            _commentManagerService = commentManagerService;
        }

        [HttpGet("{questionId}/comments")]
        [HttpGet("{questionId}/answers/{answerId}/comments")]
        public async Task<IActionResult> GetComments(int questionId, int? answerId)
        {
            IEnumerable<CommentDto> list;
            if (answerId == null)
                list = await _commentManagerService.GetComments(questionId, null);
            else
                list = await _commentManagerService.GetComments(null, answerId);
            return Ok(list);
        }

        [HttpPost("{questionId}/comments")]
        [HttpPost("{questionId}/answers/{answerId}/comments")]
        [Authorize]
        public async Task<IActionResult> CreateComment(CommentDto commentDto, int questionId, int? answerId)
        {
            if (answerId == null)
                await _commentManagerService.CreateComment(commentDto, questionId, null);
            else
                await _commentManagerService.CreateComment(commentDto, null, answerId);
            return Ok();
        }


        [HttpPut("{questionId}/comments/{commentId}")]
        [HttpPut("{questionId}/answers/{answerId}/comments/{commentId}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(CommentDto commentDto)
        {
            await _commentManagerService.UpdateComment(commentDto);
            return Ok();
        }

        [HttpDelete("{questionId}/comments/{commentId}")]
        [HttpDelete("{questionId}/answers/{answerId}/comments/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            await _commentManagerService.DeleteComment(commentId);
            return Ok();
        }

        [HttpPut("{questionId}/comments/{commentId}/upvote")]
        [HttpPut("{questionId}/answers/{answerId}/comments/{commentId}/upvote")]
        [Authorize]
        public async Task<IActionResult> UpvoteForComment(int commentId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
            await _commentManagerService.MarkComment(userId, commentId, 1);
            return Ok();
        }

        [HttpPut("{questionId}/comments/{commentId}/downvote")]
        [HttpPut("{questionId}/answers/{answerId}/comments/{commentId}/downvote")]
        [Authorize]
        public async Task<IActionResult> DownvoteForComment(int commentId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
            await _commentManagerService.MarkComment(userId, commentId, -1);
            return Ok();
        }
    }
}