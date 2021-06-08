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
			var questions = context.Questions.Include(q => q.Tags).ToList();
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
			var question = context.Questions.Include(q => q.Tags).FirstOrDefault(q => q.Id == questionId);
			return new QuestionDTO(question);
		}

		public IEnumerable<QuestionDTO> Search(string query)
		{
			using var context = new ApplicationContext();

			var words = query.Split(" .,:?!");
			var subseqs = new List<string>();
			for (int length = 1; length <= words.Length; length++)
			{
				for (int i = 0; i <= words.Length - length; i++)
				{
					string subseq = words[i];
					for (int j = i + 1; j < i + length; j++)
						subseq += " " + words[j];
					subseqs.Add(subseq);
				}
			}

			var questions = context.Questions.ToList().Where(q => 
				subseqs.Any(s => 
					q.Header.Contains(s, StringComparison.OrdinalIgnoreCase) 
					|| q.Text.Contains(s, StringComparison.OrdinalIgnoreCase)
					)
				);

			var questionsList = new List<QuestionDTO>();
			foreach (var question in questions)
			{
				questionsList.Add(new QuestionDTO(question));
			}
			return questionsList;

		}

		public IEnumerable<QuestionDTO> FilterQuestions(string[] tags)
		{
			using var context = new ApplicationContext();

			var questions = context.Questions.Include(q => q.Tags).ToList()
				.Where(q => 
					tags.Any(t => 
						q.Tags.Select(t => t.Name)
						.Contains(t, StringComparer.OrdinalIgnoreCase))
				).ToList();

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
			var questions = context.Questions.Include(t => t.Tags).Where(q => q.UserId == userId);
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

			question.Tags = new List<Tag>();
			foreach (var tag in questionDto.Tags)
			{
				//rewrite case-insensitive
				if (context.Tags.Select(t => t.Name).Any(t => EF.Functions.Like(t, tag)))
				{
					question.Tags.Add(context.Tags.First(t => StringComparer.OrdinalIgnoreCase.Compare(t.Name, tag) == 0));
				}
				else
				{
					var dbTag = new Tag() { Name = tag };
					context.Tags.Add(dbTag);
					question.Tags.Add(dbTag);
				}
			}

			context.Questions.Add(question);

			context.SaveChanges();
		}

		public void UpdateQuestion(QuestionDTO questionDto)
		{
			using var context = new ApplicationContext();

			var question = context.Questions.Find(questionDto.Id);

			if (questionDto.Text != null) question.Text = questionDto.Text;
			if (questionDto.Header != null) question.Header = questionDto.Header;

			question.UpdateTime = DateTime.Now;

			foreach (var tag in questionDto.Tags)
			{
				//rewrite case-insensitive
				if (context.Tags.Select(t => t.Name).Any(t => EF.Functions.Like(t, tag)))
				{
					question.Tags.Add(context.Tags.First(t => StringComparer.OrdinalIgnoreCase.Compare(t.Name, tag) == 0));
				}
				else
				{
					var dbTag = new Tag() { Name = tag };
					context.Tags.Add(dbTag);
					question.Tags.Add(dbTag);
				}
			}

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
