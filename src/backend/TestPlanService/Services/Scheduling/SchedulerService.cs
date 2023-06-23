using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyNamespace;
using TestPlanService.Services.Config;

namespace TestPlanService.Services.Scheduling
{
    public class SchedulerService : IHostedService
    {
        private readonly ICollection<ISchedulerWorker> _workers;
        private readonly ILogger<SchedulerService> _logger;
        private readonly ConfigService _config;
        private readonly DateTime[] _syncsPoint = {
            new DateTime(1,1,1,03,0,0),
#if DEBUG
           // new DateTime(1,1,1,22,21,0),
#endif
        };

        public SchedulerService(ILogger<SchedulerService> logger, IEnumerable<ISchedulerWorker> workers, ConfigService config)
        {
            _logger = logger;
            _config = config;
            _workers = workers.ToArray();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ShowInfo();
            Task.Run(() => Do(cancellationToken), cancellationToken)
                .ContinueWith((t) =>
                {
                    _logger.LogInformation("Task was exited");
                });
#if !DEBUG
            Task.Run(() => Wakeup(cancellationToken), cancellationToken)
                .ContinueWith((t) =>
                {
                    _logger.LogInformation("Task was exited");
                });
#endif
            return Task.FromResult(true);
        }

        private void ShowInfo()
        {
            foreach (var w in _syncsPoint)
                _logger.LogInformation($"SyncPoint: {w.TimeOfDay}");
            foreach (var w in _workers)
                _logger.LogInformation($"Schedule worker: {w.GetType().Name}");
        }

        private async Task Wakeup(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
#if !DEBUG
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
#endif
                    _logger.LogInformation($"Wakeup");
                    using var httpClient = new HttpClient();
                    var client = new TestPlanClient(httpClient);
                    // todo
                    //client.BaseUrl = _config.AppSettings.TestPlan.Url;
                    await client.Wakeup_PingAsync();
                }
                catch (Exception err)
                {
                    _logger.LogError(err, "Schedyle service cicle error");
                }
            }
        }


        private async Task Do(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var nextInterval = GetNextInterval();
                    _logger.LogInformation($"Next sync point after {nextInterval} at {DateTime.Now.Add(nextInterval)}");
                    await Task.Delay(nextInterval, cancellationToken);
                    _logger.LogInformation($"Start");
                    foreach (var schedulerWorker in _workers)
                    {
                        try
                        {
                            _logger.LogInformation(schedulerWorker.GetType().Name);
                            await schedulerWorker.Start();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error on run schedule worker");
                        }
                    }
                    _logger.LogInformation($"End");
                }
                catch (Exception err)
                {
                    _logger.LogError(err, "Schedyle service cicle error");
                }
            }
        }

        private TimeSpan GetNextInterval()
        {
            var now = DateTime.Now;
            var maxInterval = TimeSpan.MaxValue;
            foreach (var dateTime in _syncsPoint)
            {
                var dt = new DateTime(now.Year, now.Month, now.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
                if (now > dt)
                {
                    var now1 = now.AddDays(1);
                    dt = new DateTime(now1.Year, now1.Month, now1.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
                }

                var interval = dt - now;
                if (interval < maxInterval)
                {
                    maxInterval = interval;
                }
            }
            return maxInterval;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop");
            return Task.FromResult(true);
        }
    }

}
