using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.ModelsDTO;

namespace BLL.Interfaces
{
    public interface IUserManagerService
    {
        public Task<AuthDto> CreateUser(AuthDto authDto);
        public Task<AuthDto> CheckAuthData(AuthDto authDto);
        public Task UpdateUserInfo(UserDto userDto);
        public Task<UserDto> GetUser(int userId);
        public Task DeleteUser(int userId);
        public Task<IEnumerable<UserDto>> GetAllUsers();
        public Task<IEnumerable<QuestionDto>> GetFollowedQuestions(int userId);
    }
}