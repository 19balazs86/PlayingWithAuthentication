using ApiJWT.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiJWT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(LoginRequest loginModel)
        {
            if (loginModel.Name == "test" && loginModel.Password == "pass")
            {
                var user = new UserModel(1, loginModel.Name, new string[] { loginModel.Role! });

                return Ok(new { Token = AuthHelper.CreateToken(user.ToClaims()) });
            }

            return Unauthorized();
        }

        [HttpGet]
        public UserModel Get() => new UserModel(User.Claims);

        //[Authorize(Roles = "Admin")] // This can be used too.
        [Authorize(Policy = "Admin")]
        [HttpGet("Admin")]
        public UserModel GetAdmin() => new UserModel(User.Claims);

        [AllowAnonymous]
        [HttpPost("ValidateToken")]
        public ActionResult<UserModel> ValidateToken(ValidateTokenRequest tokenRequest)
        {
            if (AuthHelper.TryValidateToken(tokenRequest.Token, out var claimsPrincipal))
            {
                var user = new UserModel(claimsPrincipal.Claims);

                return Ok(user);
            }

            return BadRequest("Invalid token");
        }
    }

    public record ValidateTokenRequest(string Token);
}