using Castle.Core.Logging;
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
        private DatabaseService _db;

        public SetupService(ILogger<SetupService> logger, ConfigService config, DatabaseService db)
        { 
            _logger = logger;
            _config = config;
            _db = db;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            // todo
            //var dbContext = DatabaseContext.FromConfig(_config);
            //_db = new DatabaseService(_config.CreateLogger<DatabaseService>(), _config, dbContext);
            //return Update();

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task Update()
        {
            _logger.LogInformation("Database EnsureCreated"); 
            if (_db.Context.Database.EnsureCreated())
                new EmptyDbTemplate(_db).Fill();
            await UpdateVersion();
        }

        private async Task UpdateVersion()
        {
            _logger.LogInformation("UpdateVersion");
            var version = _db.Context.Options.FirstOrDefault(p => p.Name == "Version");
            if (version == null)
            {
                version = new Option()
                {
                    Name = "Version"
                };
                _db.Context.Options.Add(version);
            }
            version.Value = DatabaseService.Version.ToString();
            await _db.Context.SaveChangesAsync();
        }
    }
}
