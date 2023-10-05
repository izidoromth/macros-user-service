using System.ComponentModel.DataAnnotations;

namespace macros_user_service.Model.Request
{
    public class LoginRequest
    {
        [Required]
        public string Identifier { get; set; } = String.Empty;
        [Required]
        public string Password { get; set; } = String.Empty;
    }
}
