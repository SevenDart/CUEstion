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
    [Route("questions")]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswerManagerService _answerManagerService;
        private readonly IQuestionManagerService _questionManagerService;
        private readonly IMarkManagerService _markManagerService;

        public AnswersController(
            IAnswerManagerService answerManagerService, 
            IQuestionManagerService questionManagerService, 
            IMarkManagerService markManagerService)
        {
            _answerManagerService = answerManagerService;
            _questionManagerService = questionManagerService;
            _markManagerService = markManagerService;
        }

        [HttpGet("{questionId}/answers")]
        public async Task<IActionResult> GetAnswers(int questionId)
        {
            var list = await _answerManagerService.GetAnswersAsync(questionId);
            return Ok(list);
        }

        [HttpPost("{questionId}/answers")]
        [Authorize]
        public async Task<IActionResult> CreateAnswer(AnswerDto answerDto, int questionId)
        {
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }

            await _answerManagerService.CreateAnswerAsync(answerDto, questionId);
            return Ok();
        }

        [HttpPut("{questionId}/answers/{answerId}")]
        [Authorize]
        public async Task<IActionResult> UpdateAnswer(int questionId, int answerId, AnswerDto answerDto)
        {
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
            
            await _answerManagerService.UpdateAnswer(answerDto);
            return Ok();
        }

        [HttpDelete("{questionId}/answers/{answerId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAnswer(int answerId, int questionId)
        {
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
            
            await _answerManagerService.DeleteAnswer(answerId);
            return Ok();
        }


        [HttpPut("{questionId}/answers/{answerId}/upvote")]
        [Authorize]
        public async Task<IActionResult> UpvoteForAnswer(int questionId, int answerId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Sid)!.Value);
            
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
            
            var currentMark = await _markManagerService.GetQuestionMarkAsync(userId, questionId);
            if (currentMark != null && currentMark.MarkValue == 1)
            {
                return Conflict(new { Message = "You can't set the same mark again." });
            }
            
            await _answerManagerService.MarkAnswerAsync(userId, answerId, 1);
            return Ok();
        }

        [HttpPut("{questionId}/answers/{answerId}/downvote")]
        [Authorize]
        public async Task<IActionResult> DownvoteForAnswer(int questionId, int answerId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Sid)!.Value);
            
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

            var currentMark = await _markManagerService.GetQuestionMarkAsync(userId, questionId);
            if (currentMark != null && currentMark.MarkValue == -1)
            {
                return Conflict(new { Message = "You can't set the same mark again." });
            }
            
            await _answerManagerService.MarkAnswerAsync(userId, answerId, -1);
            return Ok();
        }
    }
}