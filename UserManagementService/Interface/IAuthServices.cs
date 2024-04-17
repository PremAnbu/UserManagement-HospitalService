using UserManagementService.Entity;

namespace UserManagementService.Interface
{
    public interface IAuthServices
    {
        public string GenerateJwtToken(UserEntity user);

    }
}
