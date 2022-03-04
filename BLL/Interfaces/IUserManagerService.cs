using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.ModelsDTO;

namespace BLL.Interfaces
{
    public interface IUserManagerService
    {
        public AuthDto CreateUser(AuthDto authDto);
        public AuthDto CheckAuthData(AuthDto authDto);
        public void UpdateUserInfo(UserDto userDto);
        public UserDto GetUser(int userId);
        public void DeleteUser(int userId);
        public IEnumerable<UserDto> GetAllUsers();
        public Task<IEnumerable<QuestionDto>> GetUsersQuestions(int userId);
        public Task<IEnumerable<QuestionDto>> GetFollowedQuestions(int userId);
    }
}