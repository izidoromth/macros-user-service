using System.ComponentModel.DataAnnotations;

namespace macros_user_service.Model.Request
{
    public class AskChangePasswordRequest
    {
        [Required]
        public string Identifier { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "The password must have at least 8 characters, one number, one special character, one uppercase and one lowercase letter")]
        public string NewPassword { get; set; } = string.Empty;
        [Required]
        [Compare("Password", ErrorMessage = "The passwords informed don't match.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
