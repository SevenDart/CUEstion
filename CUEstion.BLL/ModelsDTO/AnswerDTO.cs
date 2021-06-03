using CUEstion.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CUEstion.BLL.ModelsDTO
{
	public class AnswerDTO
	{
		public int Id { get; set; }

		public string Text { get; set; }

		public int Rate { get; set; }

		public DateTime CreateTime { get; set; }
		public DateTime UpdateTime { get; set; }

		public UserDTO User { get; set; }


		public AnswerDTO()
		{

		}

		public AnswerDTO(Answer answer)
		{
			Id = answer.Id;
			Text = answer.Text;
			Rate = answer.Rate;
			User = new UserDTO(answer.User);
		}
	}
}
