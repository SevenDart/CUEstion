using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CUEstion.DAL.Entities
{
	public class User
	{
		public int Id { get; set; }
		
		[Required]
		[MaxLength(30)]
		public string Username { get; set; }

		[Required]
		[MaxLength(30)]
		public string Email { get; set; }

		[Required]
		[MaxLength(64)]
		public string Password { get; set; }

		[Required]
		[MaxLength(128)]
		public string Salt { get; set; }

		public int Rate { get; set; }

		public List<Tag> InterestedTags { get; set; }
		public List<FollowedQuestion> FollowedQuestions { get; set; }
	}
}
