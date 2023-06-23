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
    public  class ConfigsControllerTests : TestClass
    {
        static TestConfigsController controller;
        static int projectId;

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

            controller = new TestConfigsController(LogFactory.Create<TestConfigsController>(), sessionService, dbService);
            controller.ControllerContext = controllerContext;
        }

        [TestInitialize]
        public void TestInit()
        {
        }

        [TestMethod]
        public void AddUpdateDeleteConfigs()
        {
            var id = controller.AddConfig(projectId, new Models.Suites.AddOrUpdateConfigsRequest()
            {
                Comment = "NewComment",
                IsDefault = false,
                Name = "NewName",
            }).Value;

            var config = controller.GetConfigs(projectId).Value.Configs.FirstOrDefault(p => p.Id == id);
            Assert.AreEqual("NewName", config.Name);
            Assert.AreEqual("NewComment", config.Comment);
            Assert.AreEqual(false, config.IsDefault);


            controller.UpdateConfig(projectId, id, new Models.Suites.AddOrUpdateConfigsRequest
            {
                Name = "NewName2",
                Comment = "NewComment2",
                IsDefault = true,
            });
             config = controller.GetConfigs(projectId).Value.Configs.FirstOrDefault(p => p.Id == id);
            Assert.AreEqual("NewName2", config.Name);
            Assert.AreEqual("NewComment2", config.Comment);
            Assert.AreEqual(true, config.IsDefault);

            controller.DeleteConfig(projectId, id); 
            config = controller.GetConfigs(projectId).Value.Configs.FirstOrDefault(p => p.Id == id);
            Assert.IsNull(config);
        }
    }
}
