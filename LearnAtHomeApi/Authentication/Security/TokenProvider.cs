using System.Security.Claims;
using System.Text;
using LearnAtHomeApi.User.Dto;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace LearnAtHomeApi.Authentication.Security;

public sealed class TokenProvider(IConfiguration configuration)
{
    /// <summary>
    /// Generate the session time token used to access to resources of the API. <br/> See Jwt:ExpirationInMinutes in app's config to see its duration.
    /// </summary>
    /// <param name="user">User that requested a token generation</param>
    /// <param name="expiration">Return the token expiration</param>
    /// <returns>Session time token generated</returns>
    public string GenerateAccessToken(UserDto user, out DateTime expiration)
    {
        return GenerateToken(user, out expiration, "Jwt:ExpirationInMinutes");
    }

    /// <summary>
    /// Generate the refresh token used to access to resources of the API. <br/> See Jwt:RefreshExpirationInDays in app's config to see its duration.
    /// </summary>
    /// <param name="user">User that requested a token generation</param>
    /// <param name="expiration">Return the token expiration</param>
    /// <returns>Refresh token generated</returns>
    public string GenerateRefreshToken(UserDto user, out DateTime expiration)
    {
        return GenerateToken(user, out expiration, "Jwt:RefreshExpirationInDays");
    }

    /// <summary>
    /// Generate a base64 token that last based on the given duration configuration
    /// </summary>
    /// <param name="user">User that requested a token generation</param>
    /// <param name="expiration">Return the token expiration</param>
    /// <param name="expirationConfigKey">Config key that points the token duration to set</param>
    /// <returns>Token generated</returns>
    /// <exception cref="ArgumentNullException">Thrown if the given user has no id</exception>
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

    /// <summary>
    /// Create a token descriptor which is used to store 4 claims : user's id, email, role and a unique Guid to ensure the token is unique.
    /// </summary>
    /// <param name="credentials">Signing credentials used to sign the token</param>
    /// <param name="user">User that requested a token generation</param>
    /// <returns>Token descriptor</returns>
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
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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

    /// <summary>
    /// Parse user id from given token
    /// </summary>
    /// <param name="token">Base64 token</param>
    /// <returns>User's id</returns>
    /// <exception cref="SecurityTokenException">Thrown if the token is invalid or if the user ID parsed from given token is null</exception>
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

    /// <summary>
    ///
    /// </summary>
    /// <param name="token"></param>
    /// <returns>Tuple of id, email and role parsed from token</returns>
    /// <exception cref="SecurityTokenException">Thrown if the token is invalid or if the user ID, email or role parsed from given token is null</exception>
    public (int id, string email, UserRole role) ParseUserToken(string token)
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
        var email = claims[JwtRegisteredClaimNames.Email]?.ToString();
        var role = claims[ClaimTypes.Role]?.ToString();
        var isRoleValid = Enum.TryParse(role, out UserRole parsedRole);

        if (userId == null || email == null || role == null || !isRoleValid)
            throw new SecurityTokenException();

        return (int.Parse(userId), email, role: parsedRole);
    }
}
