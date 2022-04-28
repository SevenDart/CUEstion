using System;
using DAL.Entities;

namespace BLL.ModelsDTO
{
	public class CommentDto
	{
		public int Id { get; set; }

		public int Rate { get; set; }

		public string Text { get; set; }

		public DateTime CreateTime { get; set; }
		public DateTime? UpdateTime { get; set; }

		public int? QuestionId { get; set; }

		public int? AnswerId { get; set; }

		public UserDto User { get; set; }
	}
}
