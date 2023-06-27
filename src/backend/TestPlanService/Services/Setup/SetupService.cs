using Castle.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestPlanService.Services.Config;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Templates;

namespace TestPlanService.Services.Setup
{
    public class SetupService : IHostedService
    {
         
        ILogger<SetupService> _logger;
        private ConfigService _config; 
        private readonly IServiceScopeFactory _scopeFactory;

        public SetupService(ILogger<SetupService> logger, ConfigService config, IServiceScopeFactory scopeFactory)
        { 
            _logger = logger;
            _config = config; 
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseService>();
            var db = new DatabaseService(_config.CreateLogger<DatabaseService>(), _config, dbContext.Context);
            await Update(db);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task Update(DatabaseService db)
        {
            _logger.LogInformation("Database EnsureCreated"); 
            if (db.Context.Database.EnsureCreated())
                new EmptyDbTemplate(db).Fill();
            await UpdateVersion(db);
        }

        private async Task UpdateVersion(DatabaseService db)
        {
            _logger.LogInformation("UpdateVersion");
            var version = db.Context.Options.FirstOrDefault(p => p.Name == "Version");
            if (version == null)
            {
                version = new Option()
                {
                    Name = "Version"
                };
                db.Context.Options.Add(version);
            }
            version.Value = DatabaseService.Version.ToString();
            await db.Context.SaveChangesAsync();
        }
    }
}
