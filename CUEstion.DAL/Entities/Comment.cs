﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

		public int? UserId { get; set; }
		public User User { get; set; }

		public bool CommentType { get; set; }

		public int? QuestionId { get; set; }
		public Question Question { get; set; }

		public int? AnswerId { get; set; }
		public Answer Answer { get; set; }
	}
}
