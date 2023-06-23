//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using System.Threading.Tasks;
//using TestPlanService.Services.Cleaning;
//using TestPlanService.Services.Sessions;

//namespace TestPlanService.Controllers
//{
//    [ApiController]
//    public class AdminController : ControllerBase
//    {
//        private readonly ILogger<AdminController> _logger;
//        AccessService _session;
//        CleaningService _cleaning; 

//        public AdminController(ILogger<AdminController> logger, AccessService session, CleaningService cleaning)
//        {
//            _logger = logger;
//            _session = session;
//            _cleaning = cleaning;
//        } 

//        //[HttpPost("admin/clear/db")]
//        //public async Task ClearDb()
//        //{
//        //    var user = _session.GetRootId(Request);
//        //    await _cleaning.Start();
//        //}

//    }
//}
