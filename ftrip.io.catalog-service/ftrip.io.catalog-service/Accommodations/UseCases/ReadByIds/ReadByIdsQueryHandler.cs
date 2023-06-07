using ftrip.io.catalog_service.Accommodations.Domain;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations.UseCases.ReadByIds
{
    public class ReadByIdsQueryHandler : IRequestHandler<ReadByIdsQuery, IEnumerable<Accommodation>>
    {
        private readonly IAccommodationRepository _accommodationRepository;

        public ReadByIdsQueryHandler(IAccommodationRepository accommodationRepository)
        {
            _accommodationRepository = accommodationRepository;
        }

        public async Task<IEnumerable<Accommodation>> Handle(ReadByIdsQuery request, CancellationToken cancellationToken)
        {
            return await _accommodationRepository.ReadByIds(request.Ids, cancellationToken);
        }
    }
}
