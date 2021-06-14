﻿using System;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CUEstion.BLL;
using CUEstion.BLL.ModelsDTO;

namespace CUEstion.WEB.Controllers
{
	[ApiController]
	[Route("questions")]
	public class QuestionsController : ControllerBase
	{

		private QuestionManagerService _questionManagerService;

		public QuestionsController()
		{
			_questionManagerService = new QuestionManagerService();
		}

		[HttpGet]
		public IActionResult GetAllQuestions()
		{
			try
			{
				var list = _questionManagerService.GetAllQuestions();
				return Ok(list);
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpGet("filter")]
		public IActionResult GetQuestionsByTags([FromQuery] string[] tags)
		{
			try
			{
				var list = _questionManagerService.FilterQuestions(tags);
				return Ok(list);
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpGet("search")]
		public IActionResult Search(string query)
		{
			try
			{
				var list = _questionManagerService.Search(query);
				return Ok(list);
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpGet("{questionId}")]
		public IActionResult GetQuestion(int questionId)
		{
			try
			{
				var question = _questionManagerService.GetQuestion(questionId);
				return question != null
					? Ok(question) 
					: StatusCode(404, new { Message = "No such question." });
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut]
		[Authorize]
		public IActionResult CreateQuestion(QuestionDTO questionDto)
		{
			try
			{
				_questionManagerService.CreateQuestion(questionDto);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpDelete("{questionId}")]
		[Authorize]
		public IActionResult DeleteQuestion(int questionId)
		{
			try
			{
				_questionManagerService.DeleteQuestion(questionId);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPost("{questionId}")]
		[Authorize]
		public IActionResult UpdateQuestion(QuestionDTO questionDto)
		{
			try
			{
				_questionManagerService.UpdateQuestion(questionDto);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpGet("{questionId}/subscribe")]
		[Authorize]
		public IActionResult SubscribeToQuestion(int questionId)
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
				_questionManagerService.SubscribeToQuestion(questionId, userId);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpGet("{questionId}/answers")]
		public IActionResult GetAnswers(int questionId)
		{
			try
			{
				var list = _questionManagerService.GetAnswers(questionId);
				return Ok(list);
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}/answers")]
		[Authorize]
		public IActionResult CreateAnswer(AnswerDTO answerDto, int questionId)
		{
			try
			{
				_questionManagerService.CreateAnswer(answerDto, questionId);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPost("{questionId}/answers/{answerId}")]
		[Authorize]
		public IActionResult UpdateAnswer(AnswerDTO answerDto)
		{
			try
			{
				_questionManagerService.UpdateAnswer(answerDto);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpDelete("{questionId}/answers/{answerId}")]
		[Authorize]
		public IActionResult DeleteAnswer(int answerId)
		{
			try
			{
				_questionManagerService.DeleteAnswer(answerId);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpGet("{questionId}/comments")]
		[HttpGet("{questionId}/answers/{answerId}/comments")]
		[Authorize]
		public IActionResult GetComments(int questionId, int? answerId)
		{
			try
			{
				IEnumerable<CommentDTO> list;
				if (answerId == null)
					list = _questionManagerService.GetComments(questionId, null);
				else
					list = _questionManagerService.GetComments(null, answerId);
				return Ok(list);
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}/comments")]
		[HttpPut("{questionId}/answers/{answerId}/comments")]
		[Authorize]
		public IActionResult CreateComment(CommentDTO commentDto, int questionId, int? answerId)
		{
			try
			{
				if (answerId == null)
					_questionManagerService.CreateComment(commentDto, questionId, null);
				else
					_questionManagerService.CreateComment(commentDto, null, answerId);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpPost("{questionId}/comments/{commentId}")]
		[HttpPost("{questionId}/answers/{answerId}/comments/{commentId}")]
		[Authorize]
		public IActionResult UpdateComment(CommentDTO commentDto)
		{
			try
			{
				_questionManagerService.UpdateComment(commentDto);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}



		[HttpPut("{questionId}")]
		[Authorize]
		public IActionResult VoteForQuestion(int questionId, int mark)
		{
			try
			{
				_questionManagerService.MarkQuestion(questionId, mark);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}/answers/{answerId}")]
		[Authorize]
		public IActionResult VoteForAnswer(int answerId, int mark)
		{
			try
			{
				_questionManagerService.MarkAnswer(answerId, mark);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}



		[HttpPut("{questionId}/comments/{commentId}")]
		[HttpPut("{questionId}/answers/{answerId}/comments/{commentId}")]
		[Authorize]
		public IActionResult VoteForComment(int commentId, int mark)
		{
			try
			{
				_questionManagerService.MarkComment(commentId, mark);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpDelete("{questionId}/comments/{commentId}")]
		[HttpDelete("{questionId}/answers/{answerId}/comments/{commentId}")]
		[Authorize]
		public IActionResult DeleteComment(int commentId)
		{
			try
			{
				_questionManagerService.DeleteComment(commentId);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}
	}
}
