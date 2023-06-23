using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Plans;
using TestPlanService.Models.Projects;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Services.Db.SubSystems
{
    public class PlansSubsystem
    {
        DatabaseService _db;

        public PlansSubsystem(DatabaseService context)
        {
            _db = context;
        }

        public TestPlan Find(int planId) => _db.Context.TestPlans.FirstOrDefault(p => p.Id == planId);

        public void UpdatePlan(TestPlan plan, AddOrUpdatePlanRequest request)
        { 
            plan.Title = request.Name;
            plan.Description = request.Description;
            plan.State = request.State;
            if(request.AssignedTo > 0)
                plan.AssignedTo = _db.Context.Users.FirstOrDefault(u => u.Id == request.AssignedTo);
            _db.Context.SaveChanges();
        }
         
        public TestPlan AddPlan(User user, Project project, AddOrUpdatePlanRequest request)
        { 
            var plan = new TestPlan()
            {
                State = PlanState.Active,
                CreatedBy = user,
                AssignedTo = user,
                Project = project,
                Title = request.Name,
                Description = request.Description,
            };
            _db.Context.TestPlans.Add(plan);

            var rootSuite = new TestSuite()
            {
                TestPlan = plan,
                Title = plan.Title,
            };
            _db.Context.TestSuites.Add(rootSuite);

            var testConfigs = _db.Context.TestConfigs.Where(p => p.Project == project && p.IsDefault).Select(p => p.Id).ToArray();
            _db.TestSuite.SetSuiteConfig(rootSuite, new Models.Suites.SetTestSuiteConfigRequest()
            {
                TestConfigIds = testConfigs
            });

            _db.Context.SaveChanges();
            UpdatePlan(plan, request);
            return plan;
        }

        internal TestPlan Dublicate(User user, TestPlan sourcePlan)
        {
            var destPlan = AddPlan(user, sourcePlan.Project, new AddOrUpdatePlanRequest()
            {
                AssignedTo = sourcePlan.AssignedTo.Id,
                Description = sourcePlan.Description,
                Name = sourcePlan.Title,
                State = sourcePlan.State
            });
            _db.Context.Update(destPlan);
            var newIds = new Dictionary<int, int>();
            var destRootSuite = destPlan.Suites.First();
            _db.Context.Update(destRootSuite);
            var sourceRootSuite = sourcePlan.Suites.First(p => p.Parent == null);
            foreach (var sourceSuite in sourcePlan.Suites.ToArray())
            {
                TestSuite destSuite = null;
                if (sourceSuite.Id == sourceRootSuite.Id)
                    destSuite = destRootSuite;
                else
                {
                    destSuite = _db.TestSuites.AddSuite(destPlan, destRootSuite, new Models.Suites.AddOrUpdateSuiteRequest()
                    {
                        Name = sourceSuite.Title,
                        ParentId = destRootSuite.Id
                    });
                }
                _db.Context.SaveChanges();
                newIds.Add(sourceSuite.Id, destSuite.Id);
                _db.TestSuite.SetSuiteConfig(destSuite, new Models.Suites.SetTestSuiteConfigRequest()
                {
                    TestConfigIds = sourceSuite.Configs.Select(p => p.Id).ToArray()
                });
                foreach (var sourceTcs in sourceSuite.TestCases)
                {
                    var destTcs = _db.TestSuite.AddExistTestCase(user, destSuite, new Models.Suites.AddTestCaseToSuiteRequest()
                    {
                        TestCasesIds = new[] { sourceTcs.TestCase.Id }
                    }).First();
                    foreach (var point in destTcs.Points.ToArray())
                        _db.Context.TestPoints.Remove(point);
                    _db.Context.SaveChanges();
                    foreach (var p in destTcs.Points)
                    {
                        var point = new TestPoint()
                        { 
                            SuiteTestCase = destTcs,
                            TestConfig = p.TestConfig,
                            Tester = p.Tester
                        };
                        _db.Context.TestPoints.Add(point);
                    }
                    _db.Context.SaveChanges();
                }
                _db.Context.SaveChanges();
            }
            foreach(var id in newIds)
            {
                var sourceSuite = _db.Context.TestSuites.First(p => p.Id == id.Key);
                if (sourceSuite.Parent != null)
                {
                    var destSuite = _db.Context.TestSuites.First(p => p.Id == id.Value);
                    destSuite.Parent = _db.Context.TestSuites.First(p => p.Id == newIds[sourceSuite.Parent.Id]);
                }
            }
            _db.Context.SaveChanges();
            return destPlan;
        }
    }
}
