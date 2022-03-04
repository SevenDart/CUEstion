using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
	public class CommentMark
	{
		public int UserId { get; set; }
		public User User { get; set; }

		public int CommentId { get; set; }
		public Comment Comment { get; set; }

		public int Mark { get; set; }
	}
}
