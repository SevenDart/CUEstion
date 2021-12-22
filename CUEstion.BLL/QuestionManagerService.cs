using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CUEstion.BLL.Interfaces;
using CUEstion.DAL.Entities;
using CUEstion.DAL.EF;
using CUEstion.BLL.ModelsDTO;
using Microsoft.EntityFrameworkCore;

namespace CUEstion.BLL
{
	public class QuestionManagerService: IQuestionManagerService
	{
		private readonly ApplicationContext _context;

		public QuestionManagerService(ApplicationContext context)
		{
			_context = context;
		}

		public IEnumerable<QuestionDTO> GetAllQuestions()
		{
			var questions = _context.Questions.Include(q => q.Tags).ToList();
			var questionsList = new List<QuestionDTO>();
			foreach (var question in questions)
			{
				questionsList.Add(new QuestionDTO(question));
			}
			return questionsList;
		}

		public IEnumerable<String> GetAllTags()
		{
			var tags = _context.Tags.Select(t => t.Name).ToList();
			return tags;
		}

		public IEnumerable<QuestionDTO> GetNewestQuestions(int count)
		{
			var questions = _context.Questions.Include(q => q.Tags)
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
			var question = _context.Questions.Include(q => q.Tags).FirstOrDefault(q => q.Id == questionId);
			question.User = _context.Users.Find(question.UserId);
			return question != null 
				? new QuestionDTO(question) 
				: null;
		}

		public IEnumerable<QuestionDTO> Search(string query, string[] tags)
		{
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

			var questions = _context.Questions.Include(q => q.Tags).ToList()
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
			var questions = _context.Questions.Include(q => q.Tags).ToList()
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
			var questions = _context.Questions.Include(t => t.Tags).Where(q => q.UserId == userId);
			var questionsList = new List<QuestionDTO>();
			foreach (var question in questions)
			{
				questionsList.Add(new QuestionDTO(question));
			}
			return questionsList;
		}

		public void CreateQuestion(QuestionDTO questionDto)
		{
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
				var foundTag = _context.Tags.FirstOrDefault(t => t.Name.ToLower() == tag.ToLower());
				if (foundTag != null)
				{
					question.Tags.Add(foundTag);
				}
				else
				{
					var dbTag = new Tag() { Name = tag };
					_context.Tags.Add(dbTag);
					question.Tags.Add(dbTag);
				}
			}

			_context.Questions.Add(question);

			_context.SaveChanges();

			questionDto.Id = _context.Questions.Max(question => question.Id);
		}

		public void UpdateQuestion(QuestionDTO questionDto)
		{
			var question = _context.Questions.Include(q => q.Tags).FirstOrDefault(q=> q.Id == questionDto.Id);

			if (questionDto.Text != null) question.Text = questionDto.Text;
			if (questionDto.Header != null) question.Header = questionDto.Header;

			question.UpdateTime = DateTime.Now;

			var deleteTags = question.Tags.Where(t => questionDto.Tags.FirstOrDefault(tag => tag.ToLower() == t.Name.ToLower()) == null).ToList();
			var addTags = questionDto.Tags.Where(t => question.Tags.FirstOrDefault(tag => tag.Name.ToLower() == t.ToLower()) == null).ToList();

			foreach (var tag in addTags)
			{
				var foundTag = _context.Tags.FirstOrDefault(t => t.Name.ToLower() == tag.ToLower());
				if (foundTag != null)
				{
					question.Tags.Add(foundTag);
				}
				else
				{
					var dbTag = new Tag() { Name = tag };
					_context.Tags.Add(dbTag);
					question.Tags.Add(dbTag);
				}
			}

			foreach (var tag in deleteTags)
			{
				question.Tags.Remove(tag);
			}

			_context.SaveChanges();
		}

		public void DeleteQuestion(int questionId)
		{
			var question = _context.Questions.Find(questionId);

			var questionAnswers = _context.Answers.Where(a => a.QuestionId == questionId).ToList();
			foreach (var answer in questionAnswers)
			{
				var answerComments = _context.Comments.Where(c => c.AnswerId == answer.Id).ToList();
				foreach (var comment in answerComments)
					_context.Comments.Remove(comment);
				_context.Answers.Remove(answer);
			}

			var questionComments = _context.Comments.Where(c => c.QuestionId == questionId);
			foreach (var comment in questionComments)
			{
				_context.Comments.Remove(comment);
			}

			_context.Questions.Remove(question);

			_context.SaveChanges();
		}

		public void SubscribeToQuestion(int questionId, int userId)
		{
			var followedQuestion = new FollowedQuestion()
			{
				UserId = userId,
				QuestionId = questionId
			};

			_context.FollowedQuestions.Add(followedQuestion);

			_context.SaveChanges();
		}

		public IEnumerable<AnswerDTO> GetAnswers(int questionId)
		{
			var answers = _context.Answers.Include(a => a.User).Where(a => a.QuestionId == questionId);

			var answersList = new List<AnswerDTO>();
			foreach (var answer in answers)
			{
				answersList.Add(new AnswerDTO(answer));
			}

			return answersList;
		}

