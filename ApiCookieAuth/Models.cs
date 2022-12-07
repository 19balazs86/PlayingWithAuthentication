namespace ApiCookieAuth
{
    public sealed class AuthToken
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }

    public sealed class LoginRequest
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public sealed class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
    }
}
