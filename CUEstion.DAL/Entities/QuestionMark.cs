﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUEstion.DAL.Entities
{
	public class QuestionMark
	{
		public int UserId { get; set; }
		public User User { get; set; }

		public int QuestionId { get; set; }
		public Question Question { get; set; }

		public int Mark { get; set; }
	}
}
