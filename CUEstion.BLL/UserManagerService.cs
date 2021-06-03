using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using CUEstion.DAL.EF;
using CUEstion.BLL.ModelsDTO;
using CUEstion.DAL.Entities;

namespace CUEstion.BLL
{
	public class UserManagerService
	{
		//TODO fix password
		public void CreateUser(UserDTO userDto)
		{
			using var context = new ApplicationContext();

			var user = new User()
			{
				Email = userDto.Email, 
				Username = userDto.Username 
			};

			context.Users.Add(user);

			context.SaveChanges();
		}


	}
}
