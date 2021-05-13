using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CUEstion.DAL.Entities
{
	class Tag
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(10)]
		public string Name { get; set; }

		public List<Question> Questions { get; set; }
		public List<User> Users { get; set; }
	}
}