		public void CreateAnswer(AnswerDTO answerDto, int questionId)
		{
			var answer = new Answer()
			{
				Text = answerDto.Text,
				CreateTime = DateTime.Now,
				QuestionId = questionId,
				UserId = answerDto.User.Id
			};

			_context.Answers.Add(answer);

			_context.SaveChanges();
		}

		public void UpdateAnswer(AnswerDTO answerDto)
		{
			var answer = _context.Answers.Find(answerDto.Id);

			if (answerDto.Text != null) answer.Text = answerDto.Text;

			answer.UpdateTime = DateTime.Now;

			_context.SaveChanges();
		}

		public void DeleteAnswer(int answerId)
		{
			var answer = _context.Answers.Find(answerId);

			var answerComments = _context.Comments.Where(c => c.AnswerId == answerId).ToList();
			foreach (var comment in answerComments)
			{
				_context.Comments.Remove(comment);
			}

			_context.Answers.Remove(answer);

			_context.SaveChanges();
		}


		public IEnumerable<CommentDTO> GetComments(int? questionId, int? answerId)
		{
			var comments = _context.Comments.Include(c => c.User).Where(c => c.QuestionId == questionId && c.AnswerId == answerId);

			var commentsList = new List<CommentDTO>();

			foreach (var comment in comments)
			{
				commentsList.Add(new CommentDTO(comment));
			}

			return commentsList;
		}

		public void CreateComment(CommentDTO commentDto, int? questionId, int? answerId)
		{
			var comment = new Comment()
			{
				Text = commentDto.Text,
				CreateTime = DateTime.Now,
				QuestionId = questionId,
				AnswerId = answerId,
				UserId = commentDto.User.Id
			};

			_context.Comments.Add(comment);

			_context.SaveChanges();
		}

		public void UpdateComment(CommentDTO commentDto)
		{
			var comment = _context.Comments.Find(commentDto.Id);

			if (commentDto.Text != null) comment.Text = commentDto.Text;

			comment.UpdateTime = DateTime.Now;

			_context.SaveChanges();
		}

		public void DeleteComment(int commentId)
		{
			var comment = _context.Comments.Find(commentId);

			_context.Comments.Remove(comment);

			_context.SaveChanges();
		}

		public void MarkQuestion(int userId, int questionId, int mark)
		{
			var questionMark = _context.QuestionMarks.Find(userId, questionId);

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
				_context.QuestionMarks.Add(questionMark);
			}

			questionMark.Mark += mark;
			var question = _context.Questions.Find(questionId);
			var user = _context.Users.Find(question.UserId);
			question.Rate += mark;
			user.Rate += mark;

			_context.SaveChanges();
		}

		public void MarkAnswer(int userId, int answerId, int mark)
		{
			var AnswerMark = _context.AnswerMarks.Find(userId, answerId);

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
				_context.AnswerMarks.Add(AnswerMark);
			}

			AnswerMark.Mark += mark;
			var answer = _context.Answers.Find(answerId);
			var user = _context.Users.Find(answer.UserId);
			answer.Rate += mark;
			user.Rate += mark;

			_context.SaveChanges();
		}

		public void MarkComment(int userId, int commentId, int mark)
		{
			var commentMark = _context.CommentMarks.Find(userId, commentId);

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
				_context.CommentMarks.Add(commentMark);
			}

			commentMark.Mark += mark;
			var comment = _context.Comments.Find(commentId);
			var user = _context.Users.Find(comment.UserId);
			comment.Rate += mark;
			user.Rate += mark;

			_context.SaveChanges();
		}


		public void CreateTag(string tag)
		{
			var foundTag = _context.Tags.FirstOrDefault(t => t.Name.ToLower() == tag.ToLower());
			if (foundTag == null)
			{
				var dbTag = new Tag() { Name = tag };
				_context.Tags.Add(dbTag);
			}
			else
			{
				throw new Exception("Such tag already exists.");
			}

			_context.SaveChanges();
		}


		public void UpdateTag(string oldTag, string newTag)
		{
			var foundTag = _context.Tags.FirstOrDefault(t => t.Name.ToLower() == oldTag.ToLower());
			if (foundTag == null)
			{
				throw new Exception("There is no such tag.");
			}
			else
			{
				foundTag.Name = newTag;
			}

			_context.SaveChanges();
		}


		public void DeleteTag(string tag)
		{
			var foundTag = _context.Tags.FirstOrDefault(t => t.Name.ToLower() == tag.ToLower());
			if (foundTag == null)
			{
				throw new Exception("There is no such tag.");
			}
			else
			{
				_context.Tags.Remove(foundTag);
			}

			_context.SaveChanges();
		}

		public bool IsSubscribedToQuestion(int questionId, int userId)
		{
			return _context.FollowedQuestions.Contains(new FollowedQuestion() { QuestionId = questionId, UserId = userId });
		}

		public void UnsubscribeFromQuestion(int questionId, int userId)
		{
			var followedQuestion = new FollowedQuestion()
			{
				UserId = userId,
				QuestionId = questionId
			};

			_context.FollowedQuestions.Remove(followedQuestion);

			_context.SaveChanges();
		}
	}
}
