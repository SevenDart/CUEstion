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
				questions = context.Questions.Where(q => q.Tags.Select(t => t.Name).Contains(tag)).ToList();
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
			var questions = context.Questions.Where(q => q.UserId == userId);
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
				CreateTime = DateTime.Now,
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

			question.UpdateTime = DateTime.Now;

			context.SaveChanges();
		}

		public void DeleteQuestion(int questionId)
		{
			using var context = new ApplicationContext();

			var question = context.Questions.Find(questionId);

			var questionAnswers = context.Answers.Where(a => a.QuestionId == questionId);
			foreach (var answer in questionAnswers)
			{
				var answerComments = context.Comments.Where(c => c.AnswerId == answer.Id);
				foreach (var comment in answerComments)
					context.Comments.Remove(comment);
				context.Answers.Remove(answer);
			}

			var questionComments = context.Comments.Where(c => c.QuestionId == questionId);
			foreach (var comment in questionComments)
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

			var answers = context.Answers.Where(a => a.QuestionId == questionId);

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
				CreateTime = DateTime.Now,
				QuestionId = questionId,
				UserId = answerDto.User.Id
			};

			context.Answers.Add(answer);

			context.SaveChanges();
		}

		public void UpdateAnswer(AnswerDTO answerDto)
		{
			using var context = new ApplicationContext();

			var answer = context.Answers.Find(answerDto.Id);

			if (answerDto.Text != null) answer.Text = answerDto.Text;

			answer.UpdateTime = DateTime.Now;

			context.SaveChanges();
		}

		public void DeleteAnswer(int answerId)
		{
			using var context = new ApplicationContext();

			var answer = context.Answers.Find(answerId);

			var answerComments = context.Comments.Where(c => c.AnswerId == answerId);
			foreach (var comment in answerComments)
			{
				context.Comments.Remove(comment);
			}

			context.Answers.Remove(answer);

			context.SaveChanges();
		}


		public IEnumerable<CommentDTO> GetComments(int? questionId, int? answerId)
		{
			using var context = new ApplicationContext();

			var comments = context.Comments.Where(c => c.QuestionId == questionId && c.AnswerId == answerId);

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

			var comment = new Comment()
			{
				Text = commentDto.Text,
				CreateTime = DateTime.Now,
				QuestionId = questionId,
				AnswerId = answerId,
				UserId = commentDto.User.Id
			};

			context.Comments.Add(comment);

			context.SaveChanges();
		}

		public void UpdateComment(CommentDTO commentDto)
		{
			using var context = new ApplicationContext();

			var comment = context.Comments.Find(commentDto.Id);

			if (commentDto.Text != null) comment.Text = commentDto.Text;

			comment.UpdateTime = DateTime.Now;

			context.SaveChanges();
		}

		public void DeleteComment(int commentId)
		{
			using var context = new ApplicationContext();

			var comment = context.Comments.Find(commentId);

			context.Comments.Remove(comment);

			context.SaveChanges();
		}

		public void MarkQuestion(int questionId, int mark)
		{
			using var context = new ApplicationContext();

			var question = context.Questions.Find(questionId);
			var user = context.Users.Find(question.UserId);
			question.Rate += mark;
			user.Rate += mark;

			context.SaveChanges();
		}

		public void MarkAnswer(int answerId, int mark)
		{
			using var context = new ApplicationContext();

			var answer = context.Answers.Find(answerId);
			var user = context.Users.Find(answer.UserId);
			answer.Rate += mark;
			user.Rate += mark;

			context.SaveChanges();
		}

		public void MarkComment(int commentId, int mark)
		{
			using var context = new ApplicationContext();

			var comment = context.Comments.Find(commentId);
			var user = context.Users.Find(comment.UserId);
			comment.Rate += mark;
			user.Rate += mark;

			context.SaveChanges();
		}
	}
}
