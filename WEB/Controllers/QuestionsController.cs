using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BLL.Interfaces;
using BLL.ModelsDTO;

namespace WEB.Controllers
{
    [ApiController]
    [Route("Questions")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionManagerService _questionManagerService;
        private readonly IMarkManagerService _markManagerService;
        private readonly IWorkspaceRoleManagerService _workspaceRoleManagerService;

        public QuestionsController(IQuestionManagerService questionManagerService, IMarkManagerService markManagerService, IWorkspaceRoleManagerService workspaceRoleManagerService)
        {
            _questionManagerService = questionManagerService;
            _markManagerService = markManagerService;
            _workspaceRoleManagerService = workspaceRoleManagerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuestions([FromQuery] int? workspaceId)
        {
            if (workspaceId != null)
            {
                var accessingUser = Tools.GetUserIdFromToken(User);
                if (accessingUser == null || 
                    !await _workspaceRoleManagerService.CheckUserAccess(accessingUser.Value, workspaceId.Value))
                {
                    return Forbid();
                }
            }
            
            var list = await _questionManagerService.GetAllQuestionsAsync(workspaceId);
            return Ok(list);
        }

        [HttpGet("Hot")]
        public async Task<IActionResult> GetHotQuestions([FromQuery] int count, [FromQuery] int? workspaceId)
        {
            if (workspaceId != null)
            {
                var accessingUser = Tools.GetUserIdFromToken(User);
                if (accessingUser == null || 
                    !await _workspaceRoleManagerService.CheckUserAccess(accessingUser.Value, workspaceId.Value))
                {
                    return Forbid();
                }
            }
            
            var list = await _questionManagerService.GetNewestQuestionsAsync(count, workspaceId);
            return Ok(list);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string query, 
            [FromQuery] string[] tags, 
            [FromQuery] int? workspaceId)
        {
            query ??= "";
            for (int i = 0; i < tags.Length; i++)
            {
                tags[i] = Uri.UnescapeDataString(tags[i]);
            }
            
            if (workspaceId != null)
            {
                var accessingUser = Tools.GetUserIdFromToken(User);
                if (accessingUser == null || 
                    !await _workspaceRoleManagerService.CheckUserAccess(accessingUser.Value, workspaceId.Value))
                {
                    return Forbid();
                }
            }

            var list = await _questionManagerService.Search(query, tags, workspaceId);
            return Ok(list);
        }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetQuestion(int questionId)
        {
            var question = await _questionManagerService.GetQuestionAsync(questionId);

            if (question == null)
            {
                return NotFound($"Question with id {questionId} not found.");
            }
            
            if (question.WorkspaceId != null)
            {
                var accessingUser = Tools.GetUserIdFromToken(User);
                if (accessingUser == null || 
                    !await _workspaceRoleManagerService.CheckUserAccess(
                        accessingUser.Value, 
                        question.WorkspaceId.Value))
                {
                    return Forbid();
                }
            }

            return Ok(question);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateQuestion(QuestionDto questionDto)
        {
            if (questionDto.WorkspaceId != null)
            {
                var accessingUser = Tools.GetUserIdFromToken(User);
                if (accessingUser == null || 
                    !await _workspaceRoleManagerService.CheckUserAccess(
                        accessingUser.Value, 
                        questionDto.WorkspaceId.Value, 
                        AccessRights.CanCreate))
                {
                    return Forbid();
                }
            }
            
            await _questionManagerService.CreateQuestion(questionDto);
            return Ok(questionDto);
        }


        [HttpDelete("{questionId}")]
        [Authorize]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            var accessingUserId = Tools.GetUserIdFromToken(User);
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }
            
            if (question.WorkspaceId != null)
            {
                if (accessingUserId == null || 
                    !await _workspaceRoleManagerService.CheckUserAccess(
                        accessingUserId.Value, 
                        question.WorkspaceId.Value, 
                        AccessRights.CanDelete))
                {
                    return Forbid();
                }
            }
            else
            {
                if (accessingUserId == null
                    || accessingUserId != question.User.Id
                    || Tools.GetSystemRoleFromToken(User) != SystemRoles.Admin)
                {
                    return Forbid();
                }
            }
            
            await _questionManagerService.DeleteQuestion(questionId);
            return Ok();
        }

        [HttpPut("{questionId}")]
        [Authorize]
        public async Task<IActionResult> UpdateQuestion(int questionId, QuestionDto questionDto)
        {
            var accessingUserId = Tools.GetUserIdFromToken(User);
            
            questionDto.Id = questionId;
            var question = await _questionManagerService.GetQuestionAsync(questionDto.Id);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionDto.Id} not found."});
            }
            
            if (questionDto.WorkspaceId != null)
            {
                if (accessingUserId == null 
                    || !await _workspaceRoleManagerService.CheckUserAccess(
                        accessingUserId.Value, 
                        questionDto.WorkspaceId.Value, 
                        AccessRights.CanUpdate))
                {
                    return Forbid();
                }
            }
            else
            {
                if (accessingUserId == null
                    || accessingUserId != question.User.Id
                    || Tools.GetSystemRoleFromToken(User) != SystemRoles.Admin)
                {
                    return Forbid();
                }
            }

            await _questionManagerService.UpdateQuestion(questionDto);
            return Ok(questionDto);
        }


        [HttpPut("{questionId}/upvote")]
        [Authorize]
        public async Task<IActionResult> UpvoteForQuestion(int questionId)
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

            var currentMark = await _markManagerService.GetQuestionMarkAsync(userId.Value, questionId);
            if (currentMark != null && currentMark.MarkValue == 1)
            {
                return Conflict(new { Message = "You can't set the same mark again." });
            }
            
            await _questionManagerService.MarkQuestion(userId.Value, questionId, 1);

            return Ok();
        }

        [HttpPut("{questionId}/downvote")]
        [Authorize]
        public async Task<IActionResult> DownvoteForQuestion(int questionId)
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
            
            var currentMark = await _markManagerService.GetQuestionMarkAsync(userId.Value, questionId);
            if (currentMark != null &&  currentMark.MarkValue == -1)
            {
                return Conflict(new { Message = "You can't set the same mark again." });
            }
            
            await _questionManagerService.MarkQuestion(userId.Value, questionId, -1);

            return Ok();
        }
    }
}