using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
	public class AnswerMark
	{
		public int UserId { get; set; }
		public User User { get; set; }

		public int AnswerId { get; set; }
		public Answer Answer { get; set; }

		public int Mark { get; set; }
	}
}
