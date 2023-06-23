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
using WildBerriesApi.Tests;

namespace TestPlanService.Tests.Controllers
{
    [TestClass]
    public  class ProjectsControllerTests : TestClass
    {
        static ProjectsController controller;

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

            controller = new ProjectsController(LogFactory.Create<ProjectsController>(), sessionService, dbService);
            controller.ControllerContext = controllerContext;
        }

        [TestInitialize]
        public void TestInit()
        {
        }

        [TestMethod]
        public void AddUpdateDelete()
        {
            var id = controller.AddProject(new Models.Projects.AddOrUpdateProjectRequest()
            {
                Name = "NewProject",
                Description = "Descr",
            }).Value;

            var project = controller.GetProjects().Value.Projects.First(p => p.Id == id);
            Assert.AreEqual("NewProject", project.Name);
            Assert.AreEqual("Descr", project.Comment);

            controller.UpdateProject(id, new Models.Projects.AddOrUpdateProjectRequest()
            {
                Name = "NewProject2",
                Description = "Descr2",
            });
            project = controller.GetProjects().Value.Projects.First(p => p.Id == id);
            Assert.AreEqual("NewProject2", project.Name);
            Assert.AreEqual("Descr2", project.Comment);

            controller.DeleteProject(id);
            project = controller.GetProjects().Value.Projects.FirstOrDefault(p => p.Id == id);
            Assert.IsNull(project);
        }
    }
}
