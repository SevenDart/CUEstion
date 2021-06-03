using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CUEstion.DAL.Entities
{
	public class Answer
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(500)]
		public string Text { get; set; }

		public int Rate { get; set; }

		public DateTime CreateTime { get; set; }
		public DateTime? UpdateTime { get; set; }

		public int? UserId { get; set; }
		public User User { get; set; }

		public int QuestionId { get; set; }
		public Question Question { get; set; }

		public List<Comment> Comments { get; set; }
	}
}
