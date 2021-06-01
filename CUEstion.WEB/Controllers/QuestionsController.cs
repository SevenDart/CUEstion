﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CUEstion.WEB.Controllers
{
	[ApiController]
	[Route("questions")]
	public class QuestionsController : ControllerBase
	{
		[HttpGet]
		public IActionResult GetAllQuestions()
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpGet("filter")]
		public IActionResult GetQuestionsByTags(string[] tags)
		{
			try
			{
				return Ok();
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
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpDelete("{questionId}")]
		public IActionResult DeleteQuestion(int questionId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut]
		[Authorize]
		public IActionResult CreateQuestion()
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPost("{questionId}")]
		[Authorize]
		public IActionResult UpdateQuestion(int questionId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpGet("{questionId}/subscribe")]
		[Authorize]
		public IActionResult SubscribeToQuestion()
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}/answers")]
		[Authorize]
		public IActionResult CreateAnswer(int questionId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPost("{questionId}/answers/{answerId}")]
		[Authorize]
		public IActionResult UpdateAnswer(int questionId, int answerId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpDelete("{questionId}/answers/{answerId}")]
		[Authorize]
		public IActionResult DeleteAnswer(int questionId, int answerId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}/comments")]
		[HttpPut("{questionId}/answers/{answerId}/comments")]
		[Authorize]
		public IActionResult CreateComment(int questionId, int? answerId)
		{
			try
			{
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
		public IActionResult UpdateComment(int questionId, int commentId, int? answerId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}



		[HttpPut("{questionId}")]
		[Authorize]
		public IActionResult VoteForQuestion(int questionId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}/answers/{answerId}")]
		[Authorize]
		public IActionResult VoteForAnswer(int questionId, int answerId)
		{
			try
			{
				int mark = int.Parse(Request.Query["mark"]);
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
		public IActionResult VoteForComment(int questionId, int commentId, int? answerId)
		{
			try
			{	
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpDelete("{questionId}/answers/comments/{commentId}")]
		[HttpDelete("{questionId}/answers/{answerId}/comments/{commentId}")]
		[Authorize]
		public IActionResult DeleteComment(int questionId, int commentId, int? answerId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}
	}
}
