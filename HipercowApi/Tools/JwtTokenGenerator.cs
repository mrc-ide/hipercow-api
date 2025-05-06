// Copyright (c) Imperial College London. All rights reserved.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Generate the JWT token.
/// </summary>
public class JwtTokenGenerator(IOptions<JwtSettings> settings)
{
    private readonly JwtSettings settings = settings.Value;

    /// <summary>
    /// Generate the token for a given username.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <returns>The string of the token.</returns>
    public string GenerateToken(string username)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.settings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: this.settings.Issuer,
            audience: this.settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(this.settings.ExpiresInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
