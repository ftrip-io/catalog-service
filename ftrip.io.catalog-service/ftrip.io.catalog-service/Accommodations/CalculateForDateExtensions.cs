using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.catalog_service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ftrip.io.catalog_service.Accommodations
{
    public static class CalculateForDateExtensions
    {
        public static (decimal price, float priceDiffPercent) CalculatePrice(this DateTime date, bool isPerGuest, int guests, decimal regularPrice, IEnumerable<ParsedPriceDiff> priceDiffData)
        {
            var price = isPerGuest ? guests * regularPrice : regularPrice;
            var priceDiffPercent = priceDiffData
                .Where(d => CronUtils.Matches(date, d.numbers.monthDays, d.numbers.months, d.numbers.weekDays))
                .Select(d => d.percentage).Sum();
            price += (decimal)priceDiffPercent / 100 * price;
            return (price, priceDiffPercent);
        }

        public static bool IsAvailable(this DateTime date, int bookingAdvancePeriod, DateTime bookingEnd, ICollection<Availability> availabilities)
        {
            if (bookingAdvancePeriod >= 0 && (bookingAdvancePeriod == 0 || date < bookingEnd))
                return !availabilities.Any((a) => !a.IsAvailable && a.FromDate <= date && a.ToDate >= date);
            return availabilities.Any((a) => a.IsAvailable && a.FromDate <= date && a.ToDate >= date);
        }
    }

    public class ParsedPriceDiff
    {
        public readonly (int[] monthDays, int[] months, int[] weekDays) numbers;
        public readonly float percentage;

        public ParsedPriceDiff(string when, float percentage)
        {
            numbers = CronUtils.ParseCronExpression(when);
            this.percentage = percentage;
        }
    }
}
