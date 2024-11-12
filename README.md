# Playing with Authentication

Table of content

- Project: [KeyGenRSA](#project-keygenrsa)
- Project: [ApiJWT](#project-apijwt)
- Project: [ApiCookieAuth](#project-apicookieauth)
- Project: [WebApi_EF_Identity](#project-webapi_ef_identity)
- Project: [WebApi_EF_Identity_BearerNET8](#project-webapi_ef_identity_bearernet8)
- Project: [ApiKeyAuth](#project-apikeyauth)
- Project: [Blazor WASM BFF - Auth0 - OpenIDConnect](#project-blazorwasmbffauth0openidconnect)
- Project: [Blazor WASM JWT - Auth0 - OpenIDConnect](#project-blazorwasmauth0jwtopenidconnect)
- [Other resources](#other-resources)
- [Password hashing](#password-hashing)

---

### `Project: KeyGenRSA`

- Console application to generate private and public RSA pem files.
- Private key is used in the ApiJWT project to sign and validate JWT.
- Public key can be used to validate an ApiJWT token. It can be an RsaSecurityKey during authentication process passing the ApiJWT token.
- Hashing and Salting password with [PBKDF2](KeyGenRSA/Hashing_PBKDF2.cs).

###### Resources

- [Hashing and Salting passwords best practices](https://code-maze.com/csharp-hashing-salting-passwords-best-practices/) 📓*Code-Maze* - [PBKDF2](KeyGenRSA/Hashing_PBKDF2.cs)| [BCrypt/SCrypt](https://github.com/BcryptNet/bcrypt.net) |Argon2 | [Bouncy Castle cryptography library](https://code-maze.com/csharp-bouncy-castle-cryptography)

### `Project: ApiJWT`

- WebAPI using JWT authentication, signing the token with the RSA private key.
- Create SigningCredentials with symmetric and asymmetric security key using RSA or X509Certificate.
- Implement a method for refreshing the token.
- Implement a method for invalidating the token.

###### Resources

- [user-jwts](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn) 📚*MS-Learn*
- [JWT Authentication](https://youtu.be/8FvN5bhVYxY) 📽️*33min* | [Raw Coding](https://www.youtube.com/@RawCoding/videos) YouTube channel contains deep dive videos into authentication topic
- [Signing JWT with RSA](https://vmsdurano.com/-net-core-3-1-signing-jwt-with-rsa/) 📓*ProudMonkey*
- [New default Authentication Scheme in .NET 7](https://auth0.com/blog/whats-new-in-dotnet-7-for-authentication-and-authorization) 📓*auth0* | [Setup JWT Bearer token](https://wildermuth.com/2022/12/07/changes-in-jwt-bearer-tokens-in-dotnet-7/) 📓*ShawnWildermuth*
- [Implement multiple JWTs for multi-token authentication](https://youtu.be/EcozgfDOq4g) 📽️*17 min - Milan*

### `Project: ApiCookieAuth`

- A simple WebAPI with a single html file. This is not a comprehensive example, just a checking the basics of cookie authentication.
- Black-list: Implemented a solution to invalidate or reject a cookie based on the session ID.
- Example of using ClaimsTransformation
###### Resources
- [ASP.NET Core Cookie Authentication](https://youtu.be/hw2B6SZj8y8) 📽️*46min-RawCoding*
- [Cookie invalidation and Token revocation](https://youtu.be/R6r_uSSIzvs) 📽️*13min - Raw Coding*
- [Claims transformation for flexible Authorization](https://www.milanjovanovic.tech/blog/master-claims-transformation-for-flexible-aspnetcore-authorization) 📓*Milan* | [Video](https://youtu.be/cgjifZF8ZME) 📽️*14min - Milan*

### `Project: WebApi_EF_Identity`

- Take advantage of the Entity Framework Identity features, including UserManager and SignInManager.
- Two Factor Authentication with AuthenticatorApp or Email
  - First: Register -> Confirm email
  - Flow #1: Get TwoFactor auth setup for Email -> Enable Email-TwoFactor with the given code -> Logout -> Login with user+pass -> Login with Email-TwoFactor with the given code (you got it after email+pass login)
  - Flow #2: Get TwoFactor auth setup for AuthenticatorApp by scanning  the QR code -> Enable Authenticator-TwoFactor with a code -> Logout -> Login with user+pass -> Login with Authenticator-TwoFactor with a code
- Generating a short-lived token for signing in like Slack and Medium
- Recovery codes for 2FA can be generated. After logging in with your username and password, you can use one of these codes instead of the authenticator code.

###### Resources

- [Two Factor Authentication with Web API and Angular using Google Authenticator](https://code-maze.com/dotnet-angular-two-factor-authentication-with-using-google-authenticator) 📓*Code-Maze*
- [QR code generator](https://goqr.me/api) 📓*WebAPI*
- [Implementing custom token provider for short-lived token](https://andrewlock.net/implementing-custom-token-providers-for-passwordless-authentication-in-asp-net-core-identity) 📓*Andrew Lock*

### `Project: WebApi_EF_Identity_BearerNET8`

NET.8 introduced a new authentication method called BearerToken with new Identity endpoints (register, login).
Just a few lines of code, you can have user management endpoints using Entity Framework.
However, it is not suitable for custom needs and is more appropriate for local demo purposes.
You can learn from the code to write your custom logic.
See the [WebApi_EF_Identity](#project-webapi_ef_identity) project.

###### Resources

- [Authentication made easy with Identity in .NET 8](https://youtu.be/S0RSsHKiD6Y) 📽*10min - Milan*
- [New .NET 8 Authentication features](https://youtu.be/XBV1gZNF_S8) 📽*20min - Anton/Raw Coding*
- [Should you use the .NET 8 Identity API endpoints?](https://andrewlock.net/should-you-use-the-dotnet-8-identity-api-endpoints) 📓*Andrew Lock*
- [JWT vs Opaque Tokens](https://medium.com/identity-beyond-borders/jwt-vs-opaque-tokens-all-you-need-to-know-307bf19bade8) 📓*Medium*

### `Project: ApiKeyAuth`

The following solutions have been implemented
1. Use a custom middleware to check the API Key
2. Add an authorization filter for all endpoints of the Controller
3. Apply an authorization filter individually *(controller and/or endpoint level)* with an attribute
4. Add an endpoint filter for minimal API
5. Add a custom authentication handler and use the [Authorize] attribute

###### Resources

- [API Key Authentication](https://youtu.be/GrJJXixjR8M) 📽️*18m - Nick Chapsas*
- [Protect your API with API Keys](https://josefottosson.se/asp-net-core-protect-your-api-with-api-keys/) 📓*Josef Ottosson - Custom authentication handler with roles*
- [Creating a custom authentication scheme](https://joonasw.net/view/creating-auth-scheme-in-aspnet-core-2/) 📓*JoonasW - BasicAuthentication*

### `Project: BlazorWASM.BFF.Auth0.OpenIDConnect`

- An example of using [Auth0](https://auth0.com) with OpenIDConnect in a Blazor WebAssembly application that has a Backend For Frontend (BFF) architecture.
- [Damien’s template](https://github.com/damienbod/Blazor.BFF.OpenIDConnect.Template) is used to create 3 projects: Client, Server and Shared and customized for Auth0.
- [For more information](BlazorWASM.BFF.Auth0.OpenIDConnect)

### `Project: BlazorWASM.Auth0.JWT.OpenIDConnect`

- An example of using [Auth0](https://auth0.com) with OpenIDConnect in a Blazor WebAssembly application.
- [For more information](BlazorWASM.Auth0.JWT.OpenIDConnect)

##### Other resources

- [SimpleAuthentication](https://github.com/marcominerva/SimpleAuthentication) 👤*Marco Minerva*
- [Overview of different App security topics](https://github.com/damienbod/aspnetcore-standup-authn-authz) 👤*DamienBod*
- [Certificate Authentication](https://damienbod.com/2019/06/13/certificate-authentication-in-asp-net-core-3-0/) 📓*DamienBod*
- [aspnet-contrib / AspNet.Security.OAuth.Providers](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers) 👤
- [Azure AD Authentication](https://www.faciletechnolab.com/blog/2021/4/13/how-to-implement-azure-ad-authentication-in-aspnet-core-50-web-application) 📓*FacileTechnolab*
- [Combining JWT and Cookie Authentication](https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET) 📓*RickStrahl*
- [What's New in .NET 7 for Authentication and Authorization](https://auth0.com/blog/whats-new-in-dotnet-7-for-authentication-and-authorization) 📓*auth0*
- [Flexible authorization](https://youtu.be/TuG0yKf8RSQ) 📽️*35m - Jason Taylor - NDC Oslo 2023*
- [Blazor Authentication and Authorization with Identity](https://youtu.be/tNzSuwV62Lw) 📽️*Patrick God*

##### Password hashing

- [How to store a password](https://www.meziantou.net/how-to-store-a-password-in-a-web-application.htm) 📓*meziantou*
- [Cryptography in .NET](https://www.meziantou.net/cryptography-in-dotnet.htm) 📓*meziantou*
- [Cryptography Implementations in .NET](https://code-maze.com/dotnet-cryptography-implementations/) 📓*Code-Maze*
- [How does the default password hasher work](https://code-maze.com/aspnetcore-default-asp-net-core-identity-password-hasher) 📓*Code-Maze*