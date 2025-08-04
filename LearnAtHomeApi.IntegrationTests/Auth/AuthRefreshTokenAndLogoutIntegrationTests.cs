using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using LearnAtHomeApi.Authentication.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace LearnAtHomeApi.FunctionalTests.Auth;

[Collection("Auth 3 Refresh Token and Logout Integration Tests")]
public class AuthRefreshTokenAndLogoutIntegrationTests(LearnAtHomeWebApplicationFactory factory)
    : IntegrationTestAbstract(factory)
{
    private readonly AuthRegisterDto _registerDto = new()
    {
        Email = "test@test.com",
        Username = "TestUser",
        Password = "Password70*$",
        PasswordConfirm = "Password70*$",
    };

    /// <summary>
    /// When I call /auth/refresh with valid refresh token in cookies, API should return 200 success with the access_token and refresh_token cookies
    /// </summary>
    [Fact]
    private async Task RefreshTokenValid_Returns200AndTokens()
    {
        var (refreshTokenCookieStr, _) = await Login(
            new AuthLoginDto() { Email = _registerDto.Email, Password = _registerDto.Password }
        );

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/refresh");
        requestMessage.Headers.Add("Cookie", StringifyCookies([refreshTokenCookieStr], true));

        var response = await Client.SendAsync(requestMessage);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.False(response.Headers.Contains("Set-Cookie"));
    }

    /// <summary>
    /// When I call /auth/refresh with an invalid token, API should return 401
    /// </summary>
    [Fact]
    private async Task RefreshTokenInvalid_Returns401()
    {
        var (refreshTokenCookieStr, sessionTokenCookieStr) = await Login(
            new AuthLoginDto() { Email = _registerDto.Email, Password = _registerDto.Password }
        );

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/refresh");
        requestMessage.Headers.Add("Cookie", StringifyCookies([refreshTokenCookieStr]));

        var response = await Client.SendAsync(requestMessage);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var setCookieHeader = response.Headers.GetValues("Set-Cookie").ToList();
        Assert.Contains(setCookieHeader, c => c.StartsWith("session_token="));

        Assert.NotEqual(
            setCookieHeader.Find(c => c.StartsWith("session_token=")),
            sessionTokenCookieStr
        );
    }

    /// <summary>
    /// When I call /auth/refresh with no refresh token in cookies, API should return 401
    /// </summary>
    [Fact]
    private async Task RefreshTokenWithNoCookie_Returns401()
    {
        var response = await Client.GetAsync("/api/v1/auth/refresh");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// When I call /auth/logout, API should return 200 and cookie shall be cleared
    /// </summary>
    [Fact]
    private async Task Logout_Returns200()
    {
        var (refreshTokenCookieStr, sessionTokenCookieStr) = await Login(
            new AuthLoginDto() { Email = _registerDto.Email, Password = _registerDto.Password }
        );

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/logout");
        requestMessage.Headers.Add(
            "Cookie",
            StringifyCookies([refreshTokenCookieStr, sessionTokenCookieStr])
        );

        var response = await Client.SendAsync(requestMessage);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var setCookieHeader = response.Headers.GetValues("Set-Cookie").ToList();
        Assert.Contains(setCookieHeader, c => c.StartsWith("refresh_token=;"));
        Assert.Contains(setCookieHeader, c => c.StartsWith("session_token=;"));
    }

    protected override async Task BeforeEachTest()
    {
        await Client.PostAsJsonAsync("/api/v1/auth/register", _registerDto);
    }

    private string FormatCookie(string cookieStr)
    {
        var split = cookieStr.Split(";").ToList();

        var cookieName = split[0].Split("=")[0];
        var cookieValue = split[0].Split("=")[1];

        return $"{cookieName}={cookieValue}";
    }

    private string StringifyCookies(string[] tokenCookiesStr, bool returnInvalid = false)
    {
        var cookies = tokenCookiesStr.Select(FormatCookie);

        return string.Join("; ", returnInvalid ? cookies.Select(c => c += "invalidTruc") : cookies);
    }
}
