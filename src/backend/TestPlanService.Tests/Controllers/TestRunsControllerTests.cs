using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Navy.MsTest;
using System;
using System.IO;
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
    public class TestRunsControllerTests : TestClass
    { 
        static TestRunsController controller;
        static int projectId;
        private static FilesController files;

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

            files = new FilesController(LogFactory.Create<FilesController>(), sessionService, dbService);
            files.ControllerContext = controllerContext;

            controller = new TestRunsController(LogFactory.Create<TestRunsController>(), sessionService, dbService);
            controller.ControllerContext = controllerContext; 
        } 


        [TestMethod]
        public void Create()
        {
            var runId = controller.Add(projectId, new Models.Projects.AddRunRequest()
            {
                IsAutomated = true,
                Name = "Autotest.Run"
            }).Value;

            var test1 = controller.AddResult(projectId, runId, new Models.Projects.TestResultRequest()
            {
                AutomatedTestId = "AutomatedTest.Id",
                AutomatedTestName = "Test",
                AutomatedTestStorage = "Assembly.dll",
                CompletedDate= DateTime.Now,
                ComputerName = "Host",
                Description= "Desc",
                ErrorMessage = "Error",
                OutCome = Outcome.Paused,
                stackTrace = "Stack",
                StartedDate= DateTime.Now
            }).Value;

            var fileSource = new MemoryStream(System.IO.File.ReadAllBytes(DeployPath("Sources\\TextFile1.txt")));
            var file = files.Upload(projectId, new FormFile(fileSource, 0, fileSource.Length, "TextFile1.txt", "TextFile1.txt"), "run attach").Value;

            controller.AddFiles(projectId, test1, new[] { file });

            var results = controller.GetResults(projectId, runId).Value;

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(1, results.First().Files.Count); 
        }
    }
}
