using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Navy.MsTest;
using System;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
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
    public  class PlansControllerTests : TestClass
    {
        static PlansController controller;
        static int projectId;
        private static TestCasesController testcontroller;
        private static TestSuitesController suitesController;
        private static TestSuiteController suitecontroller;
        private static TestConfigsController configController;
        private static PlanController planController;

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

            controller = new PlansController(LogFactory.Create<PlansController>(), sessionService, dbService);
            controller.ControllerContext = controllerContext;

            testcontroller = new TestCasesController(LogFactory.Create<TestCasesController>(), sessionService, dbService);
            testcontroller.ControllerContext = controllerContext;

            planController = new PlanController(LogFactory.Create<PlanController>(), sessionService, dbService);
            planController.ControllerContext = controllerContext;

            suitesController = new TestSuitesController(LogFactory.Create<TestSuitesController>(), sessionService, dbService);
            suitesController.ControllerContext = controllerContext;

            suitecontroller = new TestSuiteController(LogFactory.Create<TestSuiteController>(), sessionService, dbService);
            suitecontroller.ControllerContext = controllerContext;

            configController = new TestConfigsController(LogFactory.Create<TestConfigsController>(), sessionService, dbService);
            configController.ControllerContext = controllerContext;
        }

        [TestInitialize]
        public void TestInit()
        {
        }

        [TestMethod]
        public void AddUpdateDelete()
        {
            var id = controller.AddPlan(projectId, new Models.Plans.AddOrUpdatePlanRequest()
            {
                Name = "NewPlan",
                Description = "Descr",
                State = PlanState.Active,
            }).Value;

            var plan = controller.GetPlans(projectId).Value.Plans.FirstOrDefault(p => p.Id == id);
            Assert.AreEqual("NewPlan", plan.Name);
            Assert.AreEqual("Descr", plan.Comment);
            Assert.AreEqual("Admin", plan.AssignedTo.Name);
            Assert.AreEqual(PlanState.Active, plan.State);


            controller.UpdatePlan(projectId, id, new Models.Plans.AddOrUpdatePlanRequest
            {
                Name = "NewPlan2",
                Description = "Descr2",
                State = PlanState.Closed, 
            });
            plan = controller.GetPlans(projectId).Value.Plans.FirstOrDefault(p => p.Id == id);
            Assert.AreEqual("NewPlan2", plan.Name);
            Assert.AreEqual("Descr2", plan.Comment);
            Assert.AreEqual(PlanState.Closed, plan.State);

            controller.DeletePlan(projectId, id);
            plan = controller.GetPlans(projectId).Value.Plans.FirstOrDefault(p => p.Id == id);
            Assert.IsNull(plan);
        }


        [TestMethod]
        public void Dublicate()
        {
            var c1 = configController.AddConfig(projectId, new Models.Suites.AddOrUpdateConfigsRequest()
            {
                Name = "C1",
                IsDefault = true
            }).Value;
            var c2 = configController.AddConfig(projectId, new Models.Suites.AddOrUpdateConfigsRequest()
            {
                Name = "C2",
                IsDefault = true
            }).Value;


            var planId = controller.AddPlan(projectId, new Models.Plans.AddOrUpdatePlanRequest()
            {
                Name = "NewPlan",
                Description = "Descr",
                State = PlanState.Active, 
            }).Value; 
            var tree = planController.GetSuitesTree(projectId, planId).Value;
            var root = tree.Suites.Single();
            suitecontroller.SetSuiteConfig(projectId, root.Id, new Models.Suites.SetTestSuiteConfigRequest()
            {
                TestConfigIds = new[]{ c1, c2 }
            });

            var s1 = suitesController.AddSuite(projectId, planId, new Models.Suites.AddOrUpdateSuiteRequest()
            {
                Name = "1",
                ParentId = root.Id
            }).Value;
            var s2 = suitesController.AddSuite(projectId, planId, new Models.Suites.AddOrUpdateSuiteRequest()
            {
                Name = "1.1",
                ParentId = s1
            }).Value;


            var testCase1 = testcontroller.AddTest(projectId, new Models.TestCases.AddOrUpdateTestCaseRequest()
            {
                Title = "T1"
            }).Value;
            var testCase2 = testcontroller.AddTest(projectId, new Models.TestCases.AddOrUpdateTestCaseRequest()
            {
                Title = "T1"
            }).Value;

            suitecontroller.AddTestCaseSuite(projectId, s1, new Models.Suites.AddTestCaseToSuiteRequest()
            {
                TestCasesIds = new[] { testCase1, testCase2 }
            });
            suitecontroller.AddTestCaseSuite(projectId, s2, new Models.Suites.AddTestCaseToSuiteRequest()
            {
                TestCasesIds = new[] { testCase1, testCase2 }
            });

            controller.DublicatePlan(projectId, planId);
        }


    }
}
