using Dapper;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using UserManagementService.DapperContext;
using UserManagementService.Dto.RequestDto;
using UserManagementService.Dto.ResponseDto;
using UserManagementService.Entity;
using UserManagementService.GlobleCustomExceptionHandler;
using UserManagementService.Interface;

namespace UserManagementService.ServiceImpl
{
    public class UserServiceImpl : IUser
    {
        private readonly UserManagementServiceContext _context;
        private readonly IAuthServices _authService;
        private readonly IEmail _emailService;
        public UserServiceImpl(UserManagementServiceContext context, IAuthServices authService, IEmail emailService)
        {
            _context = context;
            _authService = authService;
            _emailService = emailService;
        }
        public async Task<bool> UserRegistration(UserRegistrationRequestModel userRegistrationModel, string role)  
        {
                var parameters = new DynamicParameters();
                parameters.Add("FirstName", userRegistrationModel.FirstName, DbType.String);
                parameters.Add("LastName", userRegistrationModel.LastName, DbType.String);

                //Check Emailformat Using Regex
                if (!IsValidEmail(userRegistrationModel.Email))
                    throw new InvalidEmailFormatException("Invalid email format");
                parameters.Add("Email", userRegistrationModel.Email, DbType.String);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegistrationModel.Password);
                parameters.Add("Password", hashedPassword, DbType.String);

                parameters.Add("Role", role, DbType.String);
            using (var connection = _context.CreateConnection())
            {
               int val= await connection.ExecuteAsync("spRegisterUser", parameters, commandType: CommandType.StoredProcedure);
                if (val > 0)
                    return true;
            }
            return false;
        }
        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        public async Task<string> UserLogin(LoginRequestModel loginRequestModel)
        {
            using (var connection = _context.CreateConnection())
            {
                var parameter = new DynamicParameters();
                parameter.Add("Email",loginRequestModel.Email, DbType.String);              
                UserEntity value = await connection.QueryFirstOrDefaultAsync<UserEntity>("spUserGetByEmail",parameter,commandType:CommandType.StoredProcedure);
                if (value == null)
                {
                    throw new UserNotFoundException("User Not Found ");
                }
                if (!BCrypt.Net.BCrypt.Verify(loginRequestModel.Password, value.Password))
                {  throw new InvalidPasswordException("Password Incorrect"); }
                return _authService.GenerateJwtToken(value);
            }
        }

        public async Task<UserResponseModel> GetUserById(int userId)
        {
            try
            {
                var parameters = new { UserId = userId };
                using (var connection = _context.CreateConnection())
                {
                    UserResponseModel user = await connection.QueryFirstOrDefaultAsync<UserResponseModel>("spGetUserById", parameters, commandType: CommandType.StoredProcedure);
                    if (user == null)
                    {
                        throw new UserNotFoundException($"User with ID '{userId} not found.");
                    }
                    return user;
                }
            }
            catch (Exception ex)
            {
                throw new UserNotFoundException("An error occurred while fetching user by ID.", ex);
            }
        }
        public async Task<IEnumerable<UserResponseModel>> GetAllUsers()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    IEnumerable<UserResponseModel> users = await connection.QueryAsync<UserResponseModel>("spGetAllUsers", commandType: CommandType.StoredProcedure);
                    return users;
                }
            }
            catch (Exception ex)
            {
                throw new UserNotFoundException("An error occurred while fetching all users.", ex);
            }
        }
    }
}
