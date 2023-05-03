using AutoMapper;
using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.catalog_service.Amenities;
using ftrip.io.catalog_service.PropertyTypes;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations.UseCases.CreateAccommodation
{
    public class CreateAccommodationRequestHandler : IRequestHandler<CreateAccommodationRequest, Accommodation>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IAmenityRepository _amenityRepository;
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly IMapper _mapper;
        private readonly IStringManager _stringManager;

        public CreateAccommodationRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IAmenityRepository amenityRepository,
            IPropertyTypeRepository propertyTypeRepository,
            IMapper mapper,
            IStringManager stringManager)
        {
            _unitOfWork = unitOfWork;
            _accommodationRepository = accommodationRepository;
            _amenityRepository = amenityRepository;
            _propertyTypeRepository = propertyTypeRepository;
            _mapper = mapper;
            _stringManager = stringManager;
        }

        public async Task<Accommodation> Handle(CreateAccommodationRequest request, CancellationToken cancellationToken)
        {
            if (await _propertyTypeRepository.Read(request.PropertyTypeId, cancellationToken) == null)
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", request.PropertyTypeId));

            var amenities = (await _amenityRepository.ReadByIds(request.Amenities.Select(a => a.AmenityId), cancellationToken)).ToList();
            if (amenities.Count < request.Amenities.Count)
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity",
                    string.Join(", ", request.Amenities.Where(aa => !amenities.Any(a => a.Id == aa.AmenityId)).Select(aa => aa.AmenityId)))
                );

            var accommodation = _mapper.Map<Accommodation>(request);

            await _unitOfWork.Begin(cancellationToken);

            var createdAccommodation = await CreateAccommodation(accommodation, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            return createdAccommodation;
        }

        private async Task<Accommodation> CreateAccommodation(Accommodation accommodation, CancellationToken ct)
        {
            return await _accommodationRepository.Create(accommodation, ct);
        }
    }
}
