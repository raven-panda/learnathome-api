using System.Net;
using System.Net.Http.Json;
using LearnAtHomeApi.Authentication.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace LearnAtHomeApi.FunctionalTests.Auth;

public class AuthRegisterIntegrationTests(LearnAtHomeWebApplicationFactory factory)
    : IClassFixture<LearnAtHomeWebApplicationFactory>,
        IDisposable
{
    private readonly HttpClient _client = factory.CreateClient();

    /// <summary>
    /// When I register with valid dto, API should return 200 success with the access_token and refresh_token cookies
    /// </summary>
    [Fact]
    public async Task Register_Returns200AndTokens()
    {
        var dto = new AuthRegisterDto
        {
            Email = "test@test.com",
            Username = "TestUser",
            Password = "Password70*$",
            PasswordConfirm = "Password70*$",
        };

        await RegisterUser(
            dto,
            HttpStatusCode.OK,
            response =>
            {
                var setCookieHeader = response.Headers.GetValues("Set-Cookie").ToList();
                Assert.Contains(setCookieHeader, c => c.StartsWith("refresh_token="));
                Assert.Contains(setCookieHeader, c => c.StartsWith("session_token="));
            }
        );
    }

    /// <summary>
    /// When I register with different passwords, API should return 400 Bad Request
    /// </summary>
    [Fact]
    public async Task RegisterWithDifferentPassword_Returns400()
    {
        var dto = new AuthRegisterDto
        {
            Email = "test@test.com",
            Username = "TestUser",
            Password = "Password70*$",
            PasswordConfirm = "Password70*$_notMatching",
        };

        await RegisterUser(dto, HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// When I register twice or more the same valid dto, API should return 409 Conflict
    /// </summary>
    [Fact]
    public async Task RegisterWithAlreadyUsedEmail_Returns409()
    {
        var dto = new AuthRegisterDto
        {
            Email = "test@test.com",
            Username = "TestUser",
            Password = "Password70*$",
            PasswordConfirm = "Password70*$",
        };

        await RegisterUser(dto, HttpStatusCode.OK);
        await RegisterUser(dto, HttpStatusCode.Conflict);
    }

    private async Task RegisterUser(
        AuthRegisterDto dto,
        HttpStatusCode expectedStatusCode,
        Action<HttpResponseMessage>? responseAction = null
    )
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", dto);
        Assert.Equal(expectedStatusCode, response.StatusCode);

        responseAction?.Invoke(response);
    }

    public void Dispose()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureDeleted();
    }
}
