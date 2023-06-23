using Microsoft.Extensions.Logging;

namespace WildBerriesApi.Tests
{
    internal class LogFactory
    {
        internal static ILogger<T> Create<T>()
        {
            return GetFactory().CreateLogger<T>();
        }

        internal static ILogger Logger()
        {
            return GetFactory().CreateLogger("Test");
        }

        private static ILoggerFactory GetFactory()
        {
            return LoggerFactory.Create(builder =>
            {
                builder 
                    .AddConsole()
                    .AddTraceSource("Test")
                    .AddDebug();
            });
        }
    }
}
