using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.ModelsDTO;
using DAL.EF;
using DAL.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementations
{
	public class QuestionManagerService: IQuestionManagerService
	{
		private readonly ApplicationContext _context;
		private readonly ITagManagerService _tagManagerService;
		private readonly IMarkManagerService _markManagerService;

		public QuestionManagerService(ApplicationContext context, 
			ITagManagerService tagManagerService, 
			IMarkManagerService markManagerService)
		{
			_context = context;
			_tagManagerService = tagManagerService;
			_markManagerService = markManagerService;
		}

		public async Task<IEnumerable<QuestionDto>> GetAllQuestions()
		{
			var questions = await _context
				.Questions
				.Include(q => q.Tags)
				.ToListAsync();
			
			return questions.Adapt<QuestionDto[]>();
		}

		public async Task<IEnumerable<QuestionDto>> GetNewestQuestions(int count)
		{
			var questions = await _context
				.Questions
				.Include(q => q.Tags)
				.OrderByDescending(q => q.CreateTime)
					.ThenByDescending(q => q.UpdateTime)
					.ThenByDescending(q => q.Rate)
				.Take(count)
				.ToListAsync();
			
			return questions.Adapt<QuestionDto[]>();
		}

		public async Task<QuestionDto> GetQuestion(int questionId)
		{
			var question = await _context
				.Questions
				.Include(q => q.Tags)
				.FirstOrDefaultAsync(q => q.Id == questionId);
			
			question.User = await _context.Users.FindAsync(question.UserId);

			return question.Adapt<QuestionDto>();
		}

		public async Task<IEnumerable<QuestionDto>> Search(string query, string[] tags)
		{
			var words = query.Split(' ', '.', ',', ':', '?', '!');
			var subseqs = new List<string>();

			for (int length = 1; length <= words.Length; length++)
			{
				var wordsCount = 0;
				var subseq = "";
				for (int i = 0; i < words.Length; i++)
				{
					if (wordsCount >= length)
					{
						wordsCount--;
						subseq = subseq.Substring(words[i - length].Length + ((subseq[^1] == ' ') ? 1 : 0));
					}
					if (subseq != "")
					{
						subseq += " ";
					}
					subseq += words[i];
					wordsCount++;
					if (wordsCount == length)
					{
						subseqs.Add(subseq);
					}
				}
			}

			IEnumerable<Question> questions = (await _context
					.Questions
					.Include(q => q.Tags)
					.ToListAsync()
				).Where(q => tags.All(t => q.Tags.Any(qTag => string.Equals(qTag.Name, t, StringComparison.CurrentCultureIgnoreCase))))
				.ToList();

			questions = questions
				.Where(q =>
				subseqs.Any(s => {
					var regex = new Regex(@"\b" + s + @"\b", RegexOptions.IgnoreCase);
					return regex.IsMatch(q.Header) || regex.IsMatch(q.Text);
				}))
				.OrderByDescending(q => q.Rate);
			
			return questions.Adapt<QuestionDto[]>();

		}

		public async Task<IEnumerable<QuestionDto>> FilterQuestions(string[] tags)
		{
			var questions = (await _context
				.Questions
				.Include(q => q.Tags)
				.ToListAsync())
				.Where(q => tags.All(t => q.Tags.Any(qTag => string.Equals(qTag.Name, t, StringComparison.CurrentCultureIgnoreCase))));
			
			return questions.Adapt<QuestionDto[]>();
		}

		public async Task<IEnumerable<QuestionDto>> GetUsersQuestions(int userId)
		{
			var questions = await _context
				.Questions
				.Include(t => t.Tags)
				.Where(q => q.UserId == userId)
				.ToListAsync();
			
			return questions.Adapt<QuestionDto[]>();
		}

		public async Task CreateQuestion(QuestionDto questionDto)
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
				var foundTag = await _tagManagerService.FindTag(tag);
				question.Tags.Add(foundTag);
			}

			_context.Questions.Add(question);

			await _context.SaveChangesAsync();

			questionDto.Id = question.Id;
		 }

		public async Task UpdateQuestion(QuestionDto questionDto)
		{
			var question = await _context
				.Questions
				.Include(q => q.Tags)
				.FirstOrDefaultAsync(q=> q.Id == questionDto.Id);

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
				.Where(t => questionDto.Tags.FirstOrDefault(tag => tag.ToLower() == t.Name.ToLower()) == null)
				.ToList();
			
			var addTags = questionDto
				.Tags
				.Where(t => question.Tags.FirstOrDefault(tag => tag.Name.ToLower() == t.ToLower()) == null)
				.ToList();
			
			foreach (var tag in addTags)
			{
				var foundTag = await _tagManagerService.FindTag(tag);
				if (foundTag == null)
				{
					throw new Exception($"No such tag: {tag}.");
				}
				question.Tags.Add(foundTag);
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
		

		public async Task MarkQuestion(int userId, int questionId, int markValue)
		{
			var questionMark = await _context
				.QuestionMarks
				.FindAsync(userId, questionId);
			
			if (questionMark == null)
			{
				questionMark = new QuestionMark()
				{
					QuestionId = questionId,
					UserId = userId
				};
				_context.QuestionMarks.Add(questionMark);
			}
			
			await _markManagerService.SetMark(questionMark, markValue);

			var question = await _context.Questions.FindAsync(questionId);
			question.Rate += markValue;

			await _context.SaveChangesAsync();
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
		
		public async Task<bool> IsSubscribedToQuestion(int questionId, int userId)
		{
			var isSubscribed = await _context
				.FollowedQuestions
				.ContainsAsync(new FollowedQuestion()
				{
					QuestionId = questionId, 
					UserId = userId
				});
			return isSubscribed;
		}
	}
}
