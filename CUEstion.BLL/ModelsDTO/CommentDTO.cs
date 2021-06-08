﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using CUEstion.DAL.Entities;

namespace CUEstion.BLL.ModelsDTO
{
	public class CommentDTO
	{
		public int Id { get; set; }

		public CommentDTO()
		{

		}

		public CommentDTO(Comment comment)
		{
			Id = comment.Id;
			Text = comment.Text;
			QuestionId = comment.QuestionId;
			AnswerId = comment.AnswerId;
			CreateTime = comment.CreateTime;
			UpdateTime = comment.UpdateTime;
			if (comment.User != null)
				User = new UserDTO(comment.User);
		}

		public string Text { get; set; }

		public DateTime CreateTime { get; set; }
		public DateTime? UpdateTime { get; set; }

		public int? QuestionId { get; set; }

		public int? AnswerId { get; set; }

		public UserDTO User { get; set; }

	}
}
