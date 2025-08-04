using System.Net;
using System.Net.Http.Json;
using LearnAtHomeApi.Authentication.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace LearnAtHomeApi.FunctionalTests.Auth;

[Collection("Auth 2 Login Integration Tests")]
public class AuthLoginIntegrationTests(LearnAtHomeWebApplicationFactory factory)
    : IClassFixture<LearnAtHomeWebApplicationFactory>,
        IDisposable
{
    private readonly HttpClient _client = factory.CreateClient();

    private readonly AuthRegisterDto _registerDto = new()
    {
        Email = "test@test.com",
        Username = "TestUser",
        Password = "Password70*$",
        PasswordConfirm = "Password70*$",
    };

    private async Task BeforeEachTest()
    {
        await _client.PostAsJsonAsync("/api/v1/auth/register", _registerDto);
    }

    /// <summary>
    /// When I login with valid dto, API should return 200 success with the access_token and refresh_token cookies
    /// </summary>
    [Fact]
    public async Task LoginValid_Returns200AndTokens()
    {
        await BeforeEachTest();

        var loginDto = new AuthLoginDto
        {
            Email = _registerDto.Email,
            Password = _registerDto.Password,
        };
        await LoginUser(
            loginDto,
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
    /// When I login with valid dto, API should return 401 Unauthorized
    /// </summary>
    [Fact]
    public async Task LoginInvalidEmail_Returns401()
    {
        await BeforeEachTest();

        var loginDto = new AuthLoginDto
        {
            Email = _registerDto.Email + "_invalidString",
            Password = _registerDto.Password,
        };
        await LoginUser(loginDto, HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// When I login with valid dto, API should return 401 Unauthorized
    /// </summary>
    [Fact]
    public async Task LoginInvalidPassword_Returns401()
    {
        await BeforeEachTest();

        var loginDto = new AuthLoginDto
        {
            Email = _registerDto.Email,
            Password = _registerDto.Password + "_invalidString",
        };
        await LoginUser(loginDto, HttpStatusCode.Unauthorized);
    }

    private async Task LoginUser(
        AuthLoginDto dto,
        HttpStatusCode expectedStatusCode,
        Action<HttpResponseMessage>? responseAction = null
    )
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", dto);
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
