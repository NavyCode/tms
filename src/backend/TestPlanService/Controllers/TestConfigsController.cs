using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Suites;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;
using YamlDotNet.Core.Tokens;

namespace TestSuiteService.Controllers
{
    [ApiController]
    [Route("projects/{projectId}")]
    public class TestConfigsController : ControllerBase
    {
        private readonly ILogger<TestConfigsController> _logger; 
        DatabaseService _db;
        AccessService _access;

        public TestConfigsController(ILogger<TestConfigsController> logger, AccessService session, DatabaseService db)
        {
            _logger = logger; 
            _db = db;
            _access = session;
        }  

        [HttpGet("test/configs")]
        public ActionResult<GetConfigsResponse> GetConfigs(int projectId)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var result = new GetConfigsResponse(); 
            foreach(var c in _db.Context.TestConfigs.Where(p => p.Project.Id == projectId && !p.IsDeleted))
            {
                var config = new ConfigItem() { Id = c.Id, Name = c.Name, Comment = c.Description, IsDefault = c.IsDefault };
                foreach(var v in c.Params)
                {
                    config.Variables.Add(new ConfigVariableItem()
                    {
                        Id = v.Id,
                        VariableId = v.Variable.Id,
                        ValueId = v.VariableParam.Id,
                    });
                }
                result.Configs.Add(config);
            }
            return result;
        }

        [HttpGet("test/configvars")]
        public ActionResult<GetConfigsVarsResponse> GetConfigsVars(int projectId)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var result = new GetConfigsVarsResponse();
            foreach (var c in _db.Context.TestConfigVariables.Where(p => p.Project.Id == projectId).ToList())
            {
                var configVar = new ConfigVarItem()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Comment = c.Description
                };
                foreach(var v in c.Params)
                {
                    configVar.Values.Add(new ConfigVarValueItem() { Id = v.Id, Value = v.Value });
                }
                result.Vars.Add(configVar);
            }
            return result;
        }

        [HttpPost("test/configs")]
        public ActionResult<int> AddConfig(int projectId, AddOrUpdateConfigsRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var project = _db.Projects.Find(projectId); 
            var config = _db.TestConfigs.AddConfig(project, request);
            return config.Id;
        } 


        [HttpPut("test/configs/{configId}")]
        public ActionResult UpdateConfig(int projectId, int configId, [FromBody] AddOrUpdateConfigsRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var config = _db.Context.TestConfigs.FirstOrDefault(p => p.Id == configId);
            if (config == null || config.Project.Id != projectId)
                return NotFound();
            _db.TestConfigs.UpdateConfig(config, request);
            return Ok();
        }

        [HttpPost("test/configvars")]
        public ActionResult<int> AddConfigVariable(int projectId, AddOrUpdateConfigVarRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var project = _db.Projects.Find(projectId); 
            var configVar = _db.TestConfigs.AddConfigVariable(project, request);
            return configVar.Id;  
        }

        [HttpPut("test/configvars/{id}")]
        public ActionResult UpdateConfigVariable(int projectId, int id, AddOrUpdateConfigVarRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var configVar = _db.Context.TestConfigVariables.FirstOrDefault(p => p.Id == id);
            if (configVar == null || configVar.Project.Id != projectId)
                return NotFound();
            _db.TestConfigs.UpdateConfigVariable(configVar, request);
            return Ok();
        }

        [HttpDelete("test/configvars/{id}")]
        public ActionResult DeleteConfigVariable(int projectId, int id)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var configVar = _db.Context.TestConfigVariables.FirstOrDefault(p => p.Id == id);
            if (configVar == null || configVar.Project.Id != projectId)
                    return NotFound();

            _db.Context.TestConfigVariableValues.RemoveRange(configVar.Params);
            _db.Context.TestConfigVariables.Remove(configVar);
            _db.Context.SaveChanges();
            return Ok();
        }

        [HttpDelete("test/configs/{configId}")]
        public ActionResult DeleteConfig(int projectId, int configId)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var config = _db.Context.TestConfigs.FirstOrDefault(p => p.Id == configId);
            if (config == null || config.Project.Id != projectId)
                return NotFound();

            config?.Deactivate();
            _db.Context.SaveChanges();
            return Ok();
        } 
    }
}
