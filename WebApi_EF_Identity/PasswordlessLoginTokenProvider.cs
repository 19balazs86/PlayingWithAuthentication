using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace WebApi_EF_Identity;

// This provider generates tokens for 15 minutes
public sealed class PasswordlessLoginTokenProvider<TUser> : DataProtectorTokenProvider<TUser>where TUser : class
{
    public PasswordlessLoginTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<PasswordlessLoginTokenProviderOptions> options,
        ILogger<PasswordlessLoginTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
    {
    }
}

public static class CustomIdentityBuilderExtensions
{
    public static IdentityBuilder AddPasswordlessLoginTokenProvider(this IdentityBuilder builder)
    {
        Type provider = typeof(PasswordlessLoginTokenProvider<>).MakeGenericType(builder.UserType);

        return builder.AddTokenProvider(PasswordlessLoginTokenProviderOptions.ProviderName, provider);
    }
}

public sealed class PasswordlessLoginTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public const string ProviderName = "PasswordlessLoginTokenProvider";
    public const string Purpose      = "passwordless-auth";

    public PasswordlessLoginTokenProviderOptions()
    {
        Name          = ProviderName;
        TokenLifespan = TimeSpan.FromMinutes(15);
    }
}