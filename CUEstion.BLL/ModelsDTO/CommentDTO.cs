using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using CUEstion.DAL.Entities;

namespace CUEstion.BLL.ModelsDTO
{
	public class CommentDTO
	{
		public CommentDTO(Comment comment)
		{
			Text = comment.Text;
			QuestionId = comment.QuestionId;
			AnswerId = comment.AnswerId;
			User = new UserDTO(comment.User);
		}

		public string Text { get; set; }

		public DateTime CreateTime { get; set; }
		public DateTime UpdateTime { get; set; }

		public int? QuestionId { get; set; }

		public int? AnswerId { get; set; }

		public UserDTO User { get; set; }

	}
}
