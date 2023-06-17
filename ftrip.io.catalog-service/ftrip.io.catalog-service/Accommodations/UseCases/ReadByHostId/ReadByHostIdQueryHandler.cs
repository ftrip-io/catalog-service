using ftrip.io.catalog_service.Accommodations.UseCases.SearchAccommodations;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations.UseCases.ReadByHostId
{
    public class ReadByHostIdQueryHandler : IRequestHandler<ReadByHostIdQuery, IEnumerable<AccommodationSearchInfo>>
    {
        private readonly IAccommodationRepository _accommodationRepository;

        public ReadByHostIdQueryHandler(IAccommodationRepository accommodationRepository)
        {
            _accommodationRepository = accommodationRepository;
        }

        public async Task<IEnumerable<AccommodationSearchInfo>> Handle(ReadByHostIdQuery request, CancellationToken cancellationToken)
        {
            var accommodations = await _accommodationRepository.ReadByHostId(request.HostId, cancellationToken);
            return accommodations.Select(a => new AccommodationSearchInfo
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
                IsPerGuest = a.IsPerGuest
            });
        }
    }
}
