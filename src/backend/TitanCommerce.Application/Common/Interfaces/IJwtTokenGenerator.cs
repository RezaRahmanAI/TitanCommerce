using TitanCommerce.Domain.Identity.Entities;

namespace TitanCommerce.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}