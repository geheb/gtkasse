namespace GtKasse.Core;

using GtKasse.Core.Database;
using GtKasse.Core.Email;
using GtKasse.Core.Repositories;
using GtKasse.Core.User;
using GtKasse.Core.Worker;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddCore(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<SmtpConnectionOptions>(config.GetSection("Smtp"));

        services.AddHostedService<HostedWorker>();

        services.AddSingleton<EmailValidatorService>();
        services.AddSingleton<IEmailSender, SmtpDispatcher>();

        services.AddScoped<IdentityErrorDescriber, GermanyIdentityErrorDescriber>();
        services.AddScoped<AppDbContextInitializer>();
        services.AddScoped<Users>();
        services.AddScoped<Foods>();
        services.AddScoped<Bookings>();
        services.AddScoped<Invoices>();
        services.AddScoped<Trips>();
        services.AddScoped<WikiArticles>();
        services.AddScoped<Vehicles>();
        services.AddScoped<Tryouts>();
        services.AddScoped<Boats>();
        services.AddScoped<Clubhouse>();
    }
}
