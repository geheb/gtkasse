using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GtKasse.Core.Database;

public static class ServiceCollectionExtensions
{
    public static void AddSQLiteContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.ConfigureWarnings(warn => warn.Ignore(
                CoreEventId.FirstWithoutOrderByAndFilterWarning,
                CoreEventId.RowLimitingOperationWithoutOrderByWarning,
                CoreEventId.DistinctAfterOrderByWithoutRowLimitingOperatorWarning));

            var sqlite = new SqliteConnectionStringBuilder(configuration.GetConnectionString("SQLite"));
            var dir = Path.GetDirectoryName(sqlite.DataSource);
            if (!string.IsNullOrEmpty(dir))
            {
                var di = new DirectoryInfo(dir);
                if (!di.Exists)
                {
                    di.Create();
                }
            }

            options.UseSqlite(sqlite.ToString());
        });
    }
}
