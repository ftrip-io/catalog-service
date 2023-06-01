using System;
using System.Collections.Generic;

namespace ftrip.io.catalog_service.Utilities
{
    public static class CronUtils
    {
        public static int[] ToNumbers(string intervals)
        {
            if (intervals == "*") return new int[0];

            var result = new List<int>();
            var ranges = intervals.Split(',');

            foreach (var range in ranges)
            {
                var numbers = range.Split('-');
                int start = int.Parse(numbers[0]);
                int end = (numbers.Length < 2) ? start : int.Parse(numbers[1]);
                for (int i = start; i <= end; i++) result.Add(i);
            }
            return result.ToArray();
        }

        public static (int[] monthDays, int[] months, int[] weekDays) ParseCronExpression(string cron)
        {
            var parts = cron.Split(' ');
            return (ToNumbers(parts[2]), ToNumbers(parts[3]), ToNumbers(parts[4]));
        }

        public static bool Matches(DateTime date, int[] monthDays, int[] months, int[] weekDays)
        {
            int month = date.Month;
            int monthDay = date.Day;
            int weekDay = (date.DayOfWeek == DayOfWeek.Sunday) ? 7 : (int)date.DayOfWeek;

            return (!(monthDays.Length != 0 || weekDays.Length != 0) || Array.Exists(monthDays, d => d == monthDay) || Array.Exists(weekDays, d => d == weekDay))
                && (months.Length == 0 || Array.Exists(months, m => m == month));
        }
    }
}
