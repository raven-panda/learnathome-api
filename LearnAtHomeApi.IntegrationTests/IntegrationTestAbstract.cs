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
}
