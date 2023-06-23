using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace TestPlanService.Logging
{
    public class MicrosoftLogAdapter : Navy.Core.Logger.LoggerBase
    {
        ILogger _logger;

        public MicrosoftLogAdapter(ILogger logger)
        {
            _logger = logger;
        }

        Dictionary<Navy.Core.Logger.LogType, LogLevel> _map = new Dictionary<Navy.Core.Logger.LogType, LogLevel>()
        {
            { Navy.Core.Logger.LogType.Debug, LogLevel.Debug },
            { Navy.Core.Logger.LogType.Error, LogLevel.Error },
            { Navy.Core.Logger.LogType.Info, LogLevel.Information },
            { Navy.Core.Logger.LogType.Trace, LogLevel.Trace },
            { Navy.Core.Logger.LogType.Warning, LogLevel.Warning },
        };

        public override void WriteLine(Navy.Core.Logger.LogType level, string text, params object[] args)
        {
            _logger.Log(_map[level], text, args);
        }

        protected override void WriteText(string text)
        {

        }
    }
}