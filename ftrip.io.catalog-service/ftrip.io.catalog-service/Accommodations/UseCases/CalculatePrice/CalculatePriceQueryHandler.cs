using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.catalog_service.contracts.Accommodations;
using ftrip.io.catalog_service.Utilities;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations.UseCases.CalculatePrice
{
    public class CalculatePriceQueryHandler : IRequestHandler<CalculatePriceQuery, PriceInfo>
    {
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IStringManager _stringManager;

        public CalculatePriceQueryHandler(IAccommodationRepository accommodationRepository, IStringManager stringManager)
        {
            _accommodationRepository = accommodationRepository;
            _stringManager = stringManager;
        }

        public async Task<PriceInfo> Handle(CalculatePriceQuery request, CancellationToken cancellationToken)
        {
            var accommodation = await _accommodationRepository.Read(request.AccommodationId);
            if (accommodation == null)
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", request.AccommodationId));

            var priceInfo = new PriceInfo
            {
                AccommodationId = accommodation.Id,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                Guests = request.Guests
            };

            var days = (request.CheckOutDate - request.CheckInDate).Days;
            if (request.CheckInDate < DateTime.Now.AddDays(accommodation.NoticePeriod))
                priceInfo.Problems.Add($"Too short notice period. The notice period is {accommodation.NoticePeriod} days");
            if (days < accommodation.MinNights)
                priceInfo.Problems.Add($"Too few nights. Min nights is {accommodation.MinNights}");
            if (days > accommodation.MaxNights)
                priceInfo.Problems.Add($"Too many nights. Max nights is {accommodation.MaxNights}");
            if (request.Guests < accommodation.MinGuests)
                priceInfo.Problems.Add($"Too few guests. Min guests is {accommodation.MinGuests}");
            if (request.Guests > accommodation.MaxGuests)
                priceInfo.Problems.Add($"Too many guests. Max guests is {accommodation.MaxGuests}");
            if (priceInfo.Problems.Any())
                return priceInfo;

            if (!CheckAvailability(request.CheckInDate, request.CheckOutDate, accommodation))
            {
                priceInfo.Problems.Add("This period is not available for booking");
                return priceInfo;
            }

            var priceDiffData = accommodation.PriceDiffs.Select(pd => new
            {
                numbers = CronUtils.ParseCronExpression(pd.When),
                percentage = pd.Percentage
            }).ToList();

            var date = request.CheckInDate;
            while (date < request.CheckOutDate)
            {
                var price = accommodation.IsPerGuest ? request.Guests * accommodation.Price : accommodation.Price;
                var priceDiffPercent = priceDiffData
                    .Where(d => CronUtils.Matches(date, d.numbers.monthDays, d.numbers.months, d.numbers.weekDays))
                    .Select(d => d.percentage).Sum();
                price += (decimal)priceDiffPercent / 100 * price;
                priceInfo.Items.Add(new Item { Date = date, Price = price, PriceDiffPercent = priceDiffPercent });
                priceInfo.Days++;
                priceInfo.TotalPrice += price;
                date = date.AddDays(1);
            }

            return priceInfo;
        }

        private bool CheckAvailability(DateTime checkInDate, DateTime checkOutDate, Accommodation accommodation)
        {
            var bookingEnd = DateTime.Now.AddMonths(accommodation.BookingAdvancePeriod);
            var date = checkInDate;
            bool available = true;

            while (date < checkOutDate && available)
            {
                if (accommodation.BookingAdvancePeriod >= 0 && (accommodation.BookingAdvancePeriod == 0 || date < bookingEnd))
                    available = !accommodation.Availabilities.Any((a) => !a.IsAvailable && a.FromDate <= date && a.ToDate >= date);
                else
                    available = accommodation.Availabilities.Any((a) => a.IsAvailable && a.FromDate <= date && a.ToDate >= date);
                date = date.AddDays(1);
            }

            return available;
        }
    }
}
