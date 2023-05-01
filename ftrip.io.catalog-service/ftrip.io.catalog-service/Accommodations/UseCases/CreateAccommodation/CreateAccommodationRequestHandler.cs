using AutoMapper;
using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations.UseCases.CreateAccommodation
{
    public class CreateAccommodationRequestHandler : IRequestHandler<CreateAccommodationRequest, Accommodation>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IMapper _mapper;

        public CreateAccommodationRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _accommodationRepository = accommodationRepository;
            _mapper = mapper;
        }

        public async Task<Accommodation> Handle(CreateAccommodationRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);
            var accommodation = _mapper.Map<Accommodation>(request);
            var createdAccommodation = await CreateAccommodation(accommodation, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return createdAccommodation;
        }

        private async Task<Accommodation> CreateAccommodation(Accommodation accommodation, CancellationToken cancellationToken)
        {
            return await _accommodationRepository.Create(accommodation, cancellationToken);
        }
    }
}
