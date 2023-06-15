
using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.catalog_service.Accommodations.UseCases.SearchAccommodations
{
    public class SearchAccommodationQuery : IRequest<IEnumerable<AccommodationSearchInfo>>
    {
        public string Location { get; set; }
        public int? GuestNum { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
