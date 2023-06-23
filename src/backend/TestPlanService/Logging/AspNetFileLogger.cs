using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace TestPlanService.Logging
{
    public class AspNetFileLogger : ILogger
    {
        static private object _lock = new object();
        static Navy.Core.Logger.ILogger _fileLogger;
        private Navy.Core.Logger.PrefixLogger _prefixLogger;

        string _category;
        public AspNetFileLogger(string category)
        {
            _category = category;
            if (_fileLogger == null)
                lock (_lock)
                    if (_fileLogger == null)
                        _fileLogger = new Navy.Core.Logger.FileLogger();
            _prefixLogger = new Navy.Core.Logger.PrefixLogger(_fileLogger, $"[{_category}] ");
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }



        static Dictionary<LogLevel, Navy.Core.Logger.LogType> _map = new Dictionary<LogLevel, Navy.Core.Logger.LogType>
        {
            { LogLevel.Debug, Navy.Core.Logger.LogType.Debug },
            { LogLevel.Error , Navy.Core.Logger.LogType.Error },
            { LogLevel.Information, Navy.Core.Logger.LogType.Info  },
            { LogLevel.Trace, Navy.Core.Logger.LogType.Trace  },
            { LogLevel.Warning, Navy.Core.Logger.LogType.Warning  },
        };

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                var text = formatter(state, exception);
                var level = _map[logLevel];
                _prefixLogger.WriteLine(level, text);
                if (exception != null)
                    _prefixLogger.Error(exception);
            }
        }
    }
}