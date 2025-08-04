using System.Net;
using System.Net.Http.Json;
using LearnAtHomeApi.Authentication.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace LearnAtHomeApi.FunctionalTests.Auth;

[Collection("Auth 1 Register Integration Tests")]
public class AuthRegisterIntegrationTests(LearnAtHomeWebApplicationFactory factory)
    : IntegrationTestAbstract(factory)
{
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
    /// When I register with a password that doesn't match given regex, API should return 400
    /// </summary>
    [Fact]
    public async Task RegisterWithInvalidPassword_Returns400()
    {
        var dto = new AuthRegisterDto
        {
            Email = "test@test.com",
            Username = "TestUser",
            Password = "Password70*$",
            PasswordConfirm = "Password70*$",
        };

        await RegisterUser(dto, HttpStatusCode.OK);
        dto.Password = "Passrd70*$";
        dto.PasswordConfirm = "Passrd70*$";
        await RegisterUser(dto, HttpStatusCode.BadRequest);
        dto.Password = "PasswordTT*$";
        dto.PasswordConfirm = "PasswordTT*$";
        await RegisterUser(dto, HttpStatusCode.BadRequest);
        dto.Password = "password70*$";
        dto.PasswordConfirm = "password70*$";
        await RegisterUser(dto, HttpStatusCode.BadRequest);
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
        var response = await Client.PostAsJsonAsync("/api/v1/auth/register", dto);
        Assert.Equal(expectedStatusCode, response.StatusCode);

        responseAction?.Invoke(response);
    }
}
