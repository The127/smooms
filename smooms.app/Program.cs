using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentBlazorRouter;
using HttpExceptions;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using smooms.app.Authentication;
using smooms.app.Behaviours;
using smooms.app.Models;
using smooms.app.Options;
using smooms.app.Pages;
using smooms.app.Services;
using smooms.app.Utils;

namespace smooms.app;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddControllers()
            .AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

                var enumConverter = new JsonStringEnumConverter(JsonNamingPolicy.CamelCase);
                opts.JsonSerializerOptions.Converters.Add(enumConverter);

                opts.JsonSerializerOptions.AddEntityIdJsonConverterFromAssembly(typeof(Program).Assembly);

                // opts.JsonSerializerOptions.Converters.Add(NodaConverters.InstantConverter);
            });

        builder.Services.AddEndpointsApiExplorer();
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
        builder.Services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(CachingBehaviour<,>));
        builder.Services.AddFluentValidation(typeof(Program).Assembly.Yield());

        builder.Services.AddTransient<IClockService>(provider =>
        {
            var clock = provider.GetRequiredService<IClock>();
            var dateTimeZoneProvider = provider.GetRequiredService<IDateTimeZoneProvider>();
            var clientInfo = provider.GetRequiredService<ClientInfo>();
            var dateTimeZone = dateTimeZoneProvider.GetZoneOrNull(clientInfo.TimeZone)!;
            return new ClockService(clock, dateTimeZone);
        });

        builder.Services.AddTransient<IRouterMiddleware, AuthenticationMiddleware>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

        builder.Services.AddTransient<ISecurityService, SecurityService>();
        builder.Services.AddScoped<ClientInfo>();

        builder.Services.AddOptions<SmoomsOptions>()
            .Bind(builder.Configuration.GetSection("Smooms"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddDbContextFactory<AppDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("Smooms"))
                .UseAllCheckConstraints()
                .UseSnakeCaseNamingConvention();
        });

        builder.ConfigureRouting();

        var app = builder.Build();

        await app.MigrateDatabase();

        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors();
        app.UseHttpExceptions();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.MapBlazorHub();
        app.MapControllers();
        app.MapHealthChecks("/health");
        app.MapFallbackToPage("/_Host");

        await app.RunAsync();
    }

    private static async Task MigrateDatabase(this WebApplication app)
    {
        var dbContextFactory = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        await dbContext.Database.MigrateAsync();
    }

    private static void ConfigureRouting(this WebApplicationBuilder builder)
    {
        builder.Services.AddFluentRouting<IndexPage>(rootGroup =>
        {
            rootGroup.WithPage<LoginPage>("login");
            rootGroup.WithPage<RegisterPage>("register");
            rootGroup.WithPage<ResetPasswordPage>("reset-password");
            
            rootGroup.WithPage<AppPage>("app", appBuilder =>
            {
                appBuilder.RequireAuthentication();
            });
        });
    }
}