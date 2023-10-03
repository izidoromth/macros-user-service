using macros_user_service.Contexts;
using macros_user_service.Data;
using macros_user_service.Data.Response;
using macros_user_service.Entity;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Diagnostics;
using System.Net.Mail;
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
        [Route("check")]
        public ActionResult<string> CheckUserService()
        {
            return Ok(new BaseResponse() { Message = $"The service is up and running since {Process.GetCurrentProcess().StartTime.ToString("MM/dd/yyyy H:mm:ss")}"});
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<User>>> CreateUser([FromBody] CreateUserRequest body)
        {
            var time = DateTime.Now;

            User? sameUsernameUser = await _context.Users.FirstOrDefaultAsync(i => i.Username == body.Username);

            if (sameUsernameUser != null)
            {
                return BadRequest(new BaseResponse() { Message = "A user with the informed username is already registered." });
            }

            User? sameEmailUser = await _context.Users.FirstOrDefaultAsync(i => i.Email == body.Email);

            if (sameEmailUser != null)
            {
                return BadRequest(new BaseResponse() { Message = "A user with the informed e-mail is already registered." });
            }

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
            try
            {
                _context.Users.Add(user);
                _context.Passwords.Add(password);

                int entries = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse() { Message = "Something occurred while saving the changes to the database. Check the request or try again later." });
            }

            return CreatedAtAction(nameof(CreateUser), new BaseResponse<User>() { Message = "Success.", Content = user });
        }
    }
}
