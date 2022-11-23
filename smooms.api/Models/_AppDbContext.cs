using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace smooms.api.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // reconfigure the context to use Npgsql's name translation, and other features inside linqpad
        if (InsideLINQPad)
        {
            var connectionString = optionsBuilder.Options.FindExtension<NpgsqlOptionsExtension>()?.ConnectionString;
            Program.ConfigureDbContext(optionsBuilder, connectionString);
        }
    }
    
    internal bool InsideLINQPad => AppDomain.CurrentDomain.FriendlyName.StartsWith("LINQPad");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}