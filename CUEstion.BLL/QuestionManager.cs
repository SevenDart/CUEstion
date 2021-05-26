using CUEstion.DAL.EF;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CUEstion.DAL.Entities;
using System.Collections.Generic;
using CUEstion.BLL.ModelsDTO;

namespace CUEstion.BLL
{
	public class QuestionManager
	{
		public static IEnumerable<QuestionDTO> GetAllQuestions()
		{
			using var context = new ApplicationContext();
			var questions = context.Questions.Include(q => q.Creator);
			var questionsList = new List<QuestionDTO>();
			foreach (var question in questions)
			{
				questionsList.Add(new QuestionDTO(question));
			}
			return questionsList;
		}

		public static QuestionDTO GetQuestion(int questionId)
		{
			using var context = new ApplicationContext();

			//Here I have a question. When I was reading about EF Core I found out that method Find() was fater then FirstOrDefault()
			//because of using indexes and etc. How can I use it, if I want to search after Include()?
			//Or should I divide this query like this?

			//var question = context.Questions.Find(questionId);
			//question.Creator = context.Users.Find(context.Entry(question).Property<int>("CreatorId").CurrentValue);
			//question.Answers = (from answer in context.Answers.Include(a => a.Creator) where EF.Property<int>(answer, "QuestionId") == question.Id select answer).ToList();

			var question = context.Questions.Include(q => q.Creator).Include(q => q.Answers).ThenInclude(a => a.Creator).FirstOrDefault(q => q.Id == questionId);

			var questionDTO = new QuestionDTO(question);

			questionDTO.Answers = new List<AnswerDTO>();
			foreach (var answer in question.Answers)
			{
				questionDTO.Answers.Add(new AnswerDTO(answer));
			}

			return questionDTO;
		}

		public static QuestionDTO CreateQuestion(QuestionDTO questionDto, int userId)
		{
			using var context = new ApplicationContext();

			//Here I'm getting userId by User.Identity Claim, which I will in future define.
			
			//Also I've seen a method to link entities without making a query to get a User.
			//I'm not sure that is not a "duct tape" but a right idea.

			//var user = new User() {Id = userId};
			//context.Users.Attach(user);

			//And just navigate with this user further when creating a user.


			var user = context.Users.Find(userId);

			var question = new Question()
			{
				Header = questionDto.Header,
				Text = questionDto.Text,
				Creator = user
			};

			context.Questions.Add(question);

			context.SaveChanges();

			return questionDto;
		}
	}
}
