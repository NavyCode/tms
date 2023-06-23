using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Navy.MsTest;
using System;
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
    public class TestPointsControllerTests : TestClass
    {
        static TestPointsController controller;
        static TestSuiteController suiteController;
        static TestSuitesController suitesController;
        static PlanController planDefine;
        static int projectId;
        static int planId;
        private static TestConfigsController configController;
        private static TestCasesController testcontroller;

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

            var projController = new ProjectsController(LogFactory.Create<ProjectsController>(), sessionService, dbService);
            projController.ControllerContext = controllerContext;
            projectId = projController.AddProject(new Models.Projects.AddOrUpdateProjectRequest() { }).Value;

            var planController = new PlansController(LogFactory.Create<PlansController>(), sessionService, dbService);
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

            suiteController = new TestSuiteController(LogFactory.Create<TestSuiteController>(), sessionService, dbService);
            suiteController.ControllerContext = controllerContext;

            controller = new TestPointsController(LogFactory.Create<TestPointsController>(), sessionService, dbService);
            controller.ControllerContext = controllerContext;


        }

        [TestInitialize]
        public void TestInit()
        {
        }


        [TestMethod]
        public void ExecuteTest()
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
            suiteController.SetSuiteConfig(projectId, suite.Id, new Models.Suites.SetTestSuiteConfigRequest()
            {
                TestConfigIds = new[] { c1, c2 }
            });

            var testCaseId = testcontroller.AddTest(projectId, new Models.TestCases.AddOrUpdateTestCaseRequest()
            {
                Priority = 1,
                Title = "Title",
                State = WiState.Ready
            }).Value;

            suiteController.AddTestCaseSuite(projectId, id, new Models.Suites.AddTestCaseToSuiteRequest()
            {
                TestCasesIds = new[] { testCaseId }
            });

            var points = controller.GetSuiteTestPoints(projectId, id, false).Value;
            Assert.AreEqual(2, points.Points.Count);
            var p = points.Points.Single(p => p.TestConfigId == c1);
            Assert.AreEqual(Outcome.Planed, p.Outcome);

            controller.SetOutcome(projectId, new Models.Suites.SetOutcomeRequest()
            {
                TestPointIds = new[] { p.Id },
                Outcome = Outcome.Failed
            });
            controller.SetComment(projectId, new Models.Suites.SetDescriptionRequest()
            {
                TestPointIds = new[] { p.Id },
                Description = "Desc"
            });

            points = controller.GetSuiteTestPoints(projectId, id, false).Value;
            p = points.Points.Single(p => p.TestConfigId == c1);
            Assert.AreEqual(Outcome.Failed, p.Outcome);
            Assert.AreEqual("Desc", p.Description);



        }

    }
}
