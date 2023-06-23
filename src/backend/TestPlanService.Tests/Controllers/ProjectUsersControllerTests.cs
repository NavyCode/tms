using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Navy.MsTest;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Controllers;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.ProjectUsers;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;
using WildBerriesApi.Tests;

namespace TestPlanService.Tests.Controllers
{
    [TestClass]
    public  class ProjectUsersControllerTests : TestClass
    {
        private int projectId;
        static ProjectUsersController controller;
        private static UsersController userCtrl;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        { 
            var config = ConfigServiceFactory.ExistDb();
            var dbService = new DatabaseService(LogFactory.Create<DatabaseService>(), config, DatabasePortableContext.FromConfig(config));
            var sessionService = new AccessService(LogFactory.Create<AccessService>(), dbService);

            userCtrl = new UsersController(LogFactory.Create<UsersController>(), sessionService, dbService);
            var session = userCtrl.Login("admin", "admin").Result.Value;
            var controllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
            controllerContext.HttpContext.Request.Headers.Add("session", session); 

            controller = new ProjectUsersController(LogFactory.Create<ProjectUsersController>(), sessionService, dbService);
            controller.ControllerContext = controllerContext;
        }

        [TestInitialize]
        public void TestInit()
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

            controller = new ProjectUsersController(LogFactory.Create<ProjectUsersController>(), sessionService, dbService);
            controller.ControllerContext = controllerContext;
        }
          


        [TestMethod]
        public void AddUpdateDeleteVirtualUser()
        {
            var id = controller.AddVirtualUser(projectId, new AddOrUpdateVirtualUserRequest()
            {
                Login = "addUser",
                Mail = "addUser@mail.com",
                Name = "UserName",
                Pass = "pass",
                Phone = "+7",
                Role = UserRole.Guest
            }).Value;
            var user = controller.GetUsers(projectId).Value.Users.First(p => p.Id == id);

            Assert.AreEqual(id, user.Id);
            Assert.AreEqual("addUser", user.Login);
            Assert.AreEqual("addUser@mail.com", user.Mail);
            Assert.AreEqual("UserName", user.Name); 
            Assert.AreEqual(UserRole.Guest, user.Role);
            Assert.AreEqual(true, user.IsVirtual);

            controller.UpdateVirtualUser(projectId, id, new AddOrUpdateVirtualUserRequest()
            {
                Login = "addUser2",
            });
            user = controller.GetUsers(projectId).Value.Users.First(p => p.Id == id);
            Assert.AreEqual("addUser2", user.Login);

            controller.DeleteUser(projectId, id);
            user = controller.GetUsers(projectId).Value.Users.FirstOrDefault(p => p.Id == id);
            Assert.IsNull(user);
        }

        [TestMethod]
        public void AddUpdateDeleteRealUser()
        {
            var mail = "user@mail.ru";
            var existUser = userCtrl.RegisterUser(new RegisterOrUpdateUserRequest()
            {
                Mail = mail,
                Login = "l",
                Name = "n"
            }); 
            var id = controller.AddExistUser(projectId, new AddOrUpdateExistUserRequest()
            {
                Mail = mail,
                Role = UserRole.Owner
            }).Value;
            var user = controller.GetUsers(projectId).Value.Users.First(p => p.Id == id);

            Assert.AreEqual(id, user.Id);
            Assert.AreEqual(null, user.Login);
            Assert.AreEqual(mail, user.Mail);
            Assert.AreEqual(null, user.Name);
            Assert.AreEqual(UserRole.Owner, user.Role);
            Assert.AreEqual(false, user.IsVirtual);

            controller.UpdateExistUser(projectId, id.Value, new AddOrUpdateExistUserRequest()
            {
                Mail = "user@mail.ru",
                Role = UserRole.Guest
            });
            user = controller.GetUsers(projectId).Value.Users.First(p => p.Id == id);
            Assert.AreEqual(UserRole.Guest, user.Role);

            controller.DeleteUser(projectId, id.Value);
            user = controller.GetUsers(projectId).Value.Users.FirstOrDefault(p => p.Id == id);
            Assert.IsNull(user);
        }
    }
}
