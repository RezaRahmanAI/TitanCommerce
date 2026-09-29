using Microsoft.EntityFrameworkCore;
using TitanCommerce.Application.Common.Interfaces;
using TitanCommerce.Application.Identity.DTOs;
using TitanCommerce.Application.Identity.Interfaces;
using TitanCommerce.Domain.Identity.Entities;
using TitanCommerce.Infrastructure.Persistence;

namespace TitanCommerce.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(ApplicationDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
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