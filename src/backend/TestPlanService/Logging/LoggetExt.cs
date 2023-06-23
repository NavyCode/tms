using Microsoft.Extensions.Logging;

namespace TestPlanService.Logging
{
    public static class LoggetExt
    {
        public static Navy.Core.Logger.ILogger CastToNavyLog(this ILogger l)
        {
            return new MicrosoftLogAdapter(l);
        }
    }
}
