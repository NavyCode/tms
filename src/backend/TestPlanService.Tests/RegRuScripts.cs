using Microsoft.VisualStudio.TestTools.UnitTesting;
using Navy.MsTest;
using TestPlanService.Services.Db;

namespace WildBerriesApi.Tests
{
    [TestClass]
    public class RegRuScripts : TestClass
    {
        [TestMethod]
        [Ignore]
        public void Script()
        {
            //var settings = ConfigServiceFactory.RegRu();
            //var db = new DatabaseContext(settings);

            //var clean = new CleaningService(LogFactory.Logger<CleaningService>(), settings);
            //clean.Start().Wait();

            //var dbService = new DatabaseService(LogFactory.Logger<DatabaseService>(), settings, new DatabaseContext(settings));
            //var reportService = new ReportService(LogFactory.Logger<ReportService>(), dbService);
            //var report = reportService.DetailedMoving(Guid.Parse("B249D21B-8E8D-4B60-E778-08DA3F021285"), new DateTime(2022, 02, 01).ToUniversalTime(), DateTime.UtcNow);

            //foreach (var g in report.Items.GroupBy(p => p.MovingTypeId))
            //{
            //    Logger.WriteLine($"{report.Types.First(p => p.Id == g.Key).Name}: {g.Sum(p => p.Value)}");
            //}

            //var enums = db.Enumerations.Count();
            //Assert.AreNotEqual(0, enums);
        } 

    }
}