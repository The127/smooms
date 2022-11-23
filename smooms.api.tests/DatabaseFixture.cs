using Microsoft.EntityFrameworkCore;
using smooms.api.Models;

namespace smooms.api.tests;

public class DatabaseFixture : IDisposable
{
    private readonly DbContext _dbContext;

    public string TemplateDatabaseName { get; }
    public string Connection { get; }

    public DatabaseFixture()
    {
        var id = Guid.NewGuid().ToString().Replace("-", "");
        TemplateDatabaseName = $"smooms_templ_{id}";
        Connection = $"Host=localhost;Port=5435;Database={TemplateDatabaseName};Username=user;Password=password";
        
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        Program.ConfigureDbContext(optionsBuilder, Connection);
        
        _dbContext = new AppDbContext(optionsBuilder.Options);
        _dbContext.Database.EnsureCreated();
        _dbContext.Database.CloseConnection();
    }
    
    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
    }
}