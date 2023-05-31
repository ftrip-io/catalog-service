using ftrip.io.catalog_service.contracts.Accommodations;
using MediatR;
using System;

namespace ftrip.io.catalog_service.Accommodations.UseCases.CalculatePrice
{
    public class CalculatePriceQuery : IRequest<PriceInfo>
    {
        public Guid AccommodationId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Guests { get; set; }
    }
}
