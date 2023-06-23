using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Navy.MsTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Controllers;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;
using TestSuiteService.Controllers;
using WildBerriesApi.Tests;

namespace TestPlanService.Tests.Controllers
{
    [TestClass]
    public class SuiteControllerTests : TestClass
    {
        static TestSuiteController controller;
        static TestSuitesController suitesController;
        static PlanController planDefine;
        static int projectId;
        private static PlansController planController;
        static int planId;
        private static TestConfigsController configController;
        private static TestCasesController testcontroller;
        private static int userId;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var config = ConfigServiceFactory.ExistDb();
            var dbService = new DatabaseService(LogFactory.Create<DatabaseService>(), config, DatabasePortableContext.FromConfig(config));
            var sessionService = new AccessService(LogFactory.Create<AccessService>(), dbService);

            var userCtrl = new UsersController(LogFactory.Create<UsersController>(), sessionService, dbService);
            var session = userCtrl.Login("admin", "admin").Result.Value;
            var controllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
            controllerContext.HttpContext.Request.Headers.Add("session", session);
            userCtrl.ControllerContext = controllerContext;
            userId = userCtrl.UserInfo().Value.Id;

            var projController = new ProjectsController(LogFactory.Create<ProjectsController>(), sessionService, dbService);
            projController.ControllerContext = controllerContext;
            projectId = projController.AddProject(new Models.Projects.AddOrUpdateProjectRequest() { Name = nameof(SuiteControllerTests) }).Value;

            planController = new PlansController(LogFactory.Create<PlansController>(), sessionService, dbService);
            planController.ControllerContext = controllerContext;

            configController = new TestConfigsController(LogFactory.Create<TestConfigsController>(), sessionService, dbService);
            configController.ControllerContext = controllerContext;

            planId = planController.AddPlan(projectId, new Models.Plans.AddOrUpdatePlanRequest() { Name = "NewPlan" }).Value;

            planDefine = new PlanController(LogFactory.Create<PlanController>(), sessionService, dbService);
            planDefine.ControllerContext = controllerContext;

            testcontroller = new TestCasesController(LogFactory.Create<TestCasesController>(), sessionService, dbService);
            testcontroller.ControllerContext = controllerContext;

            suitesController = new TestSuitesController(LogFactory.Create<TestSuitesController>(), sessionService, dbService);
            suitesController.ControllerContext = controllerContext;

            controller = new TestSuiteController(LogFactory.Create<TestSuiteController>(), sessionService, dbService);
            controller.ControllerContext = controllerContext;
        }

        [TestInitialize]
        public void TestInit()
        {
        }


        [TestMethod]
        public void TestCaseAddUpdateDelete()
        {
            var tree = planDefine.GetSuitesTree(projectId, planId).Value;
            var root = tree.Suites.Single();

            var c1 = configController.AddConfig(projectId, new Models.Suites.AddOrUpdateConfigsRequest()
            {
                Name = "C1"
            }).Value;
            var c2 = configController.AddConfig(projectId, new Models.Suites.AddOrUpdateConfigsRequest()
            {
                Name = "C2"
            }).Value;

            var id = suitesController.AddSuite(projectId, planId, new Models.Suites.AddOrUpdateSuiteRequest()
            {
                Name = "1",
                ParentId = root.Id
            }).Value;

            var suite = suitesController.GetChildrenSuites(projectId, root.Id).Value.Suites.First(p => p.Id == id);
            controller.SetSuiteConfig(projectId, suite.Id, new Models.Suites.SetTestSuiteConfigRequest()
            {
                TestConfigIds = new[] { c1, c2 }
            });

            var testCase = testcontroller.AddTest(projectId, new Models.TestCases.AddOrUpdateTestCaseRequest()
            {
                Priority = 1,
                Title = "Title",
                State = WiState.Ready
            }).Value;

            controller.AddTestCaseSuite(projectId, id, new Models.Suites.AddTestCaseToSuiteRequest()
            {
                TestCasesIds = new[] { testCase }
            });

            var tests = controller.GetSuiteTestCases(projectId, id, false).Value;
            Assert.AreEqual(1, tests.Tests.Count);
            var test = tests.Tests[0];
            Assert.AreEqual("Title", test.Name);
            Assert.AreEqual(1, test.Order);
            Assert.AreEqual(1, test.Priority);
            Assert.AreEqual(WiState.Ready, test.State);

            controller.DeleteTestCaseFromSuite(projectId, new Models.Suites.DeleteTestCaseFromSuiteRequest()
            {
                SuiteTestCasesIds = tests.Tests.Select(p => p.Id).ToArray()
            });
            tests = controller.GetSuiteTestCases(projectId, id, false).Value;
            Assert.AreEqual(0, tests.Tests.Count);
        }

