using ftrip.io.catalog_service.Accommodations.UseCases.SearchAccommodations;
using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.catalog_service.Accommodations.UseCases.ReadByHostId
{
    public class ReadByHostIdQuery : IRequest<IEnumerable<AccommodationSearchInfo>>
    {
        public Guid HostId { get; set; }
    }
}
