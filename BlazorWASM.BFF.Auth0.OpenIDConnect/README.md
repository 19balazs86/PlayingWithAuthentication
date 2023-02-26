# BlazorWASM.BFF.Auth0.OpenIDConnect

An example of using [Auth0](https://auth0.com) with OpenIDConnect in a Blazor WebAssembly application that has a Backend For Frontend (BFF) architecture.

Damien’s template is used to create and customize three projects: Client, Server and Shared and customized for Auth0.

###### Prerequisite to run the application

- Auth0 account, where the Application is configured with the callback URL: https://localhost:5301/callback
- Social connections can be added with the redirect URL: https://YourTenantDomain/login/callback
- appsettings.json

```json
"Authentication": {
    "Auth0": {
      "Domain": "Your Auth0 Domain",
      "ClientId": "Your Auth0 Application's ClientId"
    }
  }
```


###### Resources

- [Blazor.BFF.OpenIDConnect.Template](https://github.com/damienbod/Blazor.BFF.OpenIDConnect.Template) 👤*DamienBod* - This template is used as a base and customized it for Auth0
- [Overview of different App security topics](https://github.com/damienbod/aspnetcore-standup-authn-authz) 👤*DamienBod*
- [OpenIddict](https://documentation.openiddict.com) 📓*Official doc*
- [OpenIddict-Core](https://github.com/openiddict/openiddict-core) 👤*Official repo*
- [Configure Auth0 in ASP.NET application with cookies](https://auth0.com/docs/quickstart/webapp/aspnet-core) 📓*Auth0 doc*
