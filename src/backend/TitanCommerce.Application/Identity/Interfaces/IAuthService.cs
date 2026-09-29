using TitanCommerce.Application.Identity.DTOs;

namespace TitanCommerce.Application.Identity.Interfaces;

public interface IAuthService
{
    Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}