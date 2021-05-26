using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

		[Required]
		public User Creator { get; set; }

		[Required]
		public Question Question { get; set; }

		public List<Comment> Comments { get; set; }
	}
}
