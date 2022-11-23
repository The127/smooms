using Microsoft.EntityFrameworkCore;
using smooms.api.Models;

namespace smooms.api.tests;

public class TestDbContextFactory : IDbContextFactory<AppDbContext>
{
    private readonly string _connectionString;

    public TestDbContextFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public AppDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        Program.ConfigureDbContext(optionsBuilder, _connectionString);
        return new AppDbContext(optionsBuilder.Options);
    }
}