using System.ComponentModel.DataAnnotations;

namespace macros_user_service.Entity
{
    public class Password
    {
        public string PasswordId { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }
        [Required]
        public string Hash { get; set; }
        [Required]
        public byte[] Salt { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
