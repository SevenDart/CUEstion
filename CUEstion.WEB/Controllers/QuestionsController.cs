﻿using System;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CUEstion.BLL.Interfaces;
using CUEstion.BLL.ModelsDTO;

namespace CUEstion.WEB.Controllers
{
	[ApiController]
	[Route("questions")]
	public class QuestionsController : ControllerBase
	{

		private readonly IQuestionManagerService _questionManagerService;

		public QuestionsController(IQuestionManagerService questionManagerService)
		{
			_questionManagerService = questionManagerService;
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

		[HttpGet("tags")]
		public IActionResult GetAllTags()
		{
			try
			{
				var list = _questionManagerService.GetAllTags();
				return Ok(list);
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpGet("Hot")]
		public IActionResult GetHotQuestions(int count)
		{
			try
			{
				var list = _questionManagerService.GetNewestQuestions(count);
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
				for (int i = 0; i < tags.Length; i++)
				{
					tags[i] = Uri.UnescapeDataString(tags[i]);
				}
				var list = _questionManagerService.FilterQuestions(tags);
				return Ok(list);
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpGet("search")]
		public IActionResult Search(string query, [FromQuery]string[] tags)
		{
			try
			{
				if (query == null) query = "";
				for (int i = 0; i < tags.Length; i++)
				{
					tags[i] = Uri.UnescapeDataString(tags[i]);
				}
				var list = _questionManagerService.Search(query, tags);
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

		[HttpPost]
		[Authorize]
		public IActionResult CreateQuestion(QuestionDTO questionDto)
		{
			try
			{
				_questionManagerService.CreateQuestion(questionDto);
				return Ok(questionDto.Id);
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

		[HttpPut("{questionId}")]
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

		[HttpGet("{questionId}/unsubscribe")]
		[Authorize]
		public IActionResult UnsubscribeFromQuestion(int questionId)
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
				_questionManagerService.UnsubscribeFromQuestion(questionId, userId);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpGet("{questionId}/is-subscribed")]
		[Authorize]
		public IActionResult IsSubscribedToQuestion(int questionId)
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
				return Ok(_questionManagerService.IsSubscribedToQuestion(questionId, userId));
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

		[HttpPost("{questionId}/answers")]
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

		[HttpPut("{questionId}/answers/{answerId}")]
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

		[HttpPost("{questionId}/comments")]
		[HttpPost("{questionId}/answers/{answerId}/comments")]
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


		[HttpPut("{questionId}/comments/{commentId}")]
		[HttpPut("{questionId}/answers/{answerId}/comments/{commentId}")]
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



		[HttpPut("{questionId}/upvote")]
		[Authorize]
		public IActionResult UpvoteForQuestion(int questionId)
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
				_questionManagerService.MarkQuestion(userId, questionId, 1);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}/downvote")]
		[Authorize]
		public IActionResult DownvoteForQuestion(int questionId)
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
				_questionManagerService.MarkQuestion(userId, questionId, -1);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}/answers/{answerId}/upvote")]
		[Authorize]
		public IActionResult UpvoteForAnswer(int answerId)
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
				_questionManagerService.MarkAnswer(userId, answerId, 1);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}/answers/{answerId}/downvote")]
		[Authorize]
		public IActionResult DownvoteForAnswer(int answerId)
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
				_questionManagerService.MarkAnswer(userId, answerId, -1);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}



		[HttpPut("{questionId}/comments/{commentId}/upvote")]
		[HttpPut("{questionId}/answers/{answerId}/comments/{commentId}/upvote")]
		[Authorize]
		public IActionResult UpvoteForComment(int commentId)
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
				_questionManagerService.MarkComment(userId, commentId, 1);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}/comments/{commentId}/downvote")]
		[HttpPut("{questionId}/answers/{answerId}/comments/{commentId}/downvote")]
		[Authorize]
		public IActionResult DownvoteForComment(int commentId)
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
				_questionManagerService.MarkComment(userId, commentId, -1);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPost("tags")]
		[Authorize(Roles = "admin")]
		public IActionResult CreateTag([FromQuery] string tag)
		{
			try
			{
				tag = Uri.UnescapeDataString(tag);
				_questionManagerService.CreateTag(tag);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("tags")]
		[Authorize(Roles = "admin")]
		public IActionResult UpdateTag([FromQuery] string oldTag, string newTag)
		{
			try
			{
				oldTag = Uri.UnescapeDataString(oldTag);
				newTag = Uri.UnescapeDataString(newTag);
				_questionManagerService.UpdateTag(oldTag, newTag);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpDelete("tags")]
		[Authorize(Roles = "admin")]
		public IActionResult DeleteTag([FromQuery] string tag)
		{
			try
			{
				tag = Uri.UnescapeDataString(tag);
				_questionManagerService.DeleteTag(tag);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}
	}
}
