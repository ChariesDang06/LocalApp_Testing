using AutoMapper;
using LocalEdu_App.Dto;
using LocalEdu_App.Interfaces;
using LocalEdu_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace LocalEdu_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAppUserRopsitory _appUserRopsitory;
        private readonly IMapper _mapper;
        IConfiguration _configuration;

        public AccountController(IAppUserRopsitory appUserRopsitory, IMapper mapper, IConfiguration configuration)
        {
            _appUserRopsitory = appUserRopsitory;
            _mapper = mapper;
            _configuration = configuration;
        }
        [HttpPost]
        [Route("PostLoginDetails")]
        public async Task<IActionResult> PostLoginDetails(AppUserDto _userData)
        {
            if (_userData != null)
            {
                var resultLoginCheck = _appUserRopsitory.AppUserExist(_userData.Id);
                if (resultLoginCheck == null)
                {
                    return BadRequest("Invalid Credentials");
                }
                else
                {
                    _userData.AccessToken = GetToken(_userData);
                    return Ok(_userData);
                }
            }
            else
            {
                return BadRequest("No Data Posted");
            }
        }

        [NonAction]
        public string GetToken(AppUserDto _userData)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("UserId", _userData.Id.ToString()),
                new Claim("DisplayName", _userData.FirstName),
                new Claim("Username", _userData.AppUserName),
                new Claim("Email", _userData.Email),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: signIn);

            string Token = new JwtSecurityTokenHandler().WriteToken(token);
            return Token;
        }
    }
}
