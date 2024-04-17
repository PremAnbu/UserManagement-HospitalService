using Microsoft.AspNetCore.Mvc;
using System.Collections;
using UserManagementService.Dto.RequestDto;
using UserManagementService.Dto.ResponseDto;

namespace UserManagementService.Interface
{
    public interface IUser
    {
        public Task<bool> UserRegistration(UserRegistrationRequestModel userRegistrationModel,string role);
        public Task<string> UserLogin(LoginRequestModel userLogin);
        public Task<UserResponseModel> GetUserById(int userId);
        public Task<IEnumerable<UserResponseModel>> GetAllUsers();

    }
}
