using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestPlanService.Exceptions
{
    public class ErrorHandlerMiddleware : IMiddleware
    {
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            WriteIndented = true
        };

        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(IWebHostEnvironment env, ILogger<ErrorHandlerMiddleware> logger)
        {
            _env = env;
            _logger = logger;
        }

        const string RequestIdKey = "RequestId";

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                context.Request.Headers[RequestIdKey] = Guid.NewGuid().ToString();
                await next(context);
            }
            catch (Exception exception) when (context.RequestAborted.IsCancellationRequested)
            {
                const string message = "Request was cancelled";
                _logger.LogInformation(message);
                _logger.LogDebug(exception, message);
                context.Response.Clear();
                context.Response.StatusCode = 499; //Client Closed Request
            }
            catch (Exception exception)
            {
                const string message = "An unhandled exception has occurred while executing the request.";
                _logger.LogError(exception, exception is ApiException ? exception.Message : message);
                const string contentType = "application/json";
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = contentType;
                var json = ToJson(exception, context);
                await context.Response.WriteAsync(json);
            }
        }

        private string ToJson(in Exception exception, HttpContext context)
        {
            var message = exception.Message;
            var code = "";
            if (exception is ApiException api)
                code = api.HResult.ToString();
            else if (context.Request.Headers.TryGetValue(RequestIdKey, out var rid))
                code = rid.ToString();
            bool isDevelopmentOrQA = false;
#if DEBUG
            isDevelopmentOrQA = true;
#endif
            if (!isDevelopmentOrQA)
            {
                return JsonSerializer.Serialize(new { message, code }, SerializerOptions);
            }
            try
            {
                var info = exception.ToString();
                var data = exception.Data;
                var error = new { message, code, info, data };
                return JsonSerializer.Serialize(error, SerializerOptions);
            }
            catch (Exception ex)
            {
                const string mes = "An exception has occurred while serializing error to JSON";
                _logger.LogError(ex, mes);
            }

            return string.Empty;
        }
    }
}
