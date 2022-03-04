using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
	public class AnswerMark: Mark
	{
		public int AnswerId { get; set; }
		public Answer Answer { get; set; }
	}
}
