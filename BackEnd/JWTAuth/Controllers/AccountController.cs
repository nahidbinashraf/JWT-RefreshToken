using JWTAuth.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private static List<User> AllUsers = new List<User>();    

        public AccountController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDTO user)
        {
            if(user.UserName !="" && user.Password != "")
            {
                User addUser = new User();
                addUser.UserName = user.UserName;
                using (var genarateHash = new HMACSHA512())
                {
                    addUser.PasswordSalt = genarateHash.Key;
                    addUser.PasswordHash = genarateHash.ComputeHash(Encoding.UTF8.GetBytes(user.Password));

                    AllUsers.Add(addUser);
                    return Ok(user);
                }

            }
            return BadRequest(user);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDTO user)
        {
            if (user.UserName != "" && user.Password != "")
            {
                //varify user
                User findUser = AllUsers.FirstOrDefault(x => x.UserName.ToLower() == user.UserName.ToLower());

                if (findUser != null)
                {
                    using (var genarateHash = new HMACSHA512(key: findUser.PasswordSalt))
                    {
                        var matchedPasswordHash = genarateHash.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
                        bool matchPassword = matchedPasswordHash.SequenceEqual(findUser.PasswordHash);
                        if (matchPassword)
                        {
                            //Generate Token
                            var token = GenerateToken(findUser);

                            //Generate Refresh Token 
                            RefreshToken refreshToken = GenerateRefreshToken();
                           // setRefreshToken(refreshToken);

                            findUser.RefreshToken = refreshToken.Token;
                            findUser.TokenCreatedAt = refreshToken.TokenCreated;
                            findUser.TokenExpriedOn = refreshToken.TokenExpried;

                            return Ok( new {access_token =  token, refresh_token = refreshToken.Token});

                        }
                        return BadRequest("Password mismatch");
                    }
                }
                return BadRequest(user);
               

            }
            return NotFound(user);
        }

        [HttpGet("GetRefreshToken")]
        public async Task<IActionResult> GetRefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null) { return Unauthorized(); }
            else
            {
                var findTokenUser = AllUsers.FirstOrDefault(x=> x.RefreshToken.Equals(refreshToken));
                if (findTokenUser == null) { return Unauthorized("Not found " + refreshToken); }
                else
                {
                    if (findTokenUser.TokenExpriedOn > System.DateTime.UtcNow) return Unauthorized("Token Expired");
                    else
                    {
                        //Generate Token
                        var token = GenerateToken(findTokenUser);

                        //Generate Refresh Token 
                        RefreshToken generatedRefreshToken = GenerateRefreshToken();
                     //   setRefreshToken(generatedRefreshToken);

                        findTokenUser.RefreshToken = generatedRefreshToken.Token;
                        findTokenUser.TokenCreatedAt = generatedRefreshToken.TokenCreated;
                        findTokenUser.TokenExpriedOn = generatedRefreshToken.TokenExpried;

                        return Ok(new { access_token = token, refresh_token = generatedRefreshToken.Token });

                    }
                }
            }
        }
        private void setRefreshToken(RefreshToken refreshToken)
        {
            var cookieOption = new CookieOptions()
            {
                HttpOnly = true,
                Expires = refreshToken.TokenExpried
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOption);
        }

        private RefreshToken GenerateRefreshToken()
        {
            RefreshToken token = new RefreshToken()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)) ,
                TokenCreated = DateTime.UtcNow,
                TokenExpried = DateTime.UtcNow.AddMinutes(15)
               
            };
            return token;
        }

        private string GenerateToken(User findUser)
        {
            
            List<Claim> claims = new List<Claim>
            {
                new Claim("Name", findUser.UserName),
                new Claim("Role", "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var createToken = new JwtSecurityToken
                (
                    claims: claims,
                    signingCredentials: credential,
                    expires: DateTime.UtcNow.AddMinutes(10)
                );
            var token = new JwtSecurityTokenHandler().WriteToken(createToken);

            return token;
        }


    }
}
