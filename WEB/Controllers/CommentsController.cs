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
        private readonly IQuestionManagerService _questionManagerService;
        private readonly IAnswerManagerService _answerManagerService;
        private readonly IMarkManagerService _markManagerService;

        public CommentsController(
            ICommentManagerService commentManagerService, 
            IQuestionManagerService questionManagerService, 
            IAnswerManagerService answerManagerService, 
            IMarkManagerService markManagerService)
        {
            _commentManagerService = commentManagerService;
            _questionManagerService = questionManagerService;
            _answerManagerService = answerManagerService;
            _markManagerService = markManagerService;
        }

        [HttpGet("{questionId}/comments")]
        [HttpGet("{questionId}/answers/{answerId}/comments")]
        public async Task<IActionResult> GetComments(int questionId, int? answerId)
        {
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }

            if (answerId != null)
            {
                var answer = await _answerManagerService.GetAnswerAsync(answerId.Value);
                if (answer == null)
                {
                    return NotFound(new {Message = $"Question with id {questionId} not found."});
                }
            }
            
            IEnumerable<CommentDto> list;
            if (answerId == null)
                list = await _commentManagerService.GetCommentsAsync(questionId, null);
            else
                list = await _commentManagerService.GetCommentsAsync(null, answerId);
            return Ok(list);
        }

        [HttpPost("{questionId}/comments")]
        [HttpPost("{questionId}/answers/{answerId}/comments")]
        [Authorize]
        public async Task<IActionResult> CreateComment(CommentDto commentDto, int questionId, int? answerId)
        {
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }
            
            if (answerId == null)
            {
                await _commentManagerService.CreateCommentAsync(commentDto, questionId, null);
            }
            else
            {
                var answer = await _answerManagerService.GetAnswerAsync(answerId.Value);
                if (answer == null)
                {
                    return NotFound(new {Message = $"Question with id {questionId} not found."});
                }
                
                await _commentManagerService.CreateCommentAsync(commentDto, null, answerId);
            }
            return Ok();
        }


        [HttpPut("{questionId}/comments/{commentId}")]
        [HttpPut("{questionId}/answers/{answerId}/comments/{commentId}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(int questionId, int? answerId, int commentId, CommentDto commentDto)
        {
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }

            if (answerId != null)
            {
                var answer = await _answerManagerService.GetAnswerAsync(answerId.Value);
                if (answer == null)
                {
                    return NotFound(new {Message = $"Answer with id {answerId} not found."});
                }
            }
            
            var comment = await _commentManagerService.GetCommentAsync(questionId, answerId);
            if (comment == null)
            {
                return NotFound(new {Message = $"Question with id {commentId} not found."});
            }
            
            await _commentManagerService.UpdateCommentAsync(commentDto);
            return Ok();
        }

        [HttpDelete("{questionId}/comments/{commentId}")]
        [HttpDelete("{questionId}/answers/{answerId}/comments/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int questionId, int? answerId, int commentId)
        {
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }

            if (answerId != null)
            {
                var answer = await _answerManagerService.GetAnswerAsync(answerId.Value);
                if (answer == null)
                {
                    return NotFound(new {Message = $"Answer with id {answerId} not found."});
                }
            }
            
            var comment = await _commentManagerService.GetCommentAsync(questionId, answerId);
            if (comment == null)
            {
                return NotFound(new {Message = $"Question with id {commentId} not found."});
            }
            
            await _commentManagerService.DeleteCommentAsync(commentId);
            return Ok();
        }

        [HttpPut("{questionId}/comments/{commentId}/upvote")]
        [HttpPut("{questionId}/answers/{answerId}/comments/{commentId}/upvote")]
        [Authorize]
        public async Task<IActionResult> UpvoteForComment(int questionId, int? answerId, int commentId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid)!.Value);
            
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }

            if (answerId != null)
            {
                var answer = await _answerManagerService.GetAnswerAsync(answerId.Value);
                if (answer == null)
                {
                    return NotFound(new {Message = $"Answer with id {answerId} not found."});
                }
            }
            
            var comment = await _commentManagerService.GetCommentAsync(questionId, answerId);
            if (comment == null)
            {
                return NotFound(new {Message = $"Question with id {commentId} not found."});
            }
            
            var currentMark = await _markManagerService.GetCommentMarkAsync(userId, questionId);
            if (currentMark != null &&  currentMark.MarkValue == -1)
            {
                return Conflict(new { Message = "You can't set the same mark again." });
            }

            await _commentManagerService.MarkCommentAsync(userId, commentId, 1);
            return Ok();
        }

        [HttpPut("{questionId}/comments/{commentId}/downvote")]
        [HttpPut("{questionId}/answers/{answerId}/comments/{commentId}/downvote")]
        [Authorize]
        public async Task<IActionResult> DownvoteForComment(int questionId, int? answerId, int commentId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid)!.Value);
            
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }

            if (answerId != null)
            {
                var answer = await _answerManagerService.GetAnswerAsync(answerId.Value);
                if (answer == null)
                {
                    return NotFound(new {Message = $"Answer with id {answerId} not found."});
                }
            }
            
            var comment = await _commentManagerService.GetCommentAsync(questionId, answerId);
            if (comment == null)
            {
                return NotFound(new {Message = $"Question with id {commentId} not found."});
            }
            
            var currentMark = await _markManagerService.GetCommentMarkAsync(userId, questionId);
            if (currentMark != null &&  currentMark.MarkValue == -1)
            {
                return Conflict(new { Message = "You can't set the same mark again." });
            }
            
            await _commentManagerService.MarkCommentAsync(userId, commentId, -1);
            return Ok();
        }
    }
}