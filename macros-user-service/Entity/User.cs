using System.ComponentModel.DataAnnotations;

namespace macros_user_service.Entity
{
    public class User
    {
        public string UserId { get; set;} = Guid.NewGuid().ToString();
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
