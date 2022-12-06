using System.ComponentModel.DataAnnotations;

namespace ApiJWT.Model
{
    public class LoginRequest
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public string? Role { get; set; }
    }
}
