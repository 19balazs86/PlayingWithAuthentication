using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApi_EF_Identity.Data;

public sealed class MyDataContext : IdentityDbContext<MyIdentityUser>
{
    public MyDataContext(DbContextOptions<MyDataContext> options) : base(options)
    {
    }
}
