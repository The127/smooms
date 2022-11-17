using FluentBlazorRouter;
using smooms.app.Authentication;
using smooms.app.Pages;

namespace smooms.app;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHealthChecks();

        builder.ConfigureRouting();
        
        builder.Services.AddTransient<IRouterMiddleware, AuthenticationMiddleware>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseCors();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.MapBlazorHub();
        app.MapControllers();
        app.MapHealthChecks("/health");
        app.MapFallbackToPage("/_Host");

        await app.RunAsync();
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