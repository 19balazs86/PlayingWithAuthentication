using ApiJWT.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace ApiJWT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("Login")]
        public ActionResult<AuthToken> Login(LoginRequest loginModel)
        {
            if (loginModel.Name == "test" && loginModel.Password == "pass")
            {
                var user = new UserModel(1, loginModel.Name, new string[] { loginModel.Role! });

                string refreshTokenKey = RefreshTokenRepository.CreateRefreshToken(user.JwtId);

                string token = AuthHelper.CreateToken(user.ToClaims());

                return Ok(new AuthToken(token, refreshTokenKey));
            }

            return Unauthorized();
        }

        [HttpGet]
        public UserModel Get()
        {
            return new UserModel(User.Claims);
        }

        //[Authorize(Roles = "Admin")] // This can be used too.
        [Authorize(Policy = "Admin")]
        [HttpGet("Admin")]
        public UserModel GetAdmin() => new UserModel(User.Claims);

        [AllowAnonymous]
        [HttpPost("ValidateToken")]
        public ActionResult<UserModel> ValidateToken(ValidateToken tokenRequest)
        {
            if (AuthHelper.TryValidateToken(tokenRequest.Token, out var claimsPrincipal, out string? invalidReason))
            {
                var user = new UserModel(claimsPrincipal.Claims);

                return Ok(user);
            }

            return Problem(title: "Invalid token", detail: invalidReason, statusCode: Status400BadRequest);
        }

        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public ActionResult<AuthToken> RefreshTokenAsync(AuthToken tokenRequest)
        {
            if (AuthHelper.TryValidateExpiredToken(tokenRequest.Token, out JwtSecurityToken? jwtSecurityToken, out string? invalidReason))
            {
                //if (jwtSecurityToken.ValidTo > DateTime.UtcNow)
                //    return Problem(title: "The token is not expired yet.", detail: invalidReason, statusCode: Status400BadRequest);

                var user = new UserModel(jwtSecurityToken.Claims);

                // user.JwtId == securityToken.Id // because of wtRegisteredClaimNames.Jti

                if (RefreshTokenRepository.TryInvalidateRefreshToken(user.JwtId, tokenRequest.RefreshToken))
                {
                    user.JwtId = Guid.NewGuid().ToString();

                    string refreshTokenKey = RefreshTokenRepository.CreateRefreshToken(user.JwtId);

                    string token = AuthHelper.CreateToken(user.ToClaims());

                    return Ok(new AuthToken(token, refreshTokenKey));
                }
            }

            return Problem(title: "Invalid refresh token", detail: invalidReason, statusCode: Status400BadRequest);
        }
    }

    public record ValidateToken(string Token);
    public record AuthToken(string Token, string RefreshToken);
}