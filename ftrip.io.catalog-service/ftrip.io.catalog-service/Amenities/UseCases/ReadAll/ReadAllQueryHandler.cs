using ftrip.io.catalog_service.Amenities.Domain;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Amenities.UseCases.ReadAll
{
    public class ReadAllQueryHandler : IRequestHandler<ReadAllQuery, IEnumerable<Amenity>>
    {
        private readonly IAmenityRepository _amenityRepository;

        public ReadAllQueryHandler(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        public async Task<IEnumerable<Amenity>> Handle(ReadAllQuery request, CancellationToken cancellationToken)
        {
            return await _amenityRepository.Read(cancellationToken);
        }
    }
}
