using System.IO;
using System.Xml.Linq;
using TestPlanService.Models.Export.Navy.V1;
using TestPlanService.Services.Db.Tables;
using Navy.Core.Extensions;

namespace TestPlanService.Services.Db.SubSystems.TestSuites
{

    public class SuiteExporter
    {
        private DatabaseService _db;

        public SuiteExporter(DatabaseService db)
        {
            _db = db;
        }

        public byte[] Create(TestSuite dbSuite)
        {
            HashSet<int> suiteIds = _db.TestSuites.GetSuiteIds(dbSuite, true);

            var response = new Export()
            {
                Version = 1,
                Suite = new List<Suite>()
            };
             

            foreach (var suite in _db.Context.TestSuites.Where(p => suiteIds.Contains(p.Id)))
            {
                var rsuite = new Suite()
                {
                    Id = suite.Id, 
                    Title = suite.Title,
                    Test = new List<Test>(),
                };
                if(suite.Parent != null && suite != dbSuite)
                {
                    rsuite.Parent = new Parent()
                    {
                        Id = suite.Parent.Id,
                    };
                }
                foreach (var tc in suite.TestCases)
                {
                    var rTest = new Test()
                    {
                        Id = tc.Id,
                        Config = new List<Models.Export.Navy.V1.Config>(),
                        Step = new List<Step>(),
                        Title = tc.TestCase.Revision.Title,
                        AssignedTo = tc.TestCase.Revision.AssignedTo?.Name,
                        AutomationStatus = tc.TestCase.Revision.AutomationStatus.ToString(),
                        AutomationTestName = tc.TestCase.Revision.AutomationTestName,
                        AutomationTestStorage = tc.TestCase.Revision.AutomationTestStorage,
                        AutomationTestType = tc.TestCase.Revision.AutomationTestType,
                        Description= tc.TestCase.Revision.Description,
                        Postcondition= tc.TestCase.Revision.Postcondition,
                        Precondition= tc.TestCase.Revision.Precondition,
                        Priority = tc.TestCase.Revision.Priority,
                        State = tc.TestCase.Revision.State.ToString(),
                    };
                    rsuite.Test.Add(rTest);
                    foreach(var s in tc.TestCase.Steps)
                    {
                        rTest.Step.Add(new Step()
                        {
                            Action = s.Action,
                            Result = s.Result,
                        });
                    }
                    foreach (var p in tc.Points)
                    {
                        rTest.Config.Add(new Models.Export.Navy.V1.Config() { 
                            Id = p.TestConfig.Id, 
                            Title = p.TestConfig.Name,
                            Tester = p.Tester.Name
                        });
                    }
                }
                response.Suite.Add(rsuite);
            };

            var xml = response.XmlSerialize();
            var doc = XDocument.Parse(xml);
            var result = new MemoryStream();
            doc.Save(result);
            return result.ToArray();
        }
    }

}