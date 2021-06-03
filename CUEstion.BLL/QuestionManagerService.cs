using CUEstion.DAL.EF;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CUEstion.DAL.Entities;
using System.Collections.Generic;
using CUEstion.BLL.ModelsDTO;

namespace CUEstion.BLL
{
	public class QuestionManagerService
	{
		
		
		public IEnumerable<QuestionDTO> GetAllQuestions()
		{
			using var context = new ApplicationContext();
			var questions = context.Questions.ToList();
			var questionsList = new List<QuestionDTO>();
			foreach (var question in questions)
			{
				questionsList.Add(new QuestionDTO(question));
			}
			return questionsList;
		}

		public QuestionDTO GetQuestion(int questionId)
		{
			using var context = new ApplicationContext();
			return new QuestionDTO(context.Questions.Find(questionId));
		}

		public IEnumerable<QuestionDTO> FilterQuestions(string[] tags)
		{
			using var context = new ApplicationContext();
			var questions = context.Questions.ToList();
			foreach (var tag in tags)
			{
				questions = (from question in questions 
							where (from tag in question.Tags select tag.Name).Contains(tag) 
							select question).ToList();
			}

			var questionsList = new List<QuestionDTO>();
			foreach (var question in questions)
			{
				questionsList.Add(new QuestionDTO(question));
			}
			return questionsList;
		}

		public IEnumerable<QuestionDTO> GetUsersQuestions(int userId)
		{
			using var context = new ApplicationContext();
			var questions = from question in context.Questions where question.UserId == userId select question;
			var questionsList = new List<QuestionDTO>();
			foreach (var question in questions)
			{
				questionsList.Add(new QuestionDTO(question));
			}
			return questionsList;
		}

		public void CreateQuestion(QuestionDTO questionDto)
		{
			using var context = new ApplicationContext();

			var question = new Question()
			{
				Header = questionDto.Header,
				Text = questionDto.Text,
				UserId = questionDto.User.Id
			};

			context.Questions.Add(question);

			context.SaveChanges();
		}

		public void UpdateQuestion(QuestionDTO questionDTO)
		{
			using var context = new ApplicationContext();

			var question = context.Questions.Find(questionDTO.Id);

			if (questionDTO.Text != null) question.Text = questionDTO.Text;
			if (questionDTO.Header != null) question.Header = questionDTO.Header;

			context.SaveChanges();
		}

		public void DeleteQuestion(int questionId)
		{
			using var context = new ApplicationContext();

			var question = context.Questions.Find(questionId);

			foreach (var answer in context.Answers.Where(a => a.QuestionId == questionId))
			{
				foreach (var comment in context.Comments.Where(c => c.AnswerId == answer.Id))
					context.Comments.Remove(comment);
				context.Answers.Remove(answer);
			}

			foreach (var comment in context.Comments.Where(c => c.QuestionId == questionId))
			{
				context.Comments.Remove(comment);
			}

			context.Questions.Remove(question);

			context.SaveChanges();
		}

		public void SubscribeToQuestion(int questionId, int userId)
		{
			using var context = new ApplicationContext();

			var followedQuestion = new FollowedQuestion()
			{
				UserId = userId,
				QuestionId = questionId
			};

			context.FollowedQuestions.Add(followedQuestion);

			context.SaveChanges();
		}

		public IEnumerable<AnswerDTO> GetAnswers(int questionId)
		{
			using var context = new ApplicationContext();

			var answers = from answer in context.Answers where answer.QuestionId == questionId select answer;

			var answersList = new List<AnswerDTO>();
			foreach (var answer in answers)
			{
				answersList.Add(new AnswerDTO(answer));
			}

			return answersList;
		}

		public void CreateAnswer(AnswerDTO answerDto, int questionId)
		{
			using var context = new ApplicationContext();

			var answer = new Answer()
			{
				Text = answerDto.Text,
				QuestionId = questionId,
				UserId = answerDto.User.Id
			};

			context.Answers.Add(answer);

			context.SaveChanges();
		}

		public void UpdateAnswer(AnswerDTO answerDto, int answerId)
		{
			using var context = new ApplicationContext();

			var answer = context.Answers.Find(answerId);

			if (answerDto.Text != null) answer.Text = answerDto.Text;

			context.SaveChanges();
		}

		public void DeleteAnswer(int answerId)
		{
			using var context = new ApplicationContext();

			var answer = context.Answers.Find(answerId);

			foreach (var comment in context.Comments.Where(c => c.AnswerId == answerId))
			{
				context.Comments.Remove(comment);
			}

			context.Answers.Remove(answer);

			context.SaveChanges();
		}


		public IEnumerable<CommentDTO> GetComments(int? questionId, int? answerId)
		{
			using var context = new ApplicationContext();

			var comments = (from comment in context.Comments
						   where comment.QuestionId == questionId
						   where comment.AnswerId == answerId
						   select comment).ToList();

			var commentsList = new List<CommentDTO>();

			foreach (var comment in comments)
			{
				commentsList.Add(new CommentDTO(comment));
			}

			return commentsList;
		}

		public void CreateComment(CommentDTO commentDto, int? questionId, int? answerId)
		{
			using var context = new ApplicationContext();

			//

			var comment = new Comment()
			{
				CommentType = (answerId == null),
				Text = commentDto.Text,
				QuestionId = questionId,
				AnswerId = answerId,
				UserId = commentDto.User.Id
			};

			context.Comments.Add(comment);

			context.SaveChanges();
		}

		public void UpdateComment(CommentDTO commentDto, int commentId)
		{
			using var context = new ApplicationContext();

			var comment = context.Comments.Find(commentId);

			if (commentDto.Text != null) comment.Text = commentDto.Text;

			context.SaveChanges();
		}

		public void DeleteComment(int commentId)
		{
			using var context = new ApplicationContext();

			var comment = context.Comments.Find(commentId);

			context.Comments.Remove(comment);

			context.SaveChanges();
		}
	}
}
