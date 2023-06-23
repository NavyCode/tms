using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Services.Config;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Templates;
using TestPlanService.Services.Scheduling;

namespace TestPlanService.Services.Cleaning
{
    public class CleaningService : ISchedulerWorker
    {
        public ConfigService Config { get; private set; }
        private readonly ILogger _logger;

        public CleaningService(ILogger<CleaningService> logger, ConfigService config)
        {
            _logger = logger;
            Config = config;
        }

        public async Task Start()
        {
            _logger.LogInformation($"Start");
            await ClearDbAsync();
            _logger.LogInformation($"Done");
        }

        private async Task ClearDbAsync()
        {
            await Task.CompletedTask;
           // using var db = DatabaseContext.FromConfig(Config);
            //var demoUser = new DemoProjectTemplate(db, _logger);
            //await demoUser.FillAsync();
        } 
    }
}
