namespace ApiCookieAuth
{
    public sealed class ApiJwtClient : IApiJwtClient
    {
        public const string AccessTokenName = "access_token";

        private readonly HttpClient _httpClient;

        public ApiJwtClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthToken> Login(LoginRequest loginRequest)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/Auth/Login", loginRequest);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<AuthToken>();
        }

        public async Task<UserModel> GetUserModel()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/Auth");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<UserModel>();
        }
    }

    public interface IApiJwtClient
    {
        Task<AuthToken> Login(LoginRequest loginRequest);
        Task<UserModel> GetUserModel();
    }
}
