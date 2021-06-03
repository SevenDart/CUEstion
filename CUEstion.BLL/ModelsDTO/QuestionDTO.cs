﻿using CUEstion.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CUEstion.BLL.ModelsDTO
{
	public class QuestionDTO
	{
		public int Id { get; set; }

		public string Header { get; set; }

		public string Text { get; set; }

		public int Rate { get; set; }

		public int UserId { get; set; }
		public UserDTO User { get; set; }

		public List<AnswerDTO> Answers { get; set; }

		public QuestionDTO()
		{

		}


		public QuestionDTO(Question question)
		{
			Id = question.Id; 
			Header = question.Header;
			Text = question.Text;
			Rate = question.Rate;
			User = new UserDTO(question.User);
		}
	}
}