using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Projects;
using TestPlanService.Models.Suites;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;
using static System.Net.WebRequestMethods;

namespace TestSuiteService.Controllers
{
    [ApiController]
    [Route("projects/{projectId}")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        DatabaseService _db;
        AccessService _access;

        public FilesController(ILogger<FilesController> logger, AccessService session, DatabaseService db)
        { 
            _logger = logger;
            _db = db;
            _access = session;
        }

        [HttpPost("files")]
        public ActionResult<int> Upload(int projectId, IFormFile file, string tags)
        { 
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var project = _db.Projects.Find(projectId);
            using (var stream = new MemoryStream())
            {
                file.CopyToAsync(stream);
                var dbFile = _db.Files.Upload(project, file.FileName, stream.ToArray(), tags);
                return dbFile.Id;
            }
        }

        [HttpGet("files/{id}")]
        public IActionResult Download(int projectId, int id)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var file = _db.Files.Download(id, out var name);
            if(file == null)
                return NotFound();
             
            var content = new MemoryStream(file);
            var contentType = "APPLICATION/octet-stream"; 
            return File(content, contentType, name);
        }
    }
}
