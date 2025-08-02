using System.Security.Claims;
using System.Text;
using LearnAtHomeApi.User.Dto;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace LearnAtHomeApi.Authentication.Security;

internal sealed class TokenProvider(IConfiguration configuration)
{
    public string GenerateAccessToken(UserDto user, out DateTime expiration)
    {
        return GenerateToken(user, out expiration, "Jwt:ExpirationInMinutes");
    }

    public string GenerateRefreshToken(UserDto user, out DateTime expiration)
    {
        return GenerateToken(user, out expiration, "Jwt:RefreshExpirationInDays");
    }

    private string GenerateToken(UserDto user, out DateTime expiration, string expirationConfigKey)
    {
        if (user.Id == null)
            throw new ArgumentNullException(nameof(user.Id));

        string secretKey = configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = GenerateTokenDescriptor(credentials, user);
        expiration =
            tokenDescriptor.Expires
            ?? DateTime.UtcNow.AddMinutes(configuration.GetValue<int>(expirationConfigKey));
        var handler = new JsonWebTokenHandler();

        return handler.CreateToken(tokenDescriptor);
    }

    private SecurityTokenDescriptor GenerateTokenDescriptor(
        SigningCredentials credentials,
        UserDto user
    )
    {
        return new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id!.Value.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                ]
            ),
            Expires = DateTime.UtcNow.AddMinutes(
                configuration.GetValue<int>("Jwt:ExpirationInMinutes")
            ),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
        };
    }

    public int ParseUserId(string token)
    {
        var handler = new JsonWebTokenHandler();
        var validationResult = handler
            .ValidateTokenAsync(
                token,
                new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)
                    ),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true, // Vérifie si le token est expiré
                }
            )
            .Result;

        if (!validationResult.IsValid)
            throw new SecurityTokenException();

        var claims = validationResult.Claims;
        var userId = claims[JwtRegisteredClaimNames.Sub]?.ToString();
        if (userId == null)
            throw new SecurityTokenException();

        return int.Parse(userId);
    }
}
