using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TitanCommerce.Application.Common.Interfaces;
using TitanCommerce.Application.Identity.DTOs;
using TitanCommerce.Application.Identity.Interfaces;
using TitanCommerce.Domain.Identity.Entities;
using TitanCommerce.Infrastructure.Persistence;
using TitanCommerce.Infrastructure.Security;

namespace TitanCommerce.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IOptions<JwtSettings> _jwtSettings;

    public AuthService(ApplicationDbContext dbContext, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IOptions<JwtSettings> jwtSettings)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _jwtSettings = jwtSettings;
    }               

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken = default);

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }
        
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User account is inactive.");
        }

        // Generate JWT token
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshTokenString = _jwtTokenGenerator.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.Value.RefreshTokenExpirationDays);

        var refreshToken = RefreshToken.Create(user.Id, refreshTokenString, refreshTokenExpiry);
        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
        user.Id,
        user.Email,
        user.FirstName,
        user.LastName,
        user.Role.ToString(),
        accessToken,
        refreshTokenString,
        refreshTokenExpiry
        );
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var existingRefreshToken = await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (existingRefreshToken is null)
        {
            throw new SecurityTokenException("Invalid refresh token.");
        }

        // Reuse Detection: যদি টোকেন ইতিমধ্যে বাতিল হয়ে থাকে, তবে এটি চুরির লক্ষণ!
        if (existingRefreshToken.IsRevoked)
        {
            // ইউজারের সব সক্রিয় টোকেন বাতিল করে একাউন্ট লক/লগআউট করা
            var allUserTokens = await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == existingRefreshToken.UserId && !rt.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var token in allUserTokens)
            {
                token.Revoke("Token reuse detected.");
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            throw new SecurityTokenException("Compromised token detected. All sessions revoked. Please log in again.");
        }

        if (!existingRefreshToken.IsActive)
        {
            throw new SecurityTokenException("Expired refresh token.");
        }

        // Refresh Token Rotation (RTR): পুরোনো টোকেন বাতিল করা ও নতুন টোকেন ইস্যু করা
        var user = existingRefreshToken.User;
        var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var newRefreshTokenString = _jwtTokenGenerator.GenerateRefreshToken();
        var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.Value.RefreshTokenExpirationDays);

        existingRefreshToken.Revoke(newRefreshTokenString);

        var newRefreshToken = RefreshToken.Create(user.Id, newRefreshTokenString, newRefreshTokenExpiry);
        _dbContext.RefreshTokens.Add(newRefreshToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role.ToString(),
            newAccessToken,
            newRefreshTokenString,
            newRefreshTokenExpiry
        );
    }

    public async Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // ইমেইল ইতিমধ্যে রেজিস্টার্ড কিনা চেক
        var exists = await _dbContext.Users
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException($"User with email '{request.Email}' already exists.");
        }

        // সিকিউর পাসওয়ার্ড হ্যাশ তৈরি
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // রিচ ডোমেন মডেল ইনস্ট্যানশিয়েশন
        var user = User.Create(
            request.FirstName,
            request.LastName,
            normalizedEmail,
            passwordHash,
            phoneNumber: request.PhoneNumber
        );

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role.ToString(),
            user.IsEmailVerified
        );
    }
}