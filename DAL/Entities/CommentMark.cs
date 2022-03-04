using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
	public class CommentMark: Mark
	{
		public int CommentId { get; set; }
		public Comment Comment { get; set; }
	}
}
