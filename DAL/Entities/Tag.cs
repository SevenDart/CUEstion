using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace DAL.Entities
{
	public class Tag
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(10)]
		public string Name { get; set; }

		public List<Question> Questions { get; set; }
	}
}
