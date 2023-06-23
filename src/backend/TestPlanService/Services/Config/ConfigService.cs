using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; 

namespace TestPlanService.Services.Config
{
    public class ConfigService
    {
        private IConfiguration _configuration;
        private AppSettings _settings;

        public ConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
            var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            if (appSettings == null)
                throw new Exception("Отсутствует файл конфигурации");
            _settings = appSettings;
            ReplaceByEnvironment();
            ReplaceByCommandLine();
        }

        private void ReplaceByEnvironment()
        {
            var pgConnectionString = Environment.GetEnvironmentVariable("PG_CONNECTION_STRING");
            if (pgConnectionString != null)
                _settings.TestPlan.PgConnectionString = pgConnectionString;
        }

        private void ReplaceByCommandLine()
        {
            var args = new Navy.Core.Applications.CommandLineArgs(Environment.CommandLine);
            _settings.TestPlan.IsClearDb = args.FindValue<bool>("cleardb") ?? _settings.TestPlan.IsClearDb;
             
        }

        public ConfigService(AppSettings settings)
        {
            _settings = settings;
        }

        public string GetSourceFilePath(string path) => GetPath("Sources", path);

        public string GetHistoryFilePath(string path) => GetPath("History", path);

        public string GetTempFilePath(string path) => GetPath("Temp", path);

        public string GetReportPath(string path) => GetPath("../Reports", path);

        public static string AssemblyDirectory => Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
        public string GetPath(string subdir, string path)
        {
            var result = Path.Combine(AssemblyDirectory, subdir, path);
            if (!Directory.Exists(result))
                Directory.CreateDirectory(Path.GetDirectoryName(result));
            return result;
        }

        public AppSettings AppSettings => _settings;

        public ILogger<T> CreateLogger<T>()
        {
            return GetFactory().CreateLogger<T>();
        }

        public ILoggerFactory GetFactory()
        {
            return LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole()
                    .AddDebug();
            });
        }
    }
}