using TitanCommerce.Domain.Common;

namespace TitanCommerce.Domain.Identity.Entities;

public class RefreshToken : BaseEntity<Guid>
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string? ReplacedByToken { get; private set; }

    // Navigation Property
    public User User { get; private set; } = null!;

    private RefreshToken() : base() { }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAtUtc)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            IsRevoked = false
        };
    }

    public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAtUtc;

    public void Revoke(string? replacedByToken = null)
    {
        IsRevoked = true;
        RevokedAtUtc = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
    }
}