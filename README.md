# Playing with Authentication

##### Project: KeyGenRSA

- Console application to generate private and public RSA pem files.
- Private key is used in the ApiJWT project to sign and validate JWT.
- Public key can be used to validate an ApiJWT token. It can be an RsaSecurityKey during authentication process passing the ApiJWT token.
- Hashing and Salting password with [PBKDF2](KeyGenRSA/Hashing_PBKDF2.cs).
###### Resources
- [Hashing and Salting passwords best practices](https://code-maze.com/csharp-hashing-salting-passwords-best-practices/) *(CodeMaze)* - [PBKDF2](KeyGenRSA/Hashing_PBKDF2.cs), [BCrypt/SCrypt](https://github.com/BcryptNet/bcrypt.net), Argon2

##### Project: ApiJWT

- WebAPI using JWT authentication signing the token with the RSA private key.
- Create SigningCredentials with symmetric and asymmetric security key using RSA or X509Certificate.
- Apply a way to refresh the token.

###### Resources
- [JWT Authentication](https://youtu.be/8FvN5bhVYxY) *(33 min)* | [Raw Coding](https://www.youtube.com/@RawCoding/videos) YouTube channel contains deep dive videos into authentication topic
- [Signing JWT with RSA](https://vmsdurano.com/-net-core-3-1-signing-jwt-with-rsa/) *(ProudMonkey)*
- [What's new in .NET 7 for Authentication](https://auth0.com/blog/whats-new-in-dotnet-7-for-authentication-and-authorization/) | [Setup JWT Bearer token](https://wildermuth.com/2022/12/07/changes-in-jwt-bearer-tokens-in-dotnet-7/) *(Shawn Wildermuth)* | [user-jwts](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)

##### Project: ApiCookieAuth

- Implements a cookie authentication. Calling ApiJWT service to get a token and store it in AuthenticationProperties for later usage.
###### Resources
- [ASP.NET Core Cookie Authentication](https://youtu.be/hw2B6SZj8y8) *(46min, Raw Coding)*

##### Other resources

- [Protect your API with API Keys](https://josefottosson.se/asp-net-core-protect-your-api-with-api-keys/) *(Josef)*
- [Certificate Authentication](https://damienbod.com/2019/06/13/certificate-authentication-in-asp-net-core-3-0/) *(Damien Bod)*
- [Creating a custom authentication scheme](https://joonasw.net/view/creating-auth-scheme-in-aspnet-core-2/) (Joonas w)
- [aspnet-contrib / AspNet.Security.OAuth.Providers](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers) *(GitHub)*
- [Azure AD Authentication](https://www.faciletechnolab.com/blog/2021/4/13/how-to-implement-azure-ad-authentication-in-aspnet-core-50-web-application) *(Facile Technolab)*
- [Combining JWT and Cookie Authentication](https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET) *(RickStrahl)*
- [What's New in .NET 7 for Authentication and Authorization](https://auth0.com/blog/whats-new-in-dotnet-7-for-authentication-and-authorization) *(auth0)*

##### Password hashing
- [How to store a password](https://www.meziantou.net/how-to-store-a-password-in-a-web-application.htm) *(meziantou)*
- [Cryptography in .NET](https://www.meziantou.net/cryptography-in-dotnet.htm) *(meziantou)*