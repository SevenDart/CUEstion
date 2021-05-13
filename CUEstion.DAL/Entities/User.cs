using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CUEstion.DAL.Entities
{
	class User
	{
		public int Id { get; set; }
		
		[Required]
		[MaxLength(20)]
		public string Username { get; set; }

		[Required]
		[MaxLength(20)]
		public string Email { get; set; }

		[Required]
		public int Rate { get; set; }

		public List<Tag> InterestedTags { get; set; }
		public List<FollowedQuestions> FollowedQuestions { get; set; }
	}
}
