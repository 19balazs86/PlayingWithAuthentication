# BlazorWASM Auth0 JWT OpenIDConnect

- An example of using [Auth0](https://auth0.com) with OpenIDConnect in a Blazor WebAssembly application.
- You can compare this solution with [Blazor WASM BFF architecture](../BlazorWASM.BFF.Auth0.OpenIDConnect).
- The solution was created using the default Blazor WASM template. The client is customized for OpenIDConnect using Auth0. Server is configured for JWT token issued by Auth0.

## Deviation from the default template

`Client`
- Program.cs: using AddOidcAuthentication and AddAuthorizationCore instead of AddApiAuthorization
- Authentication.razor: Replace the default LogOut in the RemoteAuthenticatorView

`Server`
- Program.cs: using AddJwtBearer instead of the IdentityServer

## Prerequisite to run the application

- Auth0 account #1: create a SPA application
  - Allowed Callback URL: https://localhost:7209/authentication/login-callback
    - Swagger Oauth flow callback URL: https://localhost:7209/swagger/oauth2-redirect.html
  - Allowed Logout URL: https://localhost:7209
- Auth0 account #2: create a custom API, the value of identifier will be used as the audience parameter on authorization calls
- [Social connections](https://marketplace.auth0.com/features/social-connections) can be added with the redirect URL: https://Your-Auth0-Domain/login/callback
- Client/wwwroot/appsettings.json

```json
"OidcConfig": {
    "Authority": "Your SPA Domain",
    "ClientId": "Your SPA ClientId",
    "MetadataUrl": "Your SPA Domain/.well-known/openid-configuration",
    "Audience": "Your Custom API identifier"
}
```
- Server/appsettings.json

```json
"OidcConfig": {
    "Authority": "Your SPA Domain",
    "ClientId": "Not used",
    "MetadataUrl": "Your SPA Domain/.well-known/openid-configuration",
    "Audience": "Your Custom API identifier"
}
```

## Auth0 - Custom Action

- Add this custom action to the login flow

```js
exports.onExecutePostLogin = async (event, api) => {
  if (event.authorization)
  {
    // Difference between ID Token and Access Token
    // - Blog:  https://auth0.com/blog/id-token-access-token-what-is-the-difference
    // - Video: https://youtu.be/vVM1Tpu9QB4

    // ID Token is used by the Blazor frontend
    api.idToken.setCustomClaim("role", event.authorization.roles);
    
    // Access Token is passed to the API server by the Blazor frontend
    // Permissions are included in the token by default
    api.accessToken.setCustomClaim("role", event.authorization.roles);
    api.accessToken.setCustomClaim("email", event.user.email);
  }
};
```

## Resources

#### Auth0

- [Configure ASP.NET Web API for JWT](https://auth0.com/docs/quickstart/backend/aspnet-core-webapi) 📓
- [Auth0 - Social Connections](https://marketplace.auth0.com/features/social-connections) 📓
- [Auth0 Templates for .NET](https://github.com/auth0/auth0-dotnet-templates) 👤*templates to create new projects tailored for Auth0 authentication*

#### Miscellaneous
- [Auth0 in ASP.NET + Blazor WASM](https://timmoth.com/posts/H9zMzMcBkUe_QfCAo0kx_Q) 📓*Tim*
- [Blazor WASM with Google authorization](https://www.telerik.com/blogs/create-webassembly-app-blazor-google-authorization) 📓*Telerik-Blogs*

![Screenshot](Screenshot.JPG)