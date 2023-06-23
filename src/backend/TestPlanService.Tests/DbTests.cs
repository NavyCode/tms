using Microsoft.VisualStudio.TestTools.UnitTesting;
using Navy.MsTest;
using System.Linq;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Templates;
using WildBerriesApi.Tests;

namespace TestPlanService.Tests
{
    [TestClass]
    public class DbTests : TestClass
    {
        [TestMethod]
        public void CreateEmptyDatabase()
        {
            CreateEmptyDb();
        }


        [TestMethod]
        [Ignore]
        public void Script()
        {
            //var config = ConfigServiceFactory.ExistDb(RealSyncDataBase);
            //var dbService = new DatabaseService(LogFactory.Logger<DatabaseService>(), config, new DatabaseContext(config));
            //var user = dbService.Context.Users.First(p => p.Id == SimpleClientDbTemplate.UserId);

            //var from = new DateTime(2022, 02, 07).ToUniversalTime();
            //var to = new DateTime(2022, 02, 14).ToUniversalTime();

            //int i = 0;
            //foreach (var move in user.MoneyMovements
            //    .Where(p => p.WbV1ReportDetail?.Sale_dt >= from && p.WbV1ReportDetail?.Sale_dt < to
            //        && (p.Type.Id == EmptyDbTemplate.MoveWbCorrectSaleId || p.Type.Id == EmptyDbTemplate.MoveWbSaleId)
            //    ))
            //{
            //    if (i++ == 0)
            //        Trace.WriteLine(string.Join("\t", move.WbV1ReportDetail.GetType().GetProperties().Select(p => p.Name)));
            //    Trace.WriteLine(string.Join("\t", move.WbV1ReportDetail.GetType().GetProperties().Select(p => p.GetValue(move.WbV1ReportDetail))));
            //}


            // new EmptyDbTemplate(dbService.Context).Fill().Wait();

            //foreach (var r in dbService.Context.WbV1QueryReportDetails
            //    .GroupBy(p => p.Rrd_id)
            //    .Where(p => p.Count() > 1)
            //    .ToList())
            //{
            //    //dbService.Context.WbV1QueryReportDetails.Remove(r);
            //    //dbService.Context.WbV1QueryReportDetails.Remove(r);
            //}
        }
        private static DatabaseContext CreateEmptyDb()
        {
            var settings = ConfigServiceFactory.ClearDb();
            var db = DatabasePortableContext.FromConfig(settings);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            var dbService = new DatabaseService(LogFactory.Create<DatabaseService>(), settings, db);
            var emptyT = new EmptyDbTemplate(dbService);
            emptyT.Fill();
            return db;
        }
         


    }
}