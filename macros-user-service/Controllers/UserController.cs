using macros_user_service.Contexts;
using macros_user_service.Model.Request;
using macros_user_service.Model.Response;
using macros_user_service.Entity;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
//using System.Net.Mail;
using System.Security.Cryptography;
//using MimeKit;
//using System.Net;
//using System;

namespace macros_user_service.Controllers
{
    [ApiController]
    [Route("/")]
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
        [Route("create")]
        public async Task<ActionResult<BaseResponse<User>>> CreateUser([FromBody] CreateUserRequest req)
        {
            var time = DateTime.Now;

            User? sameUsernameUser = await _context.Users.FirstOrDefaultAsync(i => i.Username == req.Username);

            if (sameUsernameUser != null)
            {
                return BadRequest(new BaseResponse() { Message = "A user with the informed username is already registered." });
            }

            User? sameEmailUser = await _context.Users.FirstOrDefaultAsync(i => i.Email == req.Email);

            if (sameEmailUser != null)
            {
                return BadRequest(new BaseResponse() { Message = "A user with the informed e-mail is already registered." });
            }

            User user = new User()
            {
                Username = req.Username,
                Email = req.Email,
                FirstName = req.FirstName,
                LastName = req.LastName,
            };

            byte[] salt = RandomNumberGenerator.GetBytes(16);
            Password password = new Password()
            {
                Hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(req.Password, salt, KeyDerivationPrf.HMACSHA256, 1000000, 32)),
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
            catch
            {
                return BadRequest(new BaseResponse() { Message = "Something occurred while saving the changes to the database. Check the request or try again later." });
            }

            return Ok(new BaseResponse<User>() { Message = "Success.", Content = user });
        }



        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<BaseResponse>> Login([FromBody] LoginRequest req)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Username == req.Identifier || u.Email == req.Identifier);
            if (user == null)
            {
                return BadRequest(new BaseResponse() { Message = "It seems that there's no account associated with the username informed." });
            }

            Password? activePassword = await _context.Passwords.FirstOrDefaultAsync(p => p.Active && p.UserId == user.UserId);
            if(activePassword == null)
            {
                return BadRequest(new BaseResponse() { Message = "The user seems to not have an active password." });
            }

            if (activePassword.Hash == Convert.ToBase64String(KeyDerivation.Pbkdf2(req.Password, activePassword.Salt, KeyDerivationPrf.HMACSHA256, 1000000, 32)))
            {
                return Ok(new BaseResponse<User>() { Message = "User authenticated.", Content = user });
            }
            else
            {
                return BadRequest(new BaseResponse() { Message = "Wrong password." });
            }
        }

        //[HttpPost]
        //[Route("/password/ask")]
        //public async Task<ActionResult<BaseResponse>> AskChangePassword([FromBody] AskChangePasswordRequest req)
        //{
        //    var email = new MailMessage();

        //    email.From = new MailAddress("sender@email.com", "Sender Name");
        //    email.To = "asd"; // = new MailAddressCollection() { new MailAddress("", "") };
        //        //("receiver@email.com", "Receiver Name");

        //    email.Subject = "Testing out email sending";
        //    email.Body = HttpContent(url);
        //    email.BodyFormat = MailFormat.Html;
        //}

        ////screen scrape a page here
        //private string HttpContent(string url)
        //{
        //    WebRequest objRequest = System.Net.HttpWebRequest.Create(url);
        //    StreamReader sr = new StreamReader(objRequest.GetResponse().GetResponseStream());
        //    string result = sr.ReadToEnd();
        //    sr.Close();
        //    return result;
        //}

        //[HttpPost]
        //[Route("/password/reset")]
        //public async Task<ActionResult<BaseResponse>> ChangePassword([FromBody] ChangePasswordRequest req)
        //{
        //    //User? user = await _context.Users.FirstOrDefaultAsync(u => u.Username == req.Identifier || u.Email == req.Identifier);

        //    //if(user == null)
        //    //{
        //    //    return BadRequest(new BaseResponse { Message = "No user was found with the informed identifier." });
        //    //}

        //    List<Password> oldPasswords = _context.Passwords.Where(p => p.UserId == user.UserId).ToList();

        //    foreach(Password oldPassword in oldPasswords)
        //    {
        //        string newPasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(req.NewPassword, oldPassword.Salt, KeyDerivationPrf.HMACSHA256, 1000000, 32));
        //        if (newPasswordHash == oldPassword.Hash)
        //        {
        //            return BadRequest(new BaseResponse { Message = "The password informed was already used previoulsy. You need a new password to complete the request." });
        //        }
        //    }

        //    //Password? oldPassword = _context.Passwords.FirstOrDefault(p => p.Active && p.UserId == user.UserId);

        //}
    }
}
