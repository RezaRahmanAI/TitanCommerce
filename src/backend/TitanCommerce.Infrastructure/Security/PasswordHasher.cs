using Microsoft.AspNetCore.Identity;
using TitanCommerce.Application.Common.Interfaces;
using IdentityPasswordHasher = Microsoft.AspNetCore.Identity.PasswordHasher<object>;

namespace TitanCommerce.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    // Dummy ক্লাস অবজেক্ট hasher জেনেরিক টাইপের জন্য
    private readonly IdentityPasswordHasher _hasher = new();
    private readonly object _dummyUser = new();

    public string HashPassword(string password)
    {
        return _hasher.HashPassword(_dummyUser, password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var result = _hasher.VerifyHashedPassword(_dummyUser, passwordHash, password);
        return result == PasswordVerificationResult.Success || 
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
