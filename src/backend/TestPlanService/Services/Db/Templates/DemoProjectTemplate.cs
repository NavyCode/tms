using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Services.Db.Templates
{
    public class DemoProjectTemplate
    {
        public DatabaseService _db;

        private readonly ILogger _logger;

        public DemoProjectTemplate(DatabaseService db, ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public void CreateDemoProject(User owner)
        {
            _logger.LogInformation("Fill template project");
            var project = _db.Projects.AddProject(owner, new Models.Projects.AddOrUpdateProjectRequest()
            {
                Name = "Demo project",
                Description = "Demo project with all the features",
            }); 
            var configVar = _db.TestConfigs.AddConfigVariable(project, new Models.Suites.AddOrUpdateConfigVarRequest()
            {
                Name = "OS",
                Comment = "Operation system",
                Values = new System.Collections.Generic.List<Models.Suites.AddOrUpdateConfigVarValue>()
                {
                    new Models.Suites.AddOrUpdateConfigVarValue()
                    {
                        Value = "Windows 10",
                    },
                    new Models.Suites.AddOrUpdateConfigVarValue()
                    {
                        Value = "Linux",
                    }
                }
            });
            _db.TestConfigs.AddConfig(project, new Models.Suites.AddOrUpdateConfigsRequest()
            {
                Name = "Windows 10",
                IsDefault = true,
                Variables = new System.Collections.Generic.List<Models.Suites.AddOrUpdateConfigVariable>()
                {
                    new Models.Suites.AddOrUpdateConfigVariable()
                    {
                        VariableId = configVar.Id,
                        ValueId = configVar.Params.First().Id,
                    }
                }
            });
            _db.TestConfigs.AddConfig(project, new Models.Suites.AddOrUpdateConfigsRequest()
            {
                Name = "Linux",
                IsDefault = true,
                Variables = new System.Collections.Generic.List<Models.Suites.AddOrUpdateConfigVariable>()
                {
                    new Models.Suites.AddOrUpdateConfigVariable()
                    {
                        VariableId = configVar.Id,
                        ValueId = configVar.Params.First().Id,
                    }
                }
            });

            var plan = _db.Plans.AddPlan(owner, project, new Models.Plans.AddOrUpdatePlanRequest()
            {
                Name = "Demo test plan. Version 1",
                Description = "Demo plan with all the features",
                State = PlanState.Active,
                AssignedTo = owner.Id,
            });

            var rootSuite = plan.Suites.First();
            var suite1 = _db.TestSuites.AddSuite(plan, plan.Suites.First(), new Models.Suites.AddOrUpdateSuiteRequest()
            {
                Name = "Suite 1"
            });

            var suite11 = _db.TestSuites.AddSuite(plan, suite1, new Models.Suites.AddOrUpdateSuiteRequest()
            {
                Name = "Subsuite 1.1"
            });

            var suite12 = _db.TestSuites.AddSuite(plan, suite1, new Models.Suites.AddOrUpdateSuiteRequest()
            {
                Name = "Subsuite 1.2"
            });

            var tc = _db.TestCases.AddTest(owner, project, new Models.TestCases.AddOrUpdateTestCaseRequest()
            {
                AssignedTo = owner.Id,
                AutomationStatus = AutomationStatus.Manual,
                Description = "Test description",
                Postcondition = "Postcondition",
                Precondition = "Postcondition",
                Priority = 1,
                State = WiState.Ready,
                Title = "Test 1",
                Steps = new System.Collections.Generic.List<Models.TestCases.AddOrUpdateTestStep>()
                {
                    new Models.TestCases.AddOrUpdateTestStep()
                    {
                        Action = "Open notepad",
                        Result = "Notepad opened"
                    },
                    new Models.TestCases.AddOrUpdateTestStep()
                    {
                        Action = "Write down a few thoughts, save",
                        Result = "Saved"
                    },
                }
            });

            _db.TestSuite.AddExistTestCase(owner, rootSuite, new Models.Suites.AddTestCaseToSuiteRequest()
            {
                TestCasesIds = new[] { tc.Id }
            });
        }
    }
}
