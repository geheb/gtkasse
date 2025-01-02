using CsvHelper;
using CsvHelper.Configuration;
using GtKasse.Cli.Models;
using GtKasse.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace GtKasse.Cli
{
    partial class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json");
                    config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddMySqlContext(context.Configuration);
                    services.AddLogging();
                    services.AddHttpClient();
                })
                .ConfigureLogging((context, config) =>
                {
                    config.AddConsole();
                })
                .UseConsoleLifetime()
                .Build();

            CreateDataProtectionCertificate();

            if (args.Length < 1)
            {
                Console.WriteLine("no args provided!");
                return 1;
            }

            switch (args[0])
            {
                case "--ef-migrate": return EfMigrate(host.Services);
                case "--dataprotection-create": return CreateDataProtectionCertificate();
                case "--import-users":
                    {
                        if (args.Length < 2) Console.WriteLine("Missing file!");
                        return await ImportUsers(host.Services, args[1]);
                    }
            }

            Console.WriteLine("unknown args detected!");

            return 1;
        }

        static int EfMigrate(IServiceProvider serviceProvider)
        {
            using var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

            var pendingMigrations = context!.Database.GetPendingMigrations();
            var migrations = pendingMigrations as IList<string> ?? pendingMigrations.ToList();
            if (!migrations.Any())
            {
                Console.WriteLine("No pending migrations found.");
                return 1;
            }

            Console.WriteLine("Pending migrations {0}", migrations.Count());
            foreach (var migration in migrations)
            {
                Console.WriteLine($"\t{migration}");
            }

            Console.WriteLine("Press RETURN to continue.");
            if (Console.ReadKey().Key != ConsoleKey.Enter) return 1;

            Console.WriteLine("Migrate database...");
            var watch = Stopwatch.StartNew();
            try
            {
                context.Database.Migrate();
            }
            finally
            {
                watch.Stop();
                Console.WriteLine($"Migration done, elapsed time: {watch.Elapsed}");
            }

            return 0;
        }

        static int CreateDataProtectionCertificate()
        {
            var sanBuilder = new SubjectAlternativeNameBuilder();
            sanBuilder.AddIpAddress(IPAddress.Loopback);
            sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);
            sanBuilder.AddDnsName("localhost");
            sanBuilder.AddDnsName(Environment.MachineName);

            const string Name = "DataProtection";

            var distinguishedName = new X500DistinguishedName($"CN={Name}");

            using var rsa = RSA.Create(4096);

            var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));

            request.CertificateExtensions.Add(
               new X509EnhancedKeyUsageExtension(
                   new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false));

            request.CertificateExtensions.Add(sanBuilder.Build());

            var certificate = request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.Date), new DateTimeOffset(DateTime.UtcNow.AddYears(10).Date));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                certificate.FriendlyName = Name;
            }
            var pfx = certificate.Export(X509ContentType.Pfx);
            File.WriteAllBytes("dataprotection.pfx", pfx);

            return 0;
        }

        static async Task<int> ImportUsers(IServiceProvider serviceProvider, string file)
        {
            using var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var config = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
            var apiKey = config.GetValue<string>("ApiKey");
            var apiBaseUri = config.GetValue<string>("ApiBaseUri");

            var httpClientFactory = serviceScope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ";",
            };

            using (var reader = new StreamReader(file))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                var records = csv.GetRecords<ImportUserModel>();

                foreach(var r in records)
                {
                    var email = r.EmailAddress?.Trim();
                    if (string.IsNullOrEmpty(email))
                    {
                        continue;
                    }

                    var name = r.FirstName + " " + r.LastName;

                    var payload = JsonSerializer.Serialize(new 
                    { 
                        Name = name,
                        Email = email, 
                        Roles = new[] { false, false, false, true },
                        DebtorNumber = r.DebtorNumber,
                        AddressNumber = r.AddressNumber
                    });

                    var response = await client.PostAsync(apiBaseUri + "/api/admin/user",
                        new StringContent(payload, Encoding.UTF8, "application/json"));

                    var result = await response.Content.ReadAsStringAsync();

                    File.AppendAllText(file + ".result", name + ";" + email + ";" + result + "\n");
                }
            }

            return 0;
        }
    }
}