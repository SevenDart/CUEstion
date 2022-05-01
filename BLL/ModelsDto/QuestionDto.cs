using System;
using System.Linq;
using System.Collections.Generic;
using DAL.Entities;

namespace BLL.ModelsDTO
{
	public class QuestionDto
	{
		public int Id { get; set; }

		public string Header { get; set; }

		public string Text { get; set; }

		public int Rate { get; set; }
		
		public int? WorkspaceId { get; set; }

		public DateTime CreateTime { get; set; }
		public DateTime? UpdateTime { get; set; }

		public UserDto User { get; set; }

		public List<string> Tags { get; set; }

		public List<AnswerDto> Answers { get; set; }
	}
}
