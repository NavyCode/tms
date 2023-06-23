using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using TestPlanService.Services.Config;
using TestPlanService.Services.Db.Tables;
using YamlDotNet.Core.Tokens;

namespace TestPlanService.Services.Db
{
    public class DatabasePortableContext : DatabaseContext
    { 
        public DatabasePortableContext(DbContextOptions<DatabaseContext> context)
            : base(context)
        { 
        }
        string _pgConnectionString;
        string _msSqlConnectionString;

        public static DatabaseContext FromConfig(ConfigService config)
        {
            var options = new DbContextOptions<DatabaseContext>();
            var result = new DatabasePortableContext(options)
            {
                _pgConnectionString = config.AppSettings.TestPlan.PgConnectionString,
                _msSqlConnectionString = config.AppSettings.TestPlan.MsSqlConnectionString
            };

            return result;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies();
            if (_pgConnectionString != null)
                optionsBuilder.UseNpgsql(_pgConnectionString, (o) =>
                {
                    o.EnableRetryOnFailure(1);
                });
            else if (_msSqlConnectionString != null)
            {
                optionsBuilder.UseSqlServer(_msSqlConnectionString, (o) =>
                {
                    o.EnableRetryOnFailure(1);
                });
            }
        }

    }
}
