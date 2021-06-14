using System;
using System.ComponentModel.DataAnnotations;

namespace CUEstion.DAL.Entities
{
	public class Comment
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(200)]
		public string Text { get; set; }

		public int Rate { get; set; }

		public DateTime CreateTime { get; set; }
		public DateTime? UpdateTime { get; set; }

		public int? UserId { get; set; }
		public User User { get; set; }

		public int? QuestionId { get; set; }
		public Question Question { get; set; }

		public int? AnswerId { get; set; }
		public Answer Answer { get; set; }
	}
}
