using System.ComponentModel.DataAnnotations;

namespace TitanCommerce.Application.Identity.DTOs;

public record RegisterRequest(
    [Required] string FirstName,
    [Required] string LastName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    string? PhoneNumber
);

public record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    bool IsEmailVerified
);
