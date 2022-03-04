using System;
using DAL.Entities;

namespace BLL.ModelsDTO
{
	public class AnswerDto
	{
		public int Id { get; set; }

		public string Text { get; set; }

		public int Rate { get; set; }

		public DateTime CreateTime { get; set; }
		public DateTime? UpdateTime { get; set; }

		public UserDto User { get; set; }
	}
}
