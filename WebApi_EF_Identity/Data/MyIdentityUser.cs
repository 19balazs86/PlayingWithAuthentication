using Microsoft.AspNetCore.Identity;

namespace WebApi_EF_Identity.Data;

public sealed class MyIdentityUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
