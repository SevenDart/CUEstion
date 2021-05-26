using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CUEstion.DAL.Entities
{
	public class Comment
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(200)]
		public string Text { get; set; }

		public int Rate { get; set; }

		[Required]
		public User Creator { get; set; }

		public bool CommentType { get; set; }
		public Question Question { get; set; }
		public Answer Answer { get; set; }
	}
}
