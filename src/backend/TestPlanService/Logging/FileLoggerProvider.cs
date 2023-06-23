using Microsoft.Extensions.Logging;

namespace TestPlanService.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        public FileLoggerProvider()
        {
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new AspNetFileLogger(categoryName);
        }

        public void Dispose()
        {
        }
    }
}
