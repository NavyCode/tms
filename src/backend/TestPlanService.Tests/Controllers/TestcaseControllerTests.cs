using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Navy.MsTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Controllers;
using TestPlanService.Models.TestCases;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;
using WildBerriesApi.Tests;

namespace TestPlanService.Tests.Controllers
{
    [TestClass]
    public  class TestcaseControllerTests : TestClass
    {
        static TestCasesController controller;
        static int projectId;
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
            projectId = projController.AddProject(new Models.Projects.AddOrUpdateProjectRequest() { }).Value;

            controller = new TestCasesController(LogFactory.Create<TestCasesController>(), sessionService, dbService);
            controller.ControllerContext = controllerContext;
        }

        [TestInitialize]
        public void TestInit()
        {
        }

        [TestMethod]
        public void AddUpdateDelete()
        {
            var query = new AddOrUpdateTestCaseRequest()
            {
                AssignedTo = userId,
                AutomationStatus = AutomationStatus.Automated,
                Description = "Descr",
                AutomationTestName = "Autotest",
                AutomationTestStorage = "dll",
                AutomationTestType = "UI",
                Postcondition = "Post",
                Precondition = "Pre",
                Priority = 1,
                State = WiState.Ready,
                Title = "Ti",
                Steps = new List<AddOrUpdateTestStep>()
                {
                    new AddOrUpdateTestStep()
                    {
                        Action = "Act",
                        Result = "Res",
                        Order = 1
                    },
                    new AddOrUpdateTestStep()
                    {
                        Action = "Act2",
                        Result = "Res2",
                        Order = 2
                    }
                }
            };

            var id = controller.AddTest(projectId, query).Value;

            var tc = controller.GetTestCase(projectId, id).Value;
            Assert.AreEqual(userId, tc.AssignedTo.Id);
            Assert.AreEqual(AutomationStatus.Automated, tc.AutomationStatus);
            Assert.AreEqual("Autotest", tc.AutomationTestName);
            Assert.AreEqual("dll", tc.AutomationTestStorage);
            Assert.AreEqual("UI", tc.AutomationTestType);
            Assert.AreEqual(userId, tc.ChangeBy.Id);
            Assert.AreEqual("Descr", tc.Description);
            Assert.AreEqual("Post", tc.Postcondition);
            Assert.AreEqual("Pre", tc.Precondition);
            Assert.AreEqual(1, tc.Priority);
            Assert.AreEqual(WiState.Ready, tc.State);
            Assert.AreEqual("Ti", tc.Title);

            Assert.AreEqual(2, tc.Steps.Count); 
            var s = tc.Steps.First(p => p.Order== 1);
            Assert.AreEqual("Act", s.Action);
            Assert.AreEqual("Res", s.Result);

            query.Steps.RemoveAll(p => p.Order == 1);
            query.Steps.Add(new AddOrUpdateTestStep()
            {
                Action = "Act3",
                Result = "Res3",
                Order = 1,
            });
            controller.UpdateTest(projectId, id, query);

            tc = controller.GetTestCase(projectId, id).Value;
            Assert.AreEqual(2, tc.Steps.Count);
            s = tc.Steps.First(p => p.Order == 1);
            Assert.AreEqual("Act3", s.Action);
            Assert.AreEqual("Res3", s.Result);

            controller.DeleteTest(projectId, id);
            tc = controller.GetTestCase(projectId, id).Value;
            Assert.IsNull(tc);
        }


        [TestMethod]
        public void Search()
        {
            var query1 = new AddOrUpdateTestCaseRequest()
            {
                AssignedTo = userId,
                Title = "UPPER NAME",
            }; 
            var id1 = controller.AddTest(projectId, query1).Value;


            var query2 = new AddOrUpdateTestCaseRequest()
            {
                AssignedTo = userId,
                Title = "Camel Name",
            };
            var id2 = controller.AddTest(projectId, query2).Value;

            var tc1 = controller.SearchTestCases(projectId, id1.ToString()).Value;
            Assert.AreEqual(1, tc1.Count);
            Assert.AreEqual(id1, tc1[0].Id);
            Assert.AreEqual(query1.Title, tc1[0].Name);


            var tc2 = controller.SearchTestCases(projectId, "name").Value;
            Assert.AreEqual(2, tc2.Count);
            Assert.AreEqual(id1, tc2[0].Id);
            Assert.AreEqual(id2, tc2[1].Id);
        }
    }
}
