using System;

namespace TestPlanService.Services
{
    public sealed class TimeAdapter
    {
        int TimeZone;

        public TimeAdapter(int timeZone)
        {
            TimeZone = timeZone;
        }

        public DateTime ToLocalTime(DateTime date)
        {
            if (date.Kind == DateTimeKind.Local)
                return date;
            return DateTime.SpecifyKind(date, DateTimeKind.Local).AddHours(TimeZone);

        }

        public DateTime ToUniversalTime(DateTime date)
        {
            if (date.Kind == DateTimeKind.Utc)
                return date;
            return DateTime.SpecifyKind(date, DateTimeKind.Utc).AddHours(-TimeZone);
        }

        public DateTime UtcNow => DateTime.UtcNow;
    }
}
