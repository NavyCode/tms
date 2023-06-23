using System.Collections.Generic;
using System.Linq;
using TestPlanService.Models.Suites;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Services.Db.SubSystems
{
    public class TestRunSubsystem
    {
        DatabaseService _db;

        public TestRunSubsystem(DatabaseService context)
        {
            _db = context;
        } 

        public TestRun AddRun(User user, Project project, bool isAutomated, string name)
        {
            var run = new TestRun()
            {
                Owner = user,
                Project = project,
                StartDate = System.DateTime.UtcNow,
                IsAutomated= isAutomated,
                Name = name
            };
            _db.Context.TestRuns.Add(run);
            _db.Context.SaveChanges();
            return run;
        }

        public TestResult AddResult(User user, TestRun run, Outcome outcome)
        {
            var result = new TestResult()
            {
                Owner = user,
                OutCome = outcome,
                TestRun = run,
                StartedDate = System.DateTime.UtcNow,
                CompletedDate = System.DateTime.UtcNow,
            };
            _db.Context.TestResults.Add(result);
            _db.Context.SaveChanges();
            return result;
        }

        public File UploadFile(TestResult result, string name, byte[] data)
        {
            var file = _db.Files.Upload(result.TestRun.Project, name, data, "TestRun");
            result.Files.Add(file);
            _db.Context.SaveChanges();
            return file;
        }
    }
}
