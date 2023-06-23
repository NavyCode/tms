using System;
using System.Linq;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Projects;
using TestPlanService.Models.Suites;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Services.Db.SubSystems
{
    public class TestConfigsSubsystem
    {
        DatabaseService _db;

        public TestConfigsSubsystem(DatabaseService context)
        {
            _db = context;
        }

        public TestConfig AddConfig(Project project, AddOrUpdateConfigsRequest request, bool isSave = false)
        {
            var config = new TestConfig()
            {
                Project = project,
            };
            _db.Context.TestConfigs.Add(config);
            if(isSave)
                _db.Context.SaveChanges();
            UpdateConfig(config, request);
            return config;
        }

         
        public void UpdateConfig(TestConfig config, AddOrUpdateConfigsRequest request, bool isSave = false)
        {
            config.IsDefault = request.IsDefault;
            config.Description = request.Comment;
            config.Name = request.Name;
            var varIds = request.Variables.Select(p => p.Id).ToHashSet();
            _db.Context.TestConfigParams.RemoveRange(config.Params.Where(p => !varIds.Contains(p.Id)));
            foreach (var v in request.Variables)
            {
                var cv = _db.Context.TestConfigParams.FirstOrDefault(p => p.Id == v.Id);
                if (cv == null)
                {
                    cv = new TestConfigParam();
                    _db.Context.TestConfigParams.Add(cv);
                }
                cv.Config = config;
                cv.VariableParam = _db.Context.TestConfigVariableValues.FirstOrDefault(p => p.Id == v.ValueId);
                cv.Variable = _db.Context.TestConfigVariables.FirstOrDefault(p => p.Id == v.VariableId);
            }
            if (isSave)
                _db.Context.SaveChanges();
        }

        public TestConfigVariable AddConfigVariable(Project project, AddOrUpdateConfigVarRequest request)
        {
            var configVar = new TestConfigVariable()
            {
                Project = project,
            };
            _db.Context.TestConfigVariables.Add(configVar);
            _db.Context.SaveChanges(); 
            UpdateConfigVariable(configVar, request);
            return configVar;
        }

        public void UpdateConfigVariable(TestConfigVariable configVar, AddOrUpdateConfigVarRequest request)
        { 
            configVar.Description = request.Comment;
            configVar.Name = request.Name;
            var valueIds = request.Values.Select(p => p.Id).ToHashSet();
            _db.Context.TestConfigVariableValues.RemoveRange(configVar.Params.Where(p => !valueIds.Contains(p.Id)));
            foreach (var v in request.Values)
            {
                var cv = _db.Context.TestConfigVariableValues.FirstOrDefault(p => p.Id == v.Id);
                if (cv == null)
                {
                    cv = new TestConfigVariableParam();
                    _db.Context.TestConfigVariableValues.Add(cv);
                }
                cv.ConfigVar = configVar;
                cv.Value = v.Value;
            }
            _db.Context.SaveChanges();
        }
    }
}
