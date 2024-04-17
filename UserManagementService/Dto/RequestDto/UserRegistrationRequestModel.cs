using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Dto.RequestDto
{
    public class UserRegistrationRequestModel
    {
        [Required(ErrorMessage = "User First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "User Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }


    }
}
