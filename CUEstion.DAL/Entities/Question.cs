using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CUEstion.DAL.Entities
{
	public class Question
	{
		public int Id { get; set; }
		
		[Required]
		[MaxLength(50)]
		public string Header { get; set; }

		[Required]
		[MaxLength(500)]
		public string Text { get; set; }

		public int Rate { get; set; }
		
		public int? UserId { get; set; }
		public User User { get; set; }

		public List<Tag> Tags { get; set; }
		public List<Answer> Answers { get; set; }
		public List<Comment> Comments { get; set; }

		public List<FollowedQuestion> FollowedQuestions { get; set; }
	}
}
