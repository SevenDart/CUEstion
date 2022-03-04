using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
	public class QuestionMark: Mark
	{
		public int QuestionId { get; set; }
		public Question Question { get; set; }
	}
}
