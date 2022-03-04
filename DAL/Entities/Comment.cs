using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
	public abstract class Comment
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
	}
}
