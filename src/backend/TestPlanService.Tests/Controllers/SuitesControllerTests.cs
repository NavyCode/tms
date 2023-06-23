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
    public  class PlanTestSuitesControllerTests : TestClass
    {
        static TestSuitesController controller;
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
            planController.ControllerContext = controllerContext;

            planId = planController.AddPlan(projectId, new Models.Plans.AddOrUpdatePlanRequest() { Name = "NewPlan" }).Value;

            planDefine = new PlanController(LogFactory.Create<PlanController>(), sessionService, dbService);
            planDefine.ControllerContext = controllerContext;
             
            testcontroller = new TestCasesController(LogFactory.Create<TestCasesController>(), sessionService, dbService);
            testcontroller.ControllerContext = controllerContext;

            controller = new TestSuitesController(LogFactory.Create<TestSuitesController>(), sessionService, dbService);
            controller.ControllerContext = controllerContext;
        }

        [TestInitialize]
        public void TestInit()
        {
        }

        [TestMethod]
        public void AddUpdateDeleteSuite()
        {
            var tree = planDefine.GetSuitesTree(projectId, planId).Value;
            var root = tree.Suites.Single();

            var id = controller.AddSuite(projectId, planId, new Models.Suites.AddOrUpdateSuiteRequest()
            {
                Name = "1",
                ParentId = root.Id
            }).Value;

            var suite = controller.GetChildrenSuites(projectId, root.Id).Value.Suites.First(p => p.Id == id);
            tree = planDefine.GetSuitesTree(projectId, planId).Value;

            Assert.AreEqual("1", suite.Name);
            Assert.AreEqual(root.Id, suite.ParentId);
            Assert.AreEqual(2, tree.Suites.Count); 

            controller.UpdateSuite(projectId, id, new Models.Suites.AddOrUpdateSuiteRequest()
            {
                Name = "11",
                ParentId = root.Id
            });
            suite = controller.GetChildrenSuites(projectId, root.Id).Value.Suites.First(p => p.Id == id);
            Assert.AreEqual("11", suite.Name);

            controller.DeleteSuite(projectId, id);
            suite = controller.GetChildrenSuites(projectId, root.Id).Value.Suites.FirstOrDefault(p => p.Id == id);
            Assert.IsNull(suite);
        }

        [TestMethod]
        public void MoveSuite()
        {
            var tree = planDefine.GetSuitesTree(projectId, planId).Value;
            var root = tree.Suites.Single();

            var id1 = controller.AddSuite(projectId, planId, new Models.Suites.AddOrUpdateSuiteRequest()
            {
                Name = "1",
                ParentId = root.Id
            }).Value;

            var id2 = controller.AddSuite(projectId, planId, new Models.Suites.AddOrUpdateSuiteRequest()
            {
                Name = "2",
                ParentId = root.Id
            }).Value; 

            var id3 = controller.AddSuite(projectId, planId, new Models.Suites.AddOrUpdateSuiteRequest()
            {
                Name = "3",
                ParentId = root.Id
            }).Value;

            Assert.IsNotNull( controller.GetChildrenSuites(projectId, root.Id).Value.Suites.FirstOrDefault(p => p.Id == id1)); 
            controller.MoveSuite(projectId, id1, new Models.Suites.MoveSuiteRequest()
            {
                NewParentId = id3
            });


            Assert.IsNull(controller.GetChildrenSuites(projectId, root.Id).Value.Suites.FirstOrDefault(p => p.Id == id1));
            Assert.IsNotNull(controller.GetChildrenSuites(projectId, id3).Value.Suites.FirstOrDefault(p => p.Id == id1));
        }

    }
}
