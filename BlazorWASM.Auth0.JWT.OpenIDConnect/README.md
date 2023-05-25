# BlazorWASM Auth0 JWT OpenIDConnect

An example of using [Auth0](https://auth0.com) with OpenIDConnect in a Blazor WebAssembly application.

The solution was created using the default Blazor WASM template. The client is customized for OpenIDConnect using Auth0. Server is configured for JWT token issued by Auth0.

*NOTE:* Auth0 has created some [templates](https://github.com/auth0/auth0-dotnet-templates) to create new projects tailored for Auth0 authentication.

You can compare this solution with [Blazor WASM BFF architecture](../BlazorWASM.BFF.Auth0.OpenIDConnect).

###### Deviation from the default template

A few changes had to be applied to make it work:

- Client
  - Program.cs: using AddOidcAuthentication and AddAuthorizationCore instead of AddApiAuthorization
  - Authentication.razor: Replace the default LogOut in the RemoteAuthenticatorView
- Server
  - Program.cs: using AddJwtBearer instead of the IdentityServer


###### Prerequisite to run the application

- Auth0 account, where the SPA Application is configured
  - Allowed Callback URL: https://localhost:7209/authentication/login-callback
  - Allowed Logout URL: https://localhost:7209

- [Social connections](https://marketplace.auth0.com/features/social-connections) can be added with the redirect URL: https://YOUR_DOMAIN/login/callback
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
    "MetadataUrl": "Not used",
    "Audience": "Your Custom API identifier"
}
```
###### Resources

- [Configure ASP.NET Web API for JWT](https://auth0.com/docs/quickstart/backend/aspnet-core-webapi) 📓*Auth0 Docs*
- [Auth0 in ASP.NET + Blazor WASM](https://timmoth.com/posts/H9zMzMcBkUe_QfCAo0kx_Q) 📓*Tim*
- [OpenIddict](https://documentation.openiddict.com) 📓*Official doc*
- [OpenIddict-Core](https://github.com/openiddict/openiddict-core) 👤*Official repo*
- [Auth0 - Social Connections](https://marketplace.auth0.com/features/social-connections) 📓*Auth0 doc*
- [Auth0 Templates for .NET](https://github.com/auth0/auth0-dotnet-templates) 👤*Auth0*
- .NET MAUI
  - [Auth0 Authentication to Blazor Hybrid Apps in .NET MAUI](https://auth0.com/blog/add-authentication-to-blazor-hybrid-apps-in-dotnet-maui) 📓*Auth0 Blog - Andrea Chiarelli*
  - [Managing Tokens in .NET MAUI](https://auth0.com/blog/managing-tokens-in-dotnet-maui/) 📓*Auth0 Blog - Andrea Chiarelli*

![Screenshot](Screenshot.JPG)