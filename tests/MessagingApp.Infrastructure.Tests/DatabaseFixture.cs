using MessagingApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MessagingApp.Infrastructure.Tests;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly string _connectionString;

    public DatabaseFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.local.json", optional: false)
            .Build();

        _connectionString = configuration.GetConnectionString("TestConnection")
            ?? throw new InvalidOperationException("TestConnection is missing from appsettings.Test.local.json");
    }

    public MessagingAppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MessagingAppDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        return new MessagingAppDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await using var context = CreateContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await using var context = CreateContext();
        await context.Database.EnsureDeletedAsync();
    }
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}
