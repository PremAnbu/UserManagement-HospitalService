using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagementService.Dto.RequestDto;
using UserManagementService.Dto.ResponseDto;
using UserManagementService.GlobleCustomExceptionHandler;
using UserManagementService.Interface;

namespace UserManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : Controller
    {
        private readonly IUser _user;

        public UserManagementController(IUser user)
        {
            _user = user;
        }

        [HttpPost("PatientRegister")]
        public async Task<IActionResult> PatientRegistration(UserRegistrationRequestModel user)
        {
            return await Registration(user,"Patient"); 
        }

        [HttpPost("DoctorRegister")]
        public async Task<IActionResult> DoctorRegistration(UserRegistrationRequestModel user)
        {
            return await Registration(user, "Doctor");
        }

        [HttpPost("AdminRedister")]
        public async Task<IActionResult> AdminRegistration(UserRegistrationRequestModel user)
        {
            return await Registration(user, "Admin");
        }

        private async Task<IActionResult> Registration(UserRegistrationRequestModel user, string role)
        {
            try
            {
                var addedUser = await _user.UserRegistration(user, role);
                if (addedUser)
                {
                    var response = new ResponseModel<UserRegistrationRequestModel>
                    {
                        Message = "Registration Successful",
                    };
                    return Ok(response);
                }
                else
                    return BadRequest("invalid input");
            }
            catch (Exception ex)
            {
                if (ex is DuplicateEmailException)
                {
                    var response = new ResponseModel<UserRegistrationRequestModel>
                    {
                        Success = false,
                        Message = ex.Message
                    };
                    return BadRequest(response);
                }
                else if (ex is InvalidEmailFormatException)
                {
                    var response = new ResponseModel<UserRegistrationRequestModel>
                    {
                        Success = false,
                        Message = ex.Message
                    };
                    return BadRequest(response);
                }
                else
                    return StatusCode(500, $"An error occurred while adding the Details: {ex.Message}");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> UserLogin(LoginRequestModel loginRequestModel)
        {
            try
            {
                var token = await _user.UserLogin(loginRequestModel);
                if (token == null)
                {
                    var res = new ResponseModel<LoginRequestModel>
                    {
                        Success = false,
                        Message = "Invalid User Detail "
                    };
                    return BadRequest(res);
                }
                var response = new ResponseModel<string>
                {
                    Message = "Login Sucessfull",
                    Data = token
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex is UserNotFoundException)
                {
                    var response = new ResponseModel<LoginRequestModel>
                    {
                        Success = false,
                        Message = ex.Message
                    };
                    return Conflict(response);
                }
                else
                {
                    return StatusCode(500, $"Error occurred: {ex.Message}");
                }

            }
        }
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(int UserId)
        {
            try
            {
                var user = await _user.GetUserById(UserId);
                if (user != null)
                {
                    var userResponse = new UserResponseModel
                    {
                        UserID = user.UserID,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Role = user.Role
                    };
                    return Ok(userResponse);
                }
                else
                    return NotFound(new { Success = false, Message = "User not found." });
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _user.GetAllUsers();
                if (users != null)
                {
                    var userResponses = users.Select(user => new UserResponseModel
                    {
                        UserID= user.UserID,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Role = user.Role
                    }).ToList();
                    return Ok(new { Success = true, Message = "Users found.", Data = userResponses });
                }
                else
                    return NotFound(new { Success = false, Message = "No users found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }

    }
}
