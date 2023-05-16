using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations.UseCases.ReadById
{
    public class ReadByIdQueryHandler : IRequestHandler<ReadByIdQuery, Accommodation>
    {
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IStringManager _stringManager;

        public ReadByIdQueryHandler(
            IAccommodationRepository accommodationRepository,
            IStringManager stringManager)
        {
            _accommodationRepository = accommodationRepository;
            _stringManager = stringManager;
        }

        public async Task<Accommodation> Handle(ReadByIdQuery request, CancellationToken cancellationToken)
        {
            var accommodation = await (request.Simple
                ? _accommodationRepository.ReadSimple(request.Id, cancellationToken)
                : _accommodationRepository.Read(request.Id, cancellationToken)
            );

            if (accommodation == null)
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", request.Id));

            return accommodation;
        }
    }
}
