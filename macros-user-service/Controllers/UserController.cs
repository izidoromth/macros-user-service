using macros_user_service.Contexts;
using macros_user_service.Data;
using macros_user_service.Data.Response;
using macros_user_service.Entity;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace macros_user_service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;

        public UserController(UserDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public ActionResult<string> ReadUser()
        {
            return Ok("Vivasso");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<User>>> CreateUser([FromBody] CreateUserRequest body)
        {
            DateTime time = DateTime.Now;
            User? sameUsernameUser = await _context.Users.FirstOrDefaultAsync(i => i.Username == body.Username);

            if(sameUsernameUser != null)
            {
                return BadRequest(new BaseResponse() { Message = "A user with the informed username is already registered." });
            }
            Console.WriteLine($"Username check: {(DateTime.Now - time).TotalSeconds}");
            
            time = DateTime.Now;
            User? sameEmailUser = await _context.Users.FirstOrDefaultAsync(i => i.Email == body.Email);

            if (sameEmailUser != null)
            {
                return BadRequest(new BaseResponse() { Message = "A user with the informed e-mail is already registered." });
            }
            Console.WriteLine($"Email check: {(DateTime.Now - time).TotalSeconds}");

            User user = new User()
            {
                Username = body.Username,
                Email = body.Email,
                FirstName = body.FirstName,
                LastName = body.LastName,
            };

            byte[] salt = RandomNumberGenerator.GetBytes(16);
            Password password = new Password()
            {
                Hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(body.Password, salt, KeyDerivationPrf.HMACSHA256, 1000000, 32)),
                Active = true,
                Salt = salt,
                UserId = user.UserId,
                CreatedAt = DateTime.UtcNow,
            };
            time = DateTime.Now;
            _context.Users.Add(user);
            Console.WriteLine($"User insert: {(DateTime.Now - time).TotalSeconds}");

            time = DateTime.Now;
            _context.Passwords.Add(password);
            Console.WriteLine($"Password insert: {(DateTime.Now - time).TotalSeconds}");

            time = DateTime.Now;
            int entries = await _context.SaveChangesAsync();
            Console.WriteLine($"Db save: {(DateTime.Now - time).TotalSeconds}");

            Console.WriteLine($"Entries: {entries}");

            return CreatedAtAction(nameof(CreateUser), new BaseResponse<User>() { Message = "Success.", Content = user });
        }
    }
}
