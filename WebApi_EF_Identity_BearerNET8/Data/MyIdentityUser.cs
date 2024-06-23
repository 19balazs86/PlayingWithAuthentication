using Microsoft.AspNetCore.Identity;

namespace WebApi_EF_Identity_BearerNET8.Data;

public sealed class MyIdentityUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
