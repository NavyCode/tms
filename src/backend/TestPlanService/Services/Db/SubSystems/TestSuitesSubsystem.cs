using System;
using System.Collections.Generic;
using System.Linq;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Suites;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Services.Db.SubSystems
{
    public class TestSuitesSubsystem
    {
        DatabaseService _db;

        public TestSuitesSubsystem(DatabaseService context)
        {
            _db = context;
        }

        public TestSuite AddSuite(TestPlan plan, TestSuite parent, AddOrUpdateSuiteRequest request, bool isSave = true)
        {
            var suite = new TestSuite()
            {
                Parent = parent,
                TestPlan = plan
            };
            plan.Suites.Add(suite);
            _db.Context.TestSuites.Add(suite);
            UpdateSuite(suite, request, isSave);
            if (parent != null)
            {
                _db.TestSuite.SetSuiteConfig(suite, new SetTestSuiteConfigRequest()
                {
                    TestConfigIds = parent.Configs.Select(p => p.Id).ToArray()
                });
            }
            if (isSave)
                _db.Context.SaveChanges();
            return suite;
        }

        public void UpdateSuite(TestSuite suite, AddOrUpdateSuiteRequest request, bool isSave = true)
        {
            suite.Title = request.Name;
            if(isSave)
                _db.Context.SaveChanges();
        }

        internal HashSet<int> GetSuiteIds(TestSuite dbSuite, bool withCildrenSuites)
        {
            var tree = new HashSet<(int id, int? parentId)>();
            var plan = _db.Plans.Find(dbSuite.TestPlan.Id);
            var response = new GetSuitesTreeResponse();
            foreach (var s in _db.Context.TestSuites.Where(p => p.TestPlan == plan))
                tree.Add((s.Id, s.Parent?.Id));

            var result = new HashSet<int>() { dbSuite.Id };
            if (withCildrenSuites)
            { 
                var newParents = new HashSet<int>() { dbSuite.Id };
                while (newParents.Any())
                {
                    newParents = tree.Where(p => newParents.Contains(p.parentId ?? 0)).Select(p => p.id).ToHashSet();
                    foreach (var id in newParents)
                        result.Add(id);
                }
            }
            return result;
        }
    }
}
