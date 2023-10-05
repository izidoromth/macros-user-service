using System.ComponentModel.DataAnnotations;

namespace macros_user_service.Model.Request
{
    public class CreateUserRequest
    {
        [Required]
        public string Username { get; set; } = String.Empty;
        [Required]
        public string FirstName { get; set; } = String.Empty;
        [Required]
        public string LastName { get; set; } = String.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = String.Empty;
        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "The password must have at least 8 characters, one number, one special character, one uppercase and one lowercase letter")]
        public string Password { get; set; } = String.Empty;
        [Required]
        //[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "The password must have at least 8 characters, one number, one special character, one uppercase and one lowercase letter")]
        [Compare("Password", ErrorMessage = "The passwords informed don't match.")]
        public string ConfirmPassword { get; set; } = String.Empty;
    }
}
