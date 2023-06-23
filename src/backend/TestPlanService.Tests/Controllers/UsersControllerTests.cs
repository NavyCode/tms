using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Navy.MsTest;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Controllers;
using TestPlanService.Models.Authorization;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;
using WildBerriesApi.Tests;

namespace TestPlanService.Tests.Controllers
{
    [TestClass]
    public  class UsersControllerTests : TestClass
    {
        static UsersController controller;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var config = ConfigServiceFactory.ExistDb();
            var dbService = new DatabaseService(LogFactory.Create<DatabaseService>(), config, DatabasePortableContext.FromConfig(config));
            var sessionService = new AccessService(LogFactory.Create<AccessService>(), dbService);
            controller = new UsersController(LogFactory.Create<UsersController>(), sessionService, dbService);
           
        }

        [TestInitialize]
        public void TestInit()
        {
            var session = controller.Login("admin", "admin").Result.Value;
            var controllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
            controllerContext.HttpContext.Request.Headers.Add("session", session);
            controller.ControllerContext = controllerContext;
        }

        [TestMethod]
        public void Login()
        {
            var response =  controller.Login("admin", "admin").Result.Value;
            Assert.IsTrue(Guid.TryParse(response, out _));
        }

        
        [TestMethod]
        public void Info()
        { 
            var info = controller.UserInfo().Value;
            Assert.AreEqual("Admin", info.Name);
            Assert.IsTrue(info.IsRoot);
        }


        [TestMethod]
        public void RegisterUser()
        {
            // todo
            //var id = controller.RegisterUser(new RegisterUserRequest()
            //{
            //    Login = "addUser",
            //    Mail = "addUser@mail.com",
            //    Name = "UserName",
            //    Pass = "pass", 
            //});
            //var user = controller.GetUsers().Users.First(p => p.Id == id);

            //Assert.AreEqual(id, user.Id);
            //Assert.AreEqual("addUser", user.Login);
            //Assert.AreEqual("addUser@mail.com", user.Mail);
            //Assert.AreEqual("UserName", user.Name);
            //Assert.AreEqual("+7", user.Phone);
            //Assert.AreEqual(UserRole.Guest, user.Role);

            //controller.UpdateUser(id, new Models.Authorization.AddOrUpdateVirtualUserRequest()
            //{
            //    Login = "addUser2",
            //});
            //user = controller.GetUsers().Users.First(p => p.Id == id);
            //Assert.AreEqual("addUser2", user.Login);

            //controller.DeleteUser(id);
            //user = controller.GetUsers().Users.FirstOrDefault(p => p.Id == id);
            //Assert.IsNull(user);
        }
    }
}
