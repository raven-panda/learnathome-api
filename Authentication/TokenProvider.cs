using System.Security.Claims;
using System.Text;
using LearnAtHomeApi.User.Dto;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace LearnAtHomeApi.Authentication;

public sealed class TokenProvider(IConfiguration configuration)
{
    public string Generate(UserDto user, out DateTime expiration)
    {
        if (user.Id == null)
            throw new ArgumentNullException(nameof(user.Id));

        string secretKey = configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = GenerateTokenDescriptor(credentials, user);
        expiration =
            tokenDescriptor.Expires
            ?? DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes"));
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
}
