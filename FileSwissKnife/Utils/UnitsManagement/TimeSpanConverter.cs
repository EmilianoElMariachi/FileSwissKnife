using System;

namespace FileSwissKnife.Utils.UnitsManagement
{
    public static class TimeSpanConverter
    {
        public static string ToElapsedTimeString(this TimeSpan timeSpan)
        {
            //TODO: à finir
            if (timeSpan.TotalMilliseconds < 1000)
                return $"{timeSpan.TotalMilliseconds:#}ms";
            if (timeSpan.TotalSeconds < 60)
                return $"{timeSpan.TotalSeconds:F2}s";
            if (timeSpan.TotalMinutes < 60)
                return $"{timeSpan.TotalMinutes}m{timeSpan.Seconds}s";
            return $"{timeSpan.TotalHours}h{timeSpan.Minutes}m{timeSpan.Seconds}s";

        }
    }
}
