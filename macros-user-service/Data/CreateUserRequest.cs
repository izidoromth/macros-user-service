using System.ComponentModel.DataAnnotations;

namespace macros_user_service.Data
{
    public class CreateUserRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "The password must have at least 8 characters, one number, one special character, one uppercase and one lowercase letter")]
        public string Password { get; set; }
    }
}
