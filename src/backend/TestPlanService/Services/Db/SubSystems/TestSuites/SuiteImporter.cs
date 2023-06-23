using System.IO;
using System.Xml.Linq;
using TestPlanService.Models.Export.Navy.V1;
using TestPlanService.Services.Db.Tables;
using Navy.Core.Extensions;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Reflection.PortableExecutable;
using MyNamespace;

namespace TestPlanService.Services.Db.SubSystems.TestSuites
{

    public class SuiteImporter
    {
        private DatabaseService _db;
        private readonly ILogger _logger;

        public SuiteImporter(DatabaseService db, ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        int i = 0;
        int tests = 0;
        int total = 0;
        Dictionary<string, TestConfig> _testConfigDic = new Dictionary<string, TestConfig>();

        public async Task Import(byte[] bytes, ImportType type, TestSuite parent, User user)
        {
            var xdoc = XDocument.Load(new MemoryStream(bytes));
            var export = xdoc.Root.XmlDeserialize<Export>();
            total = export.Suite.Count;
            foreach (var conf in _db.Context.TestConfigs.Where(p => p.Project == parent.TestPlan.Project)) 
                _testConfigDic[conf.Name] = conf; 
            var root = export.Suite.FirstOrDefault(p => p.Parent == null) ?? export.Suite.FirstOrDefault();

            _db.Context.ChangeTracker.AutoDetectChangesEnabled = false;

            var testRun = _db.Runs.AddRun(user, parent.TestPlan.Project, false, "Run");
            await AddSuite(export, parent, user, root, testRun);

            _db.Context.ChangeTracker.DetectChanges();
            await _db.Context.SaveChangesAsync();
            foreach (var tc in _db.Context.SuiteTestCases.Where(tc => tc.TestCase.Revision == null && tc.TestSuite.TestPlan == parent.TestPlan))
                tc.TestCase.Revision = tc.TestCase.Revisions.First();
            _db.Context.ChangeTracker.DetectChanges();
            await _db.Context.SaveChangesAsync();
            _db.Context.ChangeTracker.AutoDetectChangesEnabled = true;

            
           
        }

        private async Task AddSuite(Export export, TestSuite parent, User user, Suite importSuite, TestRun testRun)
        {
            if(i++ % 100 == 0)
            {
                _logger.LogInformation($"SaveChanges");
                _db.Context.ChangeTracker.DetectChanges();
                await _db.Context.SaveChangesAsync();
                _db.Context.ChangeTracker.DetectChanges();
                foreach (var tc in _db.Context.SuiteTestCases.Where(tc => tc.TestCase.Revision == null && tc.TestSuite.TestPlan == parent.TestPlan))
                    tc.TestCase.Revision = tc.TestCase.Revisions.First();
                _db.Context.ChangeTracker.DetectChanges();
                await _db.Context.SaveChangesAsync();
                _logger.LogInformation($"Processed Suites {i} / {total}");
            }

            var newSuite = new TestSuite()
            {
                Parent = parent,
                TestPlan = parent.TestPlan,
                Title = importSuite.Title
            };
            _db.Context.TestSuites.Add(newSuite);

            foreach (var c in parent.Configs)
            {
                var point = new TestSuiteConfig()
                {
                    TestConfig = c.TestConfig,
                    Suite = newSuite
                };
                _db.Context.TestSuiteConfigs.Add(point);
            }

            var testOrder = 1;
            foreach (var tc in importSuite.Test)
            {
                if (tests++ % 100 == 0)
                {
                    _logger.LogInformation($"Added Tests {tests}");
                }
                var automationStatus = AutomationStatus.Manual;
                Enum.TryParse(tc.AutomationStatus, true, out automationStatus);
                var state = WiState.Design;
                Enum.TryParse(tc.State, true, out state);

                var newTc = new WorkItem()
                {
                    CreatedBy = user,
                    Project = parent.TestPlan.Project,
                    Type = WiType.TestCase,
                };
                _db.Context.WorkItems.Add(newTc);
                var rev = new WorkItemRevision()
                {
                    AssignedTo = user,
                    AutomationStatus = automationStatus,
                    AutomationTestName = tc.AutomationTestName,
                    AutomationTestType = tc.AutomationTestType,
                    AutomationTestStorage = tc.AutomationTestStorage,
                    ChangeBy = user,
                    Description = tc.Description,
                    Postcondition = tc.Postcondition,
                    Precondition = tc.Precondition,
                    Priority = tc.Priority,
                    State = state,
                    Steps = null,
                    Title = tc.Title,
                };
                var stepOrder = 1;
                foreach (var s in tc.Step)
                {
                    var newStep = new TestStep
                    {
                        Order = stepOrder++,
                        Result = s.Result,
                        Action = s.Action
                    };
                    newTc.Steps.Add(newStep);
                    _db.Context.TestSteps.Add(newStep);
                }
                newTc.Revisions.Add(rev);
                _db.Context.WorkItemRevisions.Add(rev);
                 
                var newSuiteTc = new SuiteTestCase()
                {
                    TestSuite = newSuite,
                    Order = testOrder++,
                    TestCase = newTc,
                };
                _db.Context.SuiteTestCases.Add(newSuiteTc);
                foreach (var c in tc.Config)
                {
                    if (!_testConfigDic.TryGetValue(c.Title, out var newConfig))
                    { 
                        newConfig = new TestConfig()
                        {
                            Name = c.Title,
                            Project = parent.TestPlan.Project,
                            Description = "",
                            IsDefault = false,
                        };
                        _db.Context.TestConfigs.Add(newConfig);
                        _testConfigDic[c.Title] = newConfig;
                    }
                    var result = new TestResult()
                    {
                        Owner = user,
                        OutCome = c.Outcome,
                        TestRun = testRun,
                        StartedDate = System.DateTime.UtcNow,
                        CompletedDate = System.DateTime.UtcNow,
                    };
                    _db.Context.TestResults.Add(result);
                    var point = new TestPoint()
                    {
                        SuiteTestCase = newSuiteTc,
                        TestConfig = newConfig,
                        Tester = user,
                        RunResult = result
                    };
                    _db.Context.TestPoints.Add(point);
                }
            }
            foreach (var s in export.Suite.Where(p => p.Parent?.Id == importSuite.Id))
                await AddSuite(export, newSuite, user, s, testRun);
        }
    }

}