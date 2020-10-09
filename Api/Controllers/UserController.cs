using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Api.Models;
using Microsoft.Extensions.Logging;
using Service;
using Api.Engine;
using Api.Helpers;

namespace Api.Controllers
{
    [ApiController]
    public class UserController : BaseController
    {
        private IConfiguration _config;
        private ILogger<UserController> _logging;
        private IUserService _userService;
        private IWorkContext _workContext;

        public UserController(IUserService userService, IConfiguration config, IWorkContext workContext, ILogger<UserController> logging)
        {
            _config = config;
            _logging = logging;
            _userService = userService;
            _workContext = workContext;
        }

        [HttpPost]
        public IActionResult Login([FromBody]LoginModel login)
        {
            var (user, errorMessage) = _userService.GetUserForLogin(login.UserName, login.Password);

            if (user != null)
            {
                _userService.SetUserPublicClient(login.ClientPublicKey, user.Id);
                var tokenString = GenerateJSONWebToken(user.UserName, user.Id, user.FirstName, user.LastName, user.Email, login.ClientPublicKey);
                return Ok(new { token = tokenString, serverPublicKey = CryptoHelper.PulicKeyString });
            }

            return BadRequest(errorMessage);
        }

        [HttpPost]
        public IActionResult Register([FromBody]RegisterUserModel userModel)
        {
            var (user, errorMessage) = _userService.RegisterUser(userModel.UserName, userModel.Password, userModel.FirstName, userModel.LastName, userModel.Email);

            if (user)
            {
                return Ok(new { status = true, serverPublicKey = CryptoHelper.PulicKeyString });
            }

            return BadRequest(errorMessage);
        }

        [HttpGet]
        public IActionResult GetServerPublicKey()
        {
            return Ok(new { serverPublicKey = CryptoHelper.PulicKeyString });
        }

        [Authorize]
        [HttpPost]
        public IActionResult GetUserDetail([FromBody]BaseModel model)
        {

            var (user, errorMessage) = _userService.GetUser(model.Id ?? _workContext.UserId);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            var retval = new UserModel { Id = user.Id, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, Password = user.Password, UserName = user.UserName };

            return Ok(retval);
        }

        /*

        örnek şifreleme metodları !!!!
        şifreleme metodunun örneklerini incele ok. aşağıda!
        */
        [HttpPost]
        public IActionResult EncriptDecriptTest([FromBody]RegisterUserModel userModel)
        {
            var retval = new RegisterUserModel();
            retval.LastName = CryptoHelper.Encrypt(userModel.FirstName);
            retval.UserName = CryptoHelper.Decrypt(retval.LastName);

            //// token alıp denersen aşağıdakini aç !! 
            ///  client tokenları gönderdiğin metodlarıda hazırladım.
            ////retval.Password = CryptoHelper.EncryptForClient("clientDatası", _workContext.ClientPublicKey);

            return Ok(retval);
        }


        private string GenerateJSONWebToken(string userName, string userId, string firstName, string lastName, string email, string clientPublicKey)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim("UserId", userId),
                new Claim("UserName", userName),
                new Claim("FirstName", firstName),
                new Claim("LastName", lastName),
                new Claim("Email", email),
                new Claim("ClientPublicKey",clientPublicKey),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
