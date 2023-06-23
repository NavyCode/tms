using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.OperationNameGenerators;
using NSwag.CodeGeneration.TypeScript;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace TestPlanService.Tests
{
    [TestClass]
    public class SwaggerClientTests
    {  
        [TestMethod] 
        public void GenerateClientForAngular()
        {
            var apiUri =         @"..\..\..\..\TestPlanService\Api\swagger.json";
            var codeFile = @"..\..\..\..\..\frontend\src\app\api\reports\swagger.ts";
            var jsonText = File.ReadAllText(apiUri, Encoding.UTF8);
            var document = OpenApiDocument.FromJsonAsync(jsonText).Result;
            var settings = new TypeScriptClientGeneratorSettings
            {
                ClassName = $"ReportClient", 
                GenerateClientInterfaces = true,
                OperationNameGenerator = new SingleClientFromOperationIdOperationNameGenerator(), 
                // ClientBaseClass = "ReportClients.ReportClientBase", 
            };

            var generator = new TypeScriptClientGenerator(document, settings);
            var code = generator.GenerateFile();
            File.WriteAllText(codeFile, code, Encoding.UTF8);
        }

        [TestMethod]
        public void GenerateClientForDotnet()
        {
            var apiUri = @"..\..\..\..\TestPlanService\Api\swagger.json";
            var codeFile = @"..\..\..\..\TestPlanService\Api\swagger.cs"; 
            var jsonText = File.ReadAllText(apiUri, Encoding.UTF8);
            var document = OpenApiDocument.FromJsonAsync(jsonText).Result;
            var settings = new CSharpClientGeneratorSettings
            {
                ClassName = $"TestPlanClient",
                GenerateClientInterfaces = true,
                OperationNameGenerator = new SingleClientFromOperationIdOperationNameGenerator(),
            };

            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();
            File.WriteAllText(codeFile, code, Encoding.UTF8);
        }


         
    }
}