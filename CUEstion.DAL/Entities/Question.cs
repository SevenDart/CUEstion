using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CUEstion.DAL.Entities
{
	class Question
	{
		public int Id { get; set; }
		
		[Required]
		[MaxLength(50)]
		public string Header { get; set; }

		[Required]
		[MaxLength(500)]
		public string Text { get; set; }

		public int Rate { get; set; }
		
		[Required]
		public User Creator { get; set; }

		public List<Tag> Tags { get; set; }
		public List<Answer> Answers { get; set; }
		public List<Comment> Comments { get; set; }

		public List<FollowedQuestions> FollowedQuestions { get; set; }
	}
}
