using System;
using Microsoft.Extensions.Logging;
using TestPlanService.Models.Authorization;
using TestPlanService.Services.Config;
using TestPlanService.Services.Db.SubSystems;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Db.Templates;

namespace TestPlanService.Services.Db
{
    public class DatabaseService
    {
        public const int Version = 1;

        internal ILogger _logger;
        private ConfigService _configuration;
        
        public DatabaseContext Context { get; set; }

        public DatabaseService(ILogger<DatabaseService> logger, ConfigService configuration, DatabaseContext context)
        {
            _logger = logger;
            _configuration = configuration;
            Context = context;
            Clear();
        }

        static bool isCleared = false;
        private void Clear()
        {
            if (!_configuration.AppSettings.TestPlan.IsClearDb)
                return;
            if (isCleared)
                return;
            isCleared = true;
            _logger.LogInformation("Clear");
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
            new EmptyDbTemplate(this).Fill();
            _logger.LogInformation("Done");
        }

        public UsersSubsystem Users => new UsersSubsystem(this); 
        public TestSuitesSubsystem TestSuites => new TestSuitesSubsystem(this);
        public ProjectsSubsystem Projects => new ProjectsSubsystem(this);
        public PlansSubsystem Plans => new PlansSubsystem(this);
        public ProjectUsersSubsystem ProjectUsers => new ProjectUsersSubsystem(this);
        public TestConfigsSubsystem TestConfigs => new TestConfigsSubsystem(this);
        public TestCasesSubsystem TestCases => new TestCasesSubsystem(this);
        public TestSuiteSubsystem TestSuite => new TestSuiteSubsystem(this);
        public AccessSubsystem Access => new AccessSubsystem(this);
        public TestRunSubsystem Runs => new TestRunSubsystem(this);
        public FilesSubsystem Files => new FilesSubsystem(this);
    }
}