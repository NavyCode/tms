using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Navy.MsTest;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestPlanService.Controllers;
using TestPlanService.Services.Db;
using TestPlanService.Services.Sessions;
using TestSuiteService.Controllers;
using WildBerriesApi.Tests;

namespace TestPlanService.Tests.Controllers
{
    [TestClass]
    public  class FilesControllerTests : TestClass
    {
        static FilesController controller;
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

            controller = new FilesController(LogFactory.Create<FilesController>(), sessionService, dbService);
            controller.ControllerContext = controllerContext;
        }

        [TestInitialize]
        public void TestInit()
        {
        }

        [TestMethod]
        public void UploadDownload()
        {
            var name = "TextFile1.txt";
            var file = new MemoryStream(File.ReadAllBytes(DeployPath("Sources\\TextFile1.txt")));
            var id = controller.Upload(projectId, new FormFile(file, 0, file.Length, name, name), "test").Value;

            var dbFile = controller.Download(projectId, id) as FileStreamResult;
            var result = new MemoryStream();
            dbFile.FileStream.CopyTo(result);
            var resultFile = OutputPath("TextFile1.exe");
            File.WriteAllBytes(resultFile, result.ToArray());
            var actual = File.ReadAllText(resultFile, Encoding.UTF8);
            Assert.AreEqual("Text", actual);
        }
    }
}
