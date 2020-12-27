using System;

namespace FileSwissKnife.Utils.UnitsManagement
{
    public static class TimeSpanExtension
    {
        public static string ToElapsedTime(this TimeSpan ts)
        {

            // More than one hour => second rounding
            if (ts.Days > 0 || ts.Hours > 0)
            {
                var tsSecRounded = TsSecRounded(ts);
                return ToHourMinSec(tsSecRounded.Days * 24 + tsSecRounded.Hours, tsSecRounded.Minutes, tsSecRounded.Seconds);
            }

            // More than one min => second rounding
            if (ts.Minutes > 0)
            {
                var tsSecRounded = TsSecRounded(ts);
                return tsSecRounded.Hours == 1 ? ToHourMinSec(1, 0, 0) : ToMinSec(tsSecRounded.Minutes, tsSecRounded.Seconds);
            }

            if (ts.Seconds <= 0)
                return $"{ts.Milliseconds:#}ms";

            var seconds = ts.Seconds;
            RoundToHundredthSec(ts.Milliseconds, ref seconds, out var hundredthSecRounded);

            return seconds == 60 ? ToMinSec(1, 0) : $"{seconds}.{hundredthSecRounded:00}s";
        }

        private static string ToHourMinSec(int hours, int minutes, int seconds)
        {
            return $"{hours}h{minutes:00}m{seconds:00}s";
        }

        private static string ToMinSec(int minutes, int seconds)
        {
            return $"{minutes}m{seconds:00}s";
        }

        private static TimeSpan TsSecRounded(TimeSpan ts)
        {
            var tsSecRounded = ts.Milliseconds <= 500
                ? new TimeSpan(ts.Days, ts.Hours, ts.Minutes, ts.Seconds + 0, 0)
                : new TimeSpan(ts.Days, ts.Hours, ts.Minutes, ts.Seconds + 1, 0);
            return tsSecRounded;
        }

        private static void RoundToHundredthSec(in int milliseconds, ref int sec, out int hundredthSecRounded)
        {
            var hundredthFloor = (milliseconds / 10);
            var nbUnits = milliseconds - hundredthFloor * 10;
            if (nbUnits <= 5)
            {
                hundredthSecRounded = hundredthFloor;
                return;
            }

            var hundredthTmp = hundredthFloor + 1;
            if (hundredthTmp > 99)
            {
                sec++;
                hundredthSecRounded = 0;
                return;
            }

            hundredthSecRounded = hundredthTmp;
        }

        //private static bool IsMoreThanASec(in TimeSpan ts, out int sec, out int cs)
        //{
        //    if (ts.)

        //        throw new NotImplementedException();
        //}

        //private static bool IsMoreThanAMin(this TimeSpan ts, out int min, out int sec)
        //{
        //    if (ts.Milliseconds > 500)
        //        ts = new TimeSpan(ts.Days, ts.Hours, ts.Minutes, ts.Seconds + 1, 0);

        //    if (ts.Minutes > 0)
        //    {
        //        min = ts.Minutes;
        //        sec = ts.Seconds;
        //        return true;
        //    }

        //    min = 0;
        //    sec = 0;
        //    return false;

        //}
    }
}
