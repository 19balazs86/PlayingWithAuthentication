namespace ApiCookieAuth
{
    public sealed record AuthToken
    {
        public string Token { get; init; }
        public string RefreshToken { get; init; }
    }

    public sealed record LoginRequest
    {
        public string Name { get; init; }
        public string Password { get; init; }
    }

    public sealed record UserModel
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public IEnumerable<string> Roles { get; init; } = Enumerable.Empty<string>();
    }
}
