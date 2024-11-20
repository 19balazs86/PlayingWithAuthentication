# Blazor Web Application with OIDC

- An example of using OpenID Connect in a Blazor web application
- With .NET 9, Blazor now features a built-in authentication state serialization between the server and client
- There are templates available for creating web applications with authentication via Entity Framework, which can be customized with additional providers
- Microsoft offers examples of web applications using OpenID Connect, both with and without the BFF pattern
- In my example, I modified the no-authentication template to incorporate [Auth0](https://auth0.com) OIDC
- No global interactivity is set, allowing the project to test authentication across multiple interactive pages or components

## Prerequisite to run the application

- Auth0 account, where the SPA Application is configured
  - Allowed Callback URLs: https://localhost:7279/signin-oidc
  - Allowed Logout URLs: https://localhost:7279
- appsettings.json

```json
"OidcConfig": {
    "Authority": "Your SPA Domain",
    "ClientId": "Your SPA ClientId",
    "MetadataUrl": "Your SPA Domain/.well-known/openid-configuration",
    "Audience": "Your Custom API identifier"
}
```

## Resources

- [Blazor Web App with OIDC](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/blazor-web-app-with-oidc) *(with and without the BFF pattern)* | [Sample code](https://github.com/dotnet/blazor-samples/tree/main/9.0/BlazorWebAppOidc) 📚*MS-learn*
- [Secure server-side Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/server) | [Secure WASM Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly) 📚*MS-learn*
- [Accessing UserIdentity on Blazor 8](https://blog.lhotka.net/2024/10/13/Accessing-User-Identity-on-a-Blazor-Wasm-Client) *(Server and WASM)*📓*Rockford Lhotka*