using AuthTokens.Domain.Entities;

namespace AuthTokens.Service
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
        string GenerateRefreshToken();
    }
}
