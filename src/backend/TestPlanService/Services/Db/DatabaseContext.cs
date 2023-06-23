using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using TestPlanService.Services.Config;
using TestPlanService.Services.Db.Tables;
using YamlDotNet.Core.Tokens;

namespace TestPlanService.Services.Db
{
    public class DatabaseContext : DbContext
    {
#if DEBUG
        public Guid Id = Guid.NewGuid();
#endif
        public DatabaseContext(DbContextOptions<DatabaseContext> context)
            : base(context)
        { 
        }
         
        public DbSet<User> Users { get; set; } 
        public DbSet<Session> Sessions { get; set; }

        public DbSet<TestConfigVariable> TestConfigVariables { get; set; }

        public DbSet<TestConfigVariableParam> TestConfigVariableValues { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<SuiteTestCase> SuiteTestCases { get; set; }

        public DbSet<TestConfig> TestConfigs { get; set; }

        public DbSet<TestConfigParam> TestConfigParams { get; set; }

        public DbSet<TestPlan> TestPlans { get; set; }

        public DbSet<TestSuite> TestSuites { get; set; }

        public DbSet<WorkItem> WorkItems { get; set; } 
        public DbSet<WorkItemRevision> WorkItemRevisions { get; set; } 
        public DbSet<TestRun> TestRuns { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<TestSuiteConfig> TestSuiteConfigs { get; set; }
        public DbSet<TestStep> TestSteps { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Option> Options { get; set; }


        public DbSet<TestPoint> TestPoints { get; set; } 

        internal static void Setup(DbContextOptionsBuilder optionsBuilder, ConfigService config)
        {
            optionsBuilder
                .UseLazyLoadingProxies();
            if (config.AppSettings.TestPlan.PgConnectionString != null)
                optionsBuilder.UseNpgsql(config.AppSettings.TestPlan.PgConnectionString, (o) =>
                {
                    o.EnableRetryOnFailure(1);
                });
            else if (config.AppSettings.TestPlan.MsSqlConnectionString != null)
            {
                optionsBuilder.UseSqlServer(config.AppSettings.TestPlan.MsSqlConnectionString, (o) =>
                {
                    o.EnableRetryOnFailure(1);
                });
            }
        }
    }
}
