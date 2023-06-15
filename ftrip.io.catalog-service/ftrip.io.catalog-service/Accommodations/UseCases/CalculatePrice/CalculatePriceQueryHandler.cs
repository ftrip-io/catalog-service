using Amazon.Runtime.Internal.Util;
using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.catalog_service.contracts.Accommodations;
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
            var accommodation = request.Accommodation ?? await _accommodationRepository.Read(request.AccommodationId);
            if (accommodation == null)
            {
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", request.AccommodationId));
            }

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

            var priceDiffData = accommodation.PriceDiffs.Select(pd => new ParsedPriceDiff(pd.When, pd.Percentage)).ToList();

            for (var date = request.CheckInDate; date < request.CheckOutDate; date = date.AddDays(1))
            {
                (decimal price, float priceDiffPercent)
                    = date.CalculatePrice(accommodation.IsPerGuest, request.Guests, accommodation.Price, priceDiffData);
                priceInfo.Items.Add(new Item { Date = date, Price = price, PriceDiffPercent = priceDiffPercent });
                priceInfo.Days++;
                priceInfo.TotalPrice += price;
            }

            return priceInfo;
        }

        private bool CheckAvailability(DateTime checkInDate, DateTime checkOutDate, Accommodation accommodation)
        {
            var bookingEnd = DateTime.Now.AddMonths(accommodation.BookingAdvancePeriod);
            for (var date = checkInDate; date < checkOutDate; date = date.AddDays(1))
                if (!date.IsAvailable(accommodation.BookingAdvancePeriod, bookingEnd, accommodation.Availabilities))
                    return false;
            return true;
        }
    }
}
