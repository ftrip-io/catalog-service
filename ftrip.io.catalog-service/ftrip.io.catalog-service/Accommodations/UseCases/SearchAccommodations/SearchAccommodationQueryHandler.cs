using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.catalog_service.Accommodations.UseCases.CalculatePrice;
using ftrip.io.catalog_service.contracts.Accommodations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations.UseCases.SearchAccommodations
{
    public class SearchAccommodationQueryHandler : IRequestHandler<SearchAccommodationQuery, IEnumerable<AccommodationSearchInfo>>
    {
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IBookingServiceClient _bookingServiceClient;
        private readonly IMediator _mediator;

        public SearchAccommodationQueryHandler(
            IAccommodationRepository accommodationRepository, 
            IBookingServiceClient bookingServiceClient,
            IMediator mediator)
        {
            _accommodationRepository = accommodationRepository;
            _bookingServiceClient = bookingServiceClient;
            _mediator = mediator;
        }

        public async Task<IEnumerable<AccommodationSearchInfo>> Handle(SearchAccommodationQuery request, CancellationToken cancellationToken)
        {
            var matchedAccommodations = await _accommodationRepository.ReadByQuery(request, cancellationToken);
            if (!matchedAccommodations.Any())
            {
                return Enumerable.Empty<AccommodationSearchInfo>();
            }

            var areDatesProvided = request.FromDate.HasValue && request.ToDate.HasValue;
            var availableAccommodations = areDatesProvided
                ? await AvailableAccommodations(matchedAccommodations, request, cancellationToken)
                : matchedAccommodations;
            var prices = await Task.WhenAll(areDatesProvided
                ? availableAccommodations.Select(a => GetPriceInfo(matchedAccommodations.First(ma => ma.Id == a.Id), request.FromDate, request.ToDate, request.GuestNum.Value, cancellationToken))
                : Enumerable.Empty<Task<PriceInfo>>());

            return availableAccommodations
                .Where(a => !prices.FirstOrDefault(p => p.AccommodationId == a.Id)?.Problems.Any() ?? true)
                .Select(a => new AccommodationSearchInfo
                {
                    AccommodationId = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    PlaceType = a.PlaceType,
                    BathroomCount = a.BathroomCount,
                    BedCount = a.BedCount,
                    BedroomCount = a.BedroomCount,
                    Location = a.Location,
                    Price = a.Price,
                    IsPerGuest = a.IsPerGuest,
                    TotalPrice = prices.FirstOrDefault(p => p.AccommodationId == a.Id)?.TotalPrice ?? null
                });
        }

        private async Task<IEnumerable<Accommodation>> AvailableAccommodations(IEnumerable<Accommodation> matchedAccommodations, SearchAccommodationQuery request, CancellationToken cancellationToken) 
        {
            var availableAccommodationIds = await _bookingServiceClient.CheckAvailability(matchedAccommodations.Select(a => a.Id).ToList(), request.FromDate.Value, request.ToDate.Value, cancellationToken);

            return matchedAccommodations.Where(a => availableAccommodationIds.Contains(a.Id));
        }

        private async Task<PriceInfo> GetPriceInfo(Accommodation accommodation, DateTime? checkInDate, DateTime? checkOutDate, int guestNumber, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new CalculatePriceQuery()
            {
                AccommodationId = accommodation.Id,
                Accommodation = accommodation,
                CheckInDate = checkInDate.Value,
                CheckOutDate = checkOutDate.Value,
                Guests = guestNumber
            }, cancellationToken);
        }
    }
}
