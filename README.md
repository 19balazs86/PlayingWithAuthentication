# Playing with Authentication

This small application is an example to **set-up the built-in JWT authentication**.

#### In the example

- Save and retrieve the Claims from the Token.
- Create SigningCredentials with symmetric and asymmetric security key.
- Validate token.
- Configure to retrieve the token from the cookie.

#### Basics
- [JWT.io](https://jwt.io)
- [JSON Web Token (JWT) explained.](https://flaviocopes.com/jwt)
- [What is a JSON Web Token?](https://medium.com/myplanet-musings/what-is-a-json-web-token-2193f383e963)
- GitHub: [Jwt.Net](https://github.com/jwt-dotnet/jwt) - Supports generating and decoding JSON Web Tokens.

#### Other resources
- [JWT Authentication Tutorial](https://youtu.be/8FvN5bhVYxY) *(33 min)* | [Raw Coding](https://www.youtube.com/@RawCoding/videos) YouTube channel contains deep dive videos about authentication topic
- [Signing JWT with RSA](https://vmsdurano.com/-net-core-3-1-signing-jwt-with-rsa/) *(ProudMonkey)*
- [Protect your API with API Keys](https://josefottosson.se/asp-net-core-protect-your-api-with-api-keys/) *(Josef)*
- [Certificate Authentication](https://damienbod.com/2019/06/13/certificate-authentication-in-asp-net-core-3-0/) *(Damien Bod)*
- [Creating a custom authentication scheme](https://joonasw.net/view/creating-auth-scheme-in-aspnet-core-2/) (Joonas w)
- [aspnet-contrib / AspNet.Security.OAuth.Providers](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers) *(GitHub)*
- [Presentation](https://www.youtube.com/watch?v=dDroEVdAqKM) about Authentication and Authorization. [Source code](https://github.com/blowdart/AuthNAuthZPresentationDemos) for the demo. *(Barry Dorrans)*
- [Azure AD Authentication](https://www.faciletechnolab.com/blog/2021/4/13/how-to-implement-azure-ad-authentication-in-aspnet-core-50-web-application) *(Facile Technolab)*
- [Combining JWT and Cookie Authentication](https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET) *(RickStrahl)*
- [What's New in .NET 7 for Authentication and Authorization](https://auth0.com/blog/whats-new-in-dotnet-7-for-authentication-and-authorization) *(auth0)*

##### Password hashing
- [Hashing and Salting passwords best practices](https://code-maze.com/csharp-hashing-salting-passwords-best-practices/) *(CodeMaze)* - [PBKDF2](Playing_with_JWT/Playing_with_JWT/Hashing_PBKDF2.cs), [BCrypt/SCrypt](https://github.com/BcryptNet/bcrypt.net), Argon2
- [How to store a password](https://www.meziantou.net/how-to-store-a-password-in-a-web-application.htm) *(meziantou)*
- [Cryptography in .NET](https://www.meziantou.net/cryptography-in-dotnet.htm) *(meziantou)*

