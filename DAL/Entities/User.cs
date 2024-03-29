﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace DAL.Entities
{
	public class User
	{
		public int Id { get; set; }
		
		[Required]
		[MaxLength(30)]
		public string Username { get; set; }

		[Required]
		[MaxLength(30)]
		public string Email { get; set; }

		[Required]
		[MaxLength(64)]
		public string Password { get; set; }

		[Required]
		[StringLength(16)]
		public string Salt { get; set; }
		
		[Required]
		[MaxLength(10)]
		public string SystemRole { get; set; }

		public int Rate { get; set; }
		
		public IEnumerable<WorkspaceUser> Workspaces { get; set; }
	}
}