        [TestMethod]
        public void GetConfigsForAssign()
        {
            var c1 = configController.AddConfig(projectId, new Models.Suites.AddOrUpdateConfigsRequest()
            {
                Name = "C1"
            }).Value;
            var c2 = configController.AddConfig(projectId, new Models.Suites.AddOrUpdateConfigsRequest()
            {
                Name = "C2"
            }).Value;
            var configs = controller.GetConfigsForAssign(projectId).Value; 
            var c = configs.Configs.First(p => p.Id == c1);
            Assert.AreEqual("C1", c.Name);
        }

        [TestMethod]
        public void ExportImport()
        {
            var tree = planDefine.GetSuitesTree(projectId, planId).Value;
            var root = tree.Suites.Single();

            var c1 = configController.AddConfig(projectId, new Models.Suites.AddOrUpdateConfigsRequest()
            {
                Name = "C1"
            }).Value;
            var c2 = configController.AddConfig(projectId, new Models.Suites.AddOrUpdateConfigsRequest()
            {
                Name = "C2"
            }).Value;

            var s1 = suitesController.AddSuite(projectId, planId, new Models.Suites.AddOrUpdateSuiteRequest()
            {
                Name = "S1",
                ParentId = root.Id
            }).Value;
            var s2 = suitesController.AddSuite(projectId, planId, new Models.Suites.AddOrUpdateSuiteRequest()
            {
                Name = "S2",
                ParentId = s1
            }).Value;

            controller.SetSuiteConfig(projectId, s1, new Models.Suites.SetTestSuiteConfigRequest()
            {
                TestConfigIds = new[] { c1, c2 }
            });
            controller.SetSuiteConfig(projectId, s2, new Models.Suites.SetTestSuiteConfigRequest()
            {
                TestConfigIds = new[] { c2 }
            });

            var t1 = testcontroller.AddTest(projectId, new Models.TestCases.AddOrUpdateTestCaseRequest()
            {
                Priority = 1,
                Title = "T1",
                State = WiState.Ready,
                Steps = new List<Models.TestCases.AddOrUpdateTestStep>()
                {
                    new Models.TestCases.AddOrUpdateTestStep()
                    {
                        Action = "A1",
                        Result = "R1"
                    }
                },
                AssignedTo = userId
            }).Value;
            var t2 = testcontroller.AddTest(projectId, new Models.TestCases.AddOrUpdateTestCaseRequest()
            {
                Priority = 1,
                Title = "T2",
                State = WiState.Ready,
                AssignedTo = userId
            }).Value;

            controller.AddTestCaseSuite(projectId, s1, new Models.Suites.AddTestCaseToSuiteRequest()
            {
                TestCasesIds = new[] { t1, t2 }
            });
            controller.AddTestCaseSuite(projectId, s2, new Models.Suites.AddTestCaseToSuiteRequest()
            {
                TestCasesIds = new[] { t2 }
            });

            var response = controller.Export(projectId, root.Id).FileContents;
            var expectedFile = OutputPath("export.xml");
             
            System.IO.File.WriteAllBytes(expectedFile, response);
            TestContext.AddResultFile(expectedFile);

            var importPlan = planController.AddPlan(projectId, new Models.Plans.AddOrUpdatePlanRequest() { Name = "ImportPlan" }).Value;
            var importPlanTree = planDefine.GetSuitesTree(projectId, importPlan).Value;
            var importPlanRoot = importPlanTree.Suites.Single();
            controller.Import(projectId, importPlanRoot.Id, Models.Export.Navy.V1.ImportType.NavyV1, System.IO.File.ReadAllBytes(expectedFile)).Wait();

            importPlanTree = planDefine.GetSuitesTree(projectId, importPlan).Value;
            Assert.AreEqual(4, importPlanTree.Suites.Count);
            var importS1 = importPlanTree.Suites.Single(p => p.Name == "S1");
            var importTests = controller.GetSuiteTestCases(projectId, importS1.Id, false).Value;
            Assert.AreEqual(2, importTests.Tests.Count);
            var tc1 = testcontroller.GetTestCase(projectId, importTests.Tests.First(p => p.Name == "T1").TestCaseId).Value;
            Assert.AreEqual(1, tc1.Steps.Count);
            Assert.AreEqual("A1", tc1.Steps[0].Action);
            Assert.AreEqual("R1", tc1.Steps[0].Result);
        }
    }
}
