using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CUEstion.BLL.Interfaces;
using CUEstion.BLL.ModelsDTO;
using CUEstion.DAL.EF;
using CUEstion.DAL.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CUEstion.BLL.Implementations
{
	public class QuestionManagerService: IQuestionManagerService
	{
		private readonly ApplicationContext _context;

		public QuestionManagerService(ApplicationContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<QuestionDTO>> GetAllQuestions()
		{
			var questions = _context
				.Questions
				.Include(q => q.Tags)
				.ProjectToType<QuestionDTO>().ToListAsync();
			return await questions;
		}

		public async Task<IEnumerable<QuestionDTO>> GetNewestQuestions(int count)
		{
			var questions = _context.Questions.Include(q => q.Tags)
				.OrderByDescending(q => q.CreateTime)
				.ThenByDescending(q => q.UpdateTime)
				.ThenByDescending(q => q.Rate)
				.Take(count)
				.ProjectToType<QuestionDTO>()
				.ToListAsync();
			return await questions;
		}

		public async Task<QuestionDTO> GetQuestion(int questionId)
		{
			var question = await _context.Questions.Include(q => q.Tags).FirstOrDefaultAsync(q => q.Id == questionId);
			question.User = await _context.Users.FindAsync(question.UserId);
			return question.Adapt<QuestionDTO>();
		}

		public async Task<IEnumerable<QuestionDTO>> Search(string query, string[] tags)
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

			IEnumerable<Question> questions = (await _context.Questions.Include(q => q.Tags).ToListAsync())
				.Where(q => tags.All(t => q.Tags.Any(qTag => qTag.Name.ToLower() == t.ToLower()))).ToList();

			questions = questions
				.Where(q =>
				subseqs.Any(s => {
					var regex = new Regex(@"\b" + s + @"\b", RegexOptions.IgnoreCase);
					return regex.IsMatch(q.Header) || regex.IsMatch(q.Text);
				}))
				.OrderByDescending(q => q.Rate);
			
			return questions.Adapt<QuestionDTO[]>();

		}

		public async Task<IEnumerable<QuestionDTO>> FilterQuestions(string[] tags)
		{
			var questions = (await _context
				.Questions
				.Include(q => q.Tags)
				.ToListAsync())
				.Where(q => tags.All(t => q.Tags.Any(qTag => String.Equals(qTag.Name, t, StringComparison.CurrentCultureIgnoreCase))));
			
			return questions.Adapt<QuestionDTO[]>();
		}

		public async Task<IEnumerable<QuestionDTO>> GetUsersQuestions(int userId)
		{
			var questions = await _context
				.Questions
				.Include(t => t.Tags)
				.Where(q => q.UserId == userId)
				.ProjectToType<QuestionDTO>()
				.ToListAsync();
			return questions;
		}

		public async Task CreateQuestion(QuestionDTO questionDto)
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
				var foundTag = _context.Tags.FirstOrDefault(t => String.Equals(t.Name, tag, StringComparison.CurrentCultureIgnoreCase));
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

			await _context.SaveChangesAsync();

			questionDto.Id = _context.Questions.Max(q => q.Id);
		}

		public async Task UpdateQuestion(QuestionDTO questionDto)
		{
			var question = _context
				.Questions
				.Include(q => q.Tags)
				.FirstOrDefault(q=> q.Id == questionDto.Id);

			if (question == null)
			{
				throw new NullReferenceException();
			}
			
			if (questionDto.Text != null)
			{
				question.Text = questionDto.Text;
			}
			if (questionDto.Header != null)
			{
				question.Header = questionDto.Header;
			}

			question.UpdateTime = DateTime.Now;

			var deleteTags = question
				.Tags
				.Where(t => questionDto.Tags.FirstOrDefault(tag => string.Equals(tag, t.Name, StringComparison.CurrentCultureIgnoreCase)) == null)
				.ToList();
			
			var addTags = questionDto
				.Tags
				.Where(t => question.Tags.FirstOrDefault(tag => string.Equals(tag.Name, t, StringComparison.CurrentCultureIgnoreCase)) == null)
				.ToList();

			foreach (var tag in addTags)
			{
				var foundTag = _context.Tags.FirstOrDefault(t => String.Equals(t.Name, tag, StringComparison.CurrentCultureIgnoreCase));
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

			await _context.SaveChangesAsync();
		}

		public async Task DeleteQuestion(int questionId)
		{
			var question = await _context
				.Questions
				.FindAsync(questionId);

			var questionAnswers = await _context
				.Answers
				.Where(a => a.QuestionId == questionId)
				.ToListAsync();
			
			foreach (var answer in questionAnswers)
			{
				var answerComments = await _context.Comments.Where(c => c.AnswerId == answer.Id).ToListAsync();
				
				foreach (var comment in answerComments)
					_context.Comments.Remove(comment);
				
				_context.Answers.Remove(answer);
			}

			var questionComments = await _context.Comments.Where(c => c.QuestionId == questionId).ToListAsync();
			
			foreach (var comment in questionComments)
			{
				_context.Comments.Remove(comment);
			}

			_context.Questions.Remove(question);

			await _context.SaveChangesAsync();
		}

		public async Task SubscribeToQuestion(int questionId, int userId)
		{
			var followedQuestion = new FollowedQuestion()
			{
				UserId = userId,
				QuestionId = questionId
			};

			_context.FollowedQuestions.Add(followedQuestion);

			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<CommentDTO>> GetComments(int? questionId, int? answerId)
		{
			var comments = await _context
				.Comments
				.Include(c => c.User)
				.Where(c => c.QuestionId == questionId && c.AnswerId == answerId)
				.ProjectToType<CommentDTO>()
				.ToListAsync();

			return comments;
		}

		public async Task CreateComment(CommentDTO commentDto, int? questionId, int? answerId)
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

			await _context.SaveChangesAsync();
		}

		public async Task UpdateComment(CommentDTO commentDto)
		{
			var comment = await _context
				.Comments
				.FindAsync(commentDto.Id);

			if (commentDto.Text != null)
			{
				comment.Text = commentDto.Text;
			}

			comment.UpdateTime = DateTime.Now;

			await _context.SaveChangesAsync();
		}

		public async Task DeleteComment(int commentId)
		{
			var comment = await _context
				.Comments
				.FindAsync(commentId);

			_context.Comments.Remove(comment);

			await _context.SaveChangesAsync();
		}

		public async Task MarkQuestion(int userId, int questionId, int mark)
		{
			var questionMark = await _context
				.QuestionMarks
				.FindAsync(userId, questionId);

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
			var question = await _context.Questions.FindAsync(questionId);
			var user = await _context.Users.FindAsync(question.UserId);
			question.Rate += mark;
			user.Rate += mark;

			await _context.SaveChangesAsync();
		}

		

		public async Task<bool> IsSubscribedToQuestion(int questionId, int userId)
		{
			return await _context
				.FollowedQuestions
				.ContainsAsync(new FollowedQuestion()
				{
					QuestionId = questionId, 
					UserId = userId
				});
		}

		public async Task UnsubscribeFromQuestion(int questionId, int userId)
		{
			var followedQuestion = new FollowedQuestion()
			{
				UserId = userId,
				QuestionId = questionId
			};

			_context.FollowedQuestions.Remove(followedQuestion);

			await _context.SaveChangesAsync();
		}
	}
}
