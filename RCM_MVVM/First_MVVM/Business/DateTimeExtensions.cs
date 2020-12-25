using System;

namespace First_MVVM.Business
{
    public class DateTimeExtensions
    {
        public DateTime FirstDayOfWeek(DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;

            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-diff).Date;
        }

        public DateTime GetDateOfLastDays(DateTime dt,int LastDays)
        {
            var diff = dt.AddDays(LastDays);

            return diff;
        }
    }
}
