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
    public class DebugTests : TestClass
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


        [TestMethod]
        public void Debug()
        {
            var tree = planDefine.GetSuitesTree(projectId, planId).Value;
            var root = tree.Suites.Single();
                
            var file = @"D:\Temp\2\testplan.xml";
            var importPlan = planController.AddPlan(projectId, new Models.Plans.AddOrUpdatePlanRequest() { Name = "ImportPlan 2" }).Value;
            var importPlanTree = planDefine.GetSuitesTree(projectId, importPlan).Value;
            var importPlanRoot = importPlanTree.Suites.Single();
            controller.Import(projectId, importPlanRoot.Id, Models.Export.Navy.V1.ImportType.NavyV1, System.IO.File.ReadAllBytes(file)).Wait();
        }
    }
}
