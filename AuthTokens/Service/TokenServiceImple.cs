﻿using AuthTokens.Domain.Contracts;
using AuthTokens.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthTokens.Service
{  
    public class TokenServiceImple : ITokenService
    {
        private readonly SymmetricSecurityKey _secretKey;
        private readonly string? _validIssuer;
        private readonly string? _validAudience;
        private readonly double _expires;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TokenServiceImple> _logger;
     
        public TokenServiceImple(IConfiguration configuration, UserManager<ApplicationUser> userManager, ILogger<TokenServiceImple> logger)
        {
            _userManager = userManager;
            _logger = logger;
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSetting>();
            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new InvalidOperationException("JWT secret key is not configured.");
            }

            _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            _validIssuer = jwtSettings.ValidIssuer;
            _validAudience = jwtSettings.ValidAudience;
            _expires = jwtSettings.Expires;
        }
    
        public async Task<string> GenerateToken(ApplicationUser user)
        {
            var singingCredentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256);
            var claims = await GetClaimsAsync(user);
            var tokenOptions = GenerateTokenOptions(singingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user?.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user?.Id ?? string.Empty),
                new Claim(ClaimTypes.Email, user?.Email ?? string.Empty),
                new Claim("FirstName", user?.FirstName ?? string.Empty),
                new Claim("LastName", user?.LastName ?? string.Empty),
                new Claim("Gender", user?.Gender ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            return new JwtSecurityToken(
                issuer: _validIssuer,
                audience: _validAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_expires),
                signingCredentials: signingCredentials
            );
        }     
   
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var refreshToken = Convert.ToBase64String(randomNumber);
            return refreshToken;
        }
    }
}
