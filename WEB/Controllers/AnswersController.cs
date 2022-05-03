using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WEB.Controllers
{
    [ApiController]
    [Route("Questions/{questionId}/Answers")]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswerManagerService _answerManagerService;
        private readonly IQuestionManagerService _questionManagerService;
        private readonly IMarkManagerService _markManagerService;
        private readonly IWorkspaceRoleManagerService _workspaceRoleManagerService;

        public AnswersController(
            IAnswerManagerService answerManagerService, 
            IQuestionManagerService questionManagerService, 
            IMarkManagerService markManagerService, 
            IWorkspaceRoleManagerService workspaceRoleManagerService)
        {
            _answerManagerService = answerManagerService;
            _questionManagerService = questionManagerService;
            _markManagerService = markManagerService;
            _workspaceRoleManagerService = workspaceRoleManagerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnswers(int questionId)
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
            
            var list = await _answerManagerService.GetAnswersAsync(questionId);
            return Ok(list);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAnswer(AnswerDto answerDto, int questionId)
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

            await _answerManagerService.CreateAnswerAsync(answerDto, questionId);
            return Ok();
        }

        [HttpPut("{answerId}")]
        [Authorize]
        public async Task<IActionResult> UpdateAnswer(int questionId, int answerId, AnswerDto answerDto)
        {
            var userId = Tools.GetUserIdFromToken(User);
            
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }
            
            var answer = await _answerManagerService.GetAnswerAsync(answerId);
            if (answer == null)
            {
                return NotFound(new {Message = $"Answer with id {answerId} not found."});
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
                    || userId != answer.User.Id
                    && Tools.GetSystemRoleFromToken(User) != SystemRoles.Admin)
                {
                    return Forbid();
                }
            }
            
            
            
            await _answerManagerService.UpdateAnswer(answerDto);
            return Ok();
        }

        [HttpDelete("{answerId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAnswer(int answerId, int questionId)
        {
            var userId = Tools.GetUserIdFromToken(User);
            
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }
            
            var answer = await _answerManagerService.GetAnswerAsync(answerId);
            if (answer == null)
            {
                return NotFound(new {Message = $"Answer with id {answerId} not found."});
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
                    || userId != answer.User.Id
                    && Tools.GetSystemRoleFromToken(User) != SystemRoles.Admin)
                {
                    return Forbid();
                }
            }


            await _answerManagerService.DeleteAnswer(answerId);
            return Ok();
        }


        [HttpPut("{answerId}/upvote")]
        [Authorize]
        public async Task<IActionResult> UpvoteForAnswer(int questionId, int answerId)
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
            
            var answer = await _answerManagerService.GetAnswerAsync(answerId);
            if (answer == null)
            {
                return NotFound(new {Message = $"Answer with id {answerId} not found."});
            }
            
            var currentMark = await _markManagerService.GetQuestionMarkAsync(userId.Value, questionId);
            if (currentMark != null && currentMark.MarkValue == 1)
            {
                return Conflict(new { Message = "You can't set the same mark again." });
            }
            
            await _answerManagerService.MarkAnswerAsync(userId.Value, answerId, 1);
            return Ok();
        }

        [HttpPut("{answerId}/downvote")]
        [Authorize]
        public async Task<IActionResult> DownvoteForAnswer(int questionId, int answerId)
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
            
            var answer = await _answerManagerService.GetAnswerAsync(answerId);
            if (answer == null)
            {
                return NotFound(new {Message = $"Answer with id {answerId} not found."});
            }

            var currentMark = await _markManagerService.GetQuestionMarkAsync(userId.Value, questionId);
            if (currentMark != null && currentMark.MarkValue == -1)
            {
                return Conflict(new { Message = "You can't set the same mark again." });
            }
            
            await _answerManagerService.MarkAnswerAsync(userId.Value, answerId, -1);
            return Ok();
        }
    }
}