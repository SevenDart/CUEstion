using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CUEstion.DAL.Entities;
using CUEstion.DAL.EF;
using CUEstion.BLL.ModelsDTO;
using Microsoft.EntityFrameworkCore;

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

		public IEnumerable<String> GetAllTags()
		{
			using var context = new ApplicationContext();
			var tags = context.Tags.Select(t => t.Name).ToList();
			return tags;
		}

		public IEnumerable<QuestionDTO> GetNewestQuestions(int count)
		{
			using var context = new ApplicationContext();
			var questions = context.Questions.Include(q => q.Tags)
				.OrderByDescending(q => q.CreateTime)
				.ThenByDescending(q => q.UpdateTime)
				.ThenByDescending(q => q.Rate)
				.Take(count);
			var questionList = new List<QuestionDTO>();
			foreach (var question in questions)
			{
				questionList.Add(new QuestionDTO(question));
			}
			return questionList;
		}

		public QuestionDTO GetQuestion(int questionId)
		{
			using var context = new ApplicationContext();
			var question = context.Questions.Include(q => q.Tags).FirstOrDefault(q => q.Id == questionId);
			question.User = context.Users.Find(question.UserId);
			return question != null 
				? new QuestionDTO(question) 
				: null;
		}

		public IEnumerable<QuestionDTO> Search(string query, string[] tags)
		{
			using var context = new ApplicationContext();

			var words = query.Split(' ', '.', ',', ':', '?', '!');
			var subseqs = new List<string>();

			for (int length = 1; length <= words.Length; length++)
			{
				int wordsCount = 0;
				string subseq = "";
				for (int i = 0; i < words.Length; i++)
				{
					if (wordsCount >= length)
					{
						wordsCount--;
						subseq = subseq.Substring(words[i - length].Length + ((subseq[^1] == ' ') ? 1 : 0));
					}
					if (subseq != "") subseq += " ";
					subseq += words[i];
					wordsCount++;
					if (wordsCount == length) subseqs.Add(subseq);
				}
			}

			var questions = context.Questions.Include(q => q.Tags).ToList()
				.Where(q => tags.All(t => q.Tags.Any(qTag => qTag.Name.ToLower() == t.ToLower()))).ToList();

			questions = questions.Where(q =>
				subseqs.Any(s => {
					var regex = new Regex(@"\b" + s + @"\b", RegexOptions.IgnoreCase);
					return regex.IsMatch(q.Header) || regex.IsMatch(q.Text);
				})
			).OrderByDescending(q => q.Rate).ToList();


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
				.Where(q => tags.All(t => q.Tags.Any(qTag => qTag.Name.ToLower() == t.ToLower())));

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
				var foundTag = context.Tags.FirstOrDefault(t => t.Name.ToLower() == tag.ToLower());
				if (foundTag != null)
				{
					question.Tags.Add(foundTag);
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

			questionDto.Id = context.Questions.Max(question => question.Id);
		}

		public void UpdateQuestion(QuestionDTO questionDto)
		{
			using var context = new ApplicationContext();

			var question = context.Questions.Include(q => q.Tags).FirstOrDefault(q=> q.Id == questionDto.Id);

			if (questionDto.Text != null) question.Text = questionDto.Text;
			if (questionDto.Header != null) question.Header = questionDto.Header;

			question.UpdateTime = DateTime.Now;

			var deleteTags = question.Tags.Where(t => questionDto.Tags.FirstOrDefault(tag => tag.ToLower() == t.Name.ToLower()) == null).ToList();
			var addTags = questionDto.Tags.Where(t => question.Tags.FirstOrDefault(tag => tag.Name.ToLower() == t.ToLower()) == null).ToList();

			foreach (var tag in addTags)
			{
				var foundTag = context.Tags.FirstOrDefault(t => t.Name.ToLower() == tag.ToLower());
				if (foundTag != null)
				{
					question.Tags.Add(foundTag);
				}
				else
				{
					var dbTag = new Tag() { Name = tag };
					context.Tags.Add(dbTag);
					question.Tags.Add(dbTag);
				}
			}

			foreach (var tag in deleteTags)
			{
				question.Tags.Remove(tag);
			}

			context.SaveChanges();
		}

		public void DeleteQuestion(int questionId)
		{
			using var context = new ApplicationContext();

			var question = context.Questions.Find(questionId);

			var questionAnswers = context.Answers.Where(a => a.QuestionId == questionId).ToList();
			foreach (var answer in questionAnswers)
			{
				var answerComments = context.Comments.Where(c => c.AnswerId == answer.Id).ToList();
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

			var answers = context.Answers.Include(a => a.User).Where(a => a.QuestionId == questionId);

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

			var answerComments = context.Comments.Where(c => c.AnswerId == answerId).ToList();
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

			var comments = context.Comments.Include(c => c.User).Where(c => c.QuestionId == questionId && c.AnswerId == answerId);

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

		public void MarkQuestion(int userId, int questionId, int mark)
		{
			using var context = new ApplicationContext();

			var questionMark = context.QuestionMarks.Find(userId, questionId);

			if (questionMark != null && questionMark.Mark == mark)
			{
				throw new Exception("This user have already voted such.");
			}
			if (questionMark == null)
			{
				questionMark = new QuestionMark()
				{
					QuestionId = questionId,
					UserId = userId
				};
				context.QuestionMarks.Add(questionMark);
			}

			questionMark.Mark += mark;
			var question = context.Questions.Find(questionId);
			var user = context.Users.Find(question.UserId);
			question.Rate += mark;
			user.Rate += mark;

			context.SaveChanges();
		}

		public void MarkAnswer(int userId, int answerId, int mark)
		{
			using var context = new ApplicationContext();
			var AnswerMark = context.AnswerMarks.Find(userId, answerId);

			if (AnswerMark != null && AnswerMark.Mark == mark)
			{
				throw new Exception("This user have already voted such.");
			}
			if (AnswerMark == null)
			{
				AnswerMark = new AnswerMark()
				{
					AnswerId = answerId,
					UserId = userId
				};
				context.AnswerMarks.Add(AnswerMark);
			}

			AnswerMark.Mark += mark;
			var answer = context.Answers.Find(answerId);
			var user = context.Users.Find(answer.UserId);
			answer.Rate += mark;
			user.Rate += mark;

			context.SaveChanges();
		}

		public void MarkComment(int userId, int commentId, int mark)
		{
			using var context = new ApplicationContext();

			var commentMark = context.CommentMarks.Find(userId, commentId);

			if (commentMark != null && commentMark.Mark == mark)
			{
				throw new Exception("This user have already voted such.");
			}
			if (commentMark == null)
			{
				commentMark = new CommentMark()
				{
					CommentId = commentId,
					UserId = userId
				};
				context.CommentMarks.Add(commentMark);
			}

			commentMark.Mark += mark;
			var comment = context.Comments.Find(commentId);
			var user = context.Users.Find(comment.UserId);
			comment.Rate += mark;
			user.Rate += mark;

			context.SaveChanges();
		}


		public void CreateTag(string tag)
		{
			using var context = new ApplicationContext();

			var foundTag = context.Tags.FirstOrDefault(t => t.Name.ToLower() == tag.ToLower());
			if (foundTag == null)
			{
				var dbTag = new Tag() { Name = tag };
				context.Tags.Add(dbTag);
			}
			else
			{
				throw new Exception("Such tag already exists.");
			}

			context.SaveChanges();
		}


		public void UpdateTag(string oldTag, string newTag)
		{
			using var context = new ApplicationContext();

			var foundTag = context.Tags.FirstOrDefault(t => t.Name.ToLower() == oldTag.ToLower());
			if (foundTag == null)
			{
				throw new Exception("There is no such tag.");
			}
			else
			{
				foundTag.Name = newTag;
			}

			context.SaveChanges();
		}


		public void DeleteTag(string tag)
		{
			using var context = new ApplicationContext();

			var foundTag = context.Tags.FirstOrDefault(t => t.Name.ToLower() == tag.ToLower());
			if (foundTag == null)
			{
				throw new Exception("There is no such tag.");
			}
			else
			{
				context.Tags.Remove(foundTag);
			}

			context.SaveChanges();
		}
	}
}
