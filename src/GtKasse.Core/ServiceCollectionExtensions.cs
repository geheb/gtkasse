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
        services.AddScoped<IdentityErrorDescriber, GermanyIdentityErrorDescriber>();
        services.AddScoped<AppDbContextInitializer>();

        services.AddScoped<UnitOfWork>();
        services.AddScoped<Foods>();
        services.AddScoped<FoodBookings>();
        services.AddScoped<Invoices>();
        services.AddScoped<Trips>();
        services.AddScoped<Vehicles>();
        services.AddScoped<Tryouts>();
        services.AddScoped<Boats>();
        services.AddScoped<Clubhouse>();

        services.Configure<SmtpConnectionOptions>(config.GetSection("Smtp"));
        services.AddSingleton<EmailValidatorService>();
        services.AddSingleton<SmtpDispatcher>();
        services.AddScoped<EmailService>();
        services.AddScoped<UserService>();
        services.AddScoped<LoginService>();
        services.AddScoped<IdentityRepository>();
        services.AddScoped<MySqlDbContext>();
        services.AddScoped<MySqlMigration>();

        services.AddHostedService<HostedWorker>();
    }
}
