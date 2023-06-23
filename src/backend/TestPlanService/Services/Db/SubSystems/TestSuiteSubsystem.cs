using System;
using System.Collections.Generic;
using System.Linq;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Suites;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Services.Db.SubSystems
{
    public class TestSuiteSubsystem
    {
        DatabaseService _db;

        public TestSuiteSubsystem(DatabaseService context)
        {
            _db = context;
        }
         
        public List<SuiteTestCase> AddExistTestCase(User user, TestSuite suite, AddTestCaseToSuiteRequest request, bool isSave = true)
        { 
            var order = suite.TestCases.Count();
            var result = new List<SuiteTestCase>();
            foreach (var id in request.TestCasesIds)
            {
                order++;
                var tc = new SuiteTestCase()
                {
                    TestSuite = suite,
                    Order = order,
                    TestCase = _db.Context.WorkItems.FirstOrDefault(p => p.Id == id),
                };
                result.Add(tc);
                _db.Context.SuiteTestCases.Add(tc); 
                foreach (var c in suite.Configs)
                {
                    var point = new TestPoint(); 
                    point.SuiteTestCase = tc;
                    point.Tester = user;
                    point.TestConfig = c.TestConfig;
                    _db.Context.TestPoints.Add(point);
                }
            }
            if(isSave)
                _db.Context.SaveChanges();
            return result;
        }

        internal void SetSuiteTestCaseConfig(User user, int projectId, SetTestCaseConfigRequest request)
        { 
            foreach (var tc in request.SuiteTestCaseIds)
            {
                var stc = _db.Context.SuiteTestCases.FirstOrDefault(p => p.Id == tc);
                if (stc == null || stc.TestSuite.TestPlan.Project.Id != projectId)
                    continue;
                stc.Points.RemoveAll(p => !request.TestConfigIds.Contains(p.TestConfig.Id));
                foreach (var config in request.TestConfigIds)
                {
                    if (stc.Points.Any(p => p.TestConfig.Id == config))
                        continue;
                    var point = new TestPoint()
                    { 
                        SuiteTestCase = stc,
                        TestConfig = _db.Context.TestConfigs.FirstOrDefault(p => p.Id == config),
                        Tester = user,
                    };
                    _db.Context.TestPoints.Add(point);
                }
            }
            _db.Context.SaveChanges();
        }

        internal void SetSuiteConfig(TestSuite suite, SetTestSuiteConfigRequest request, bool isSave = false)
        {
            var uniqueConfigs = request.TestConfigIds.Distinct().ToArray();
            foreach (var c in suite.Configs.Where(p => p.TestConfig == null || !request.TestConfigIds.Contains(p.TestConfig.Id)).ToList())
                _db.Context.TestSuiteConfigs.Remove(c);
            foreach (var config in request.TestConfigIds)
            {
                if (suite.Configs.Any(p => p.TestConfig?.Id == config))
                    continue;
                var point = new TestSuiteConfig()
                {
                    TestConfig = _db.Context.TestConfigs.FirstOrDefault(p => p.Id == config),
                    Suite = suite
                };
                _db.Context.TestSuiteConfigs.Add(point);
                if(isSave)
                    _db.Context.SaveChanges();
            }
        }
    }
}
