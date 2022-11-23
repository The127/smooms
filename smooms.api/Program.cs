using System.Text.Json;
using System.Text.Json.Serialization;
using CheckConstraints;
using HttpExceptions;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using smooms.api.Behaviours;
using smooms.api.Models;
using smooms.api.Options;
using smooms.api.Services;
using smooms.api.Utils;

namespace smooms.api;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

        builder.Services.AddControllers()
            .AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

                var enumConverter = new JsonStringEnumConverter(JsonNamingPolicy.CamelCase);
                opts.JsonSerializerOptions.Converters.Add(enumConverter);

                opts.JsonSerializerOptions.AddEntityIdJsonConverterFromAssembly(typeof(Program).Assembly);

                opts.JsonSerializerOptions.Converters.Add(NodaConverters.InstantConverter);
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SchemaGeneratorOptions.UseInlineDefinitionsForEnums = true;
            options.SchemaGeneratorOptions.SupportNonNullableReferenceTypes = true;
            options.SchemaGeneratorOptions.AddEntityIdJsonSchemaDefinitionFromAssembly(typeof(Program).Assembly);
            // options.ConfigureForNodaTime();
        });

        builder.Services.AddHealthChecks();

        builder.Services.AddLazyCache();

        builder.Services.AddMediatR(typeof(Program).Assembly);
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehaviour<,>));
        builder.Services.AddFluentValidation(typeof(Program).Assembly.Yield());

        builder.Services.AddTransient<IClockService>(provider =>
        {
            var clock = provider.GetRequiredService<IClock>();
            var dateTimeZoneProvider = provider.GetRequiredService<IDateTimeZoneProvider>();
            var clientInfo = provider.GetRequiredService<ClientInfo>();
            var dateTimeZone = dateTimeZoneProvider.GetZoneOrNull(clientInfo.TimeZone)!;
            return new ClockService(clock, dateTimeZone);
        });

        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<ClientInfo>();

        builder.Services.AddTransient<ISecurityService, SecurityService>();

        builder.Services.AddOptions<SmoomsOptions>()
            .Bind(builder.Configuration.GetSection("Smooms"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddAppDbContext(builder.Configuration.GetConnectionString("Smooms"));

        var app = builder.Build();

// Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        await app.MigrateDatabase();

        app.UseHttpExceptions();
        app.UseHttpsRedirection();
        app.MapControllers();

        await app.RunAsync();
    }

    private static async Task MigrateDatabase(this WebApplication app)
    {
        var dbContextFactory = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        await dbContext.Database.MigrateAsync();
    }

    private static void AddAppDbContext(this IServiceCollection serviceCollection, string? connectionString)
    {
        serviceCollection.AddDbContextFactory<AppDbContext>(optionsBuilder =>
        {
            ConfigureDbContext(optionsBuilder, connectionString);
        });
    }

    public static void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder, string? connectionString)
    {
        optionsBuilder.UseNpgsql(connectionString,
                contextOptionsBuilder => { contextOptionsBuilder.UseNodaTime(); })
            .UseCheckConstraints()
            .UseSnakeCaseNamingConvention();
    }
}