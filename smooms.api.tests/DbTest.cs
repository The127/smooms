using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using smooms.api.Models;

namespace smooms.api.tests;

[Collection("Database")]
public class DbTest : IDisposable
{
    protected IDbContextFactory<AppDbContext> DbContextFactory { get; }

    protected DbTest(DatabaseFixture databaseFixture)
    {
        var id = Guid.NewGuid().ToString().Replace("-", "");
        var databaseName = $"{GetType().Name}-{id}";

        using (var templateConnection = new NpgsqlConnection(databaseFixture.Connection))
        {
            templateConnection.Open();
            
            using (var command = templateConnection.CreateCommand())
            {
                command.CommandText = $"CREATE DATABASE \"{databaseName}\" TEMPLATE \"{databaseFixture.TemplateDatabaseName}\"";
                command.ExecuteNonQuery();
            }
        }
        
        var connectionString = $"Host=localhost;Port=5435;Database={databaseName};Username=user;Password=password";
        DbContextFactory = new TestDbContextFactory(connectionString);
    }

    public void Dispose()
    {
        using var dbContext = DbContextFactory.CreateDbContext();
        dbContext.Database.EnsureDeleted();
    }
}