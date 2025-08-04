using System.Net.Http.Json;
using LearnAtHomeApi.Authentication.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace LearnAtHomeApi.FunctionalTests;

public class IntegrationTestAbstract(LearnAtHomeWebApplicationFactory factory)
    : IClassFixture<LearnAtHomeWebApplicationFactory>,
        IAsyncLifetime
{
    protected readonly HttpClient Client = factory.CreateClient();

    public async Task InitializeAsync()
    {
        await Task.Delay(50);
        await BeforeEachTest();
    }

    public Task DisposeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureDeleted();
        return Task.CompletedTask;
    }

    protected virtual Task BeforeEachTest()
    {
        return Task.CompletedTask;
    }

    protected async Task<(string refreshTokenCookie, string sessionTokenCookie)> Login(
        AuthLoginDto dto
    )
    {
        var response = await Client.PostAsJsonAsync("api/v1/auth/login", dto);

        var setCookieHeader = response.Headers.GetValues("Set-Cookie").ToList();
        var refreshTokenCookie = setCookieHeader.Find(c => c.Contains("refresh_token="));
        var sessionTokenCookie = setCookieHeader.Find(c => c.Contains("session_token="));
        Assert.NotNull(refreshTokenCookie);
        Assert.NotNull(sessionTokenCookie);

        return (refreshTokenCookie, sessionTokenCookie);
    }
}
