﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WEB.Controllers
{
    [ApiController]
    [Route("Questions/{questionId}")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentManagerService _commentManagerService;
        private readonly IQuestionManagerService _questionManagerService;
        private readonly IAnswerManagerService _answerManagerService;
        private readonly IMarkManagerService _markManagerService;
        private readonly IWorkspaceRoleManagerService _workspaceRoleManagerService;

        public CommentsController(
            ICommentManagerService commentManagerService, 
            IQuestionManagerService questionManagerService, 
            IAnswerManagerService answerManagerService, 
            IMarkManagerService markManagerService, 
            IWorkspaceRoleManagerService workspaceRoleManagerService)
        {
            _commentManagerService = commentManagerService;
            _questionManagerService = questionManagerService;
            _answerManagerService = answerManagerService;
            _markManagerService = markManagerService;
            _workspaceRoleManagerService = workspaceRoleManagerService;
        }

        [HttpGet("comments")]
        [HttpGet("answers/{answerId}/comments")]
        public async Task<IActionResult> GetComments(int questionId, int? answerId)
        {
            var userId = Tools.GetUserIdFromToken(User);
            
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }
            
            if (question.WorkspaceId != null)
            {
                if (userId == null || 
                    !await _workspaceRoleManagerService.CheckUserAccess(
                        userId.Value, 
                        question.WorkspaceId.Value))
                {
                    return Forbid();
                }
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

        [HttpPost("comments")]
        [HttpPost("answers/{answerId}/comments")]
        [Authorize]
        public async Task<IActionResult> CreateComment(CommentDto commentDto, int questionId, int? answerId)
        {
            var userId = Tools.GetUserIdFromToken(User);
            
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }
            
            if (question.WorkspaceId != null)
            {
                if (userId == null || 
                    !await _workspaceRoleManagerService.CheckUserAccess(
                        userId.Value, 
                        question.WorkspaceId.Value, 
                        AccessRights.CanCreate))
                {
                    return Forbid();
                }
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


        [HttpPut("comments/{commentId}")]
        [HttpPut("answers/{answerId}/comments/{commentId}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(int questionId, int? answerId, int commentId, CommentDto commentDto)
        {
            var userId = Tools.GetUserIdFromToken(User);
            
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
            
            var comment = await _commentManagerService.GetCommentAsync(commentId);
            if (comment == null)
            {
                return NotFound(new {Message = $"Question with id {commentId} not found."});
            }
            
            if (question.WorkspaceId != null)
            {
                if (userId == null || 
                    !await _workspaceRoleManagerService.CheckUserAccess(
                        userId.Value, 
                        question.WorkspaceId.Value,
                        AccessRights.CanUpdate))
                {
                    return Forbid();
                }
            }
            else
            {
                if (userId == null
                    || userId != comment.User.Id
                    && Tools.GetSystemRoleFromToken(User) != SystemRoles.Admin)
                {
                    return Forbid();
                }
            }

            await _commentManagerService.UpdateCommentAsync(commentDto);
            return Ok();
        }

        [HttpDelete("comments/{commentId}")]
        [HttpDelete("answers/{answerId}/comments/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int questionId, int? answerId, int commentId)
        {
            var userId = Tools.GetUserIdFromToken(User);
            
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
            
            var comment = await _commentManagerService.GetCommentAsync(commentId);
            if (comment == null)
            {
                return NotFound(new {Message = $"Question with id {commentId} not found."});
            }
            
            if (question.WorkspaceId != null)
            {
                if (userId == null || 
                    !await _workspaceRoleManagerService.CheckUserAccess(
                        userId.Value, 
                        question.WorkspaceId.Value,
                        AccessRights.CanDelete))
                {
                    return Forbid();
                }
            }
            else
            {
                if (userId == null
                    || userId != comment.User.Id
                    && Tools.GetSystemRoleFromToken(User) != SystemRoles.Admin)
                {
                    return Forbid();
                }
            }


            await _commentManagerService.DeleteCommentAsync(commentId);
            return Ok();
        }

        [HttpPut("comments/{commentId}/upvote")]
        [HttpPut("answers/{answerId}/comments/{commentId}/upvote")]
        [Authorize]
        public async Task<IActionResult> UpvoteForComment(int questionId, int? answerId, int commentId)
        {
            var userId = Tools.GetUserIdFromToken(User);
            
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }
            
            if (question.WorkspaceId != null)
            {
                if (userId == null || 
                    !await _workspaceRoleManagerService.CheckUserAccess(
                        userId.Value, 
                        question.WorkspaceId.Value,
                        AccessRights.CanCreate))
                {
                    return Forbid();
                }
            }

            if (answerId != null)
            {
                var answer = await _answerManagerService.GetAnswerAsync(answerId.Value);
                if (answer == null)
                {
                    return NotFound(new {Message = $"Answer with id {answerId} not found."});
                }
            }
            
            var comment = await _commentManagerService.GetCommentAsync(commentId);
            if (comment == null)
            {
                return NotFound(new {Message = $"Question with id {commentId} not found."});
            }
            
            var currentMark = await _markManagerService.GetCommentMarkAsync(userId.Value, questionId);
            if (currentMark != null &&  currentMark.MarkValue == -1)
            {
                return Conflict(new { Message = "You can't set the same mark again." });
            }

            await _commentManagerService.MarkCommentAsync(userId.Value, commentId, 1);
            return Ok();
        }

        [HttpPut("comments/{commentId}/downvote")]
        [HttpPut("answers/{answerId}/comments/{commentId}/downvote")]
        [Authorize]
        public async Task<IActionResult> DownvoteForComment(int questionId, int? answerId, int commentId)
        {
            var userId = Tools.GetUserIdFromToken(User);
            
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }
            
            if (question.WorkspaceId != null)
            {
                if (userId == null || 
                    !await _workspaceRoleManagerService.CheckUserAccess(
                        userId.Value, 
                        question.WorkspaceId.Value,
                        AccessRights.CanCreate))
                {
                    return Forbid();
                }
            }

            if (answerId != null)
            {
                var answer = await _answerManagerService.GetAnswerAsync(answerId.Value);
                if (answer == null)
                {
                    return NotFound(new {Message = $"Answer with id {answerId} not found."});
                }
            }
            
            var comment = await _commentManagerService.GetCommentAsync(commentId);
            if (comment == null)
            {
                return NotFound(new {Message = $"Question with id {commentId} not found."});
            }
            
            var currentMark = await _markManagerService.GetCommentMarkAsync(userId.Value, questionId);
            if (currentMark != null &&  currentMark.MarkValue == -1)
            {
                return Conflict(new { Message = "You can't set the same mark again." });
            }
            
            await _commentManagerService.MarkCommentAsync(userId.Value, commentId, -1);
            return Ok();
        }
    }
}