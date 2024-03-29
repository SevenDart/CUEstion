﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
	public class Question
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Header { get; set; }

		[Required]
		[MaxLength(3000)]
		public string Text { get; set; }

		public int Rate { get; set; }

		public int? UserId { get; set; }
		public User User { get; set; }
		
		public int? WorkspaceId { get; set; }
		public Workspace Workspace { get; set; }

		public DateTime CreateTime { get; set; }
		public DateTime? UpdateTime { get; set; }

		public List<Tag> Tags { get; set; }
		public List<Answer> Answers { get; set; }
		public List<QuestionComment> Comments { get; set; }
	}
}
