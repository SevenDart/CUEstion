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

		public QuestionDTO CreateQuestion(QuestionDTO questionDto)
		{
			using var context = new ApplicationContext();

			var question = new Question()
			{
				Header = questionDto.Header,
				Text = questionDto.Text,
				User = context.Users.Find(questionDto.UserId)
			};

			context.Questions.Add(question);

			context.SaveChanges();

			questionDto.Id = context.Questions.Find(question).Id;

			return questionDto;
		}
	}
}
