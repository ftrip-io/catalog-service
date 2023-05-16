using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.catalog_service.Amenities;
using ftrip.io.catalog_service.PropertyTypes;
using ftrip.io.framework.Contexts;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations.UseCases.UpdateAccommodation
{
    public class UpdateAccommodationRequestHandler : PartialAccommodationUpdateRequestHandler<UpdateAccommodationRequest>
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;

        public UpdateAccommodationRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IPropertyTypeRepository propertyTypeRepository,
            IStringManager stringManager,
            CurrentUserContext currentUserContext
        ) : base(unitOfWork, accommodationRepository, stringManager, currentUserContext)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        protected override async Task<bool> AdditionalValidations(UpdateAccommodationRequest request, CancellationToken ct)
        {
            if (await _propertyTypeRepository.Read(request.PropertyTypeId, ct) == null)
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", request.PropertyTypeId));
            return true;
        }

        protected override async Task<Accommodation> UpdateAccommodation(UpdateAccommodationRequest accommodationUpdate, CancellationToken ct)
            => await _accommodationRepository.Update(accommodationUpdate, ct);
    }


    public class UpdateAccommodationLocationRequestHandler : PartialAccommodationUpdateRequestHandler<UpdateAccommodationLocationRequest>
    {
        public UpdateAccommodationLocationRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IStringManager stringManager,
            CurrentUserContext currentUserContext
        ) : base(unitOfWork, accommodationRepository, stringManager, currentUserContext) { }

        protected override async Task<Accommodation> UpdateAccommodation(UpdateAccommodationLocationRequest accommodationUpdate, CancellationToken ct)
            => await _accommodationRepository.Update(accommodationUpdate, ct);
    }


    public class UpdateAccommodationAmenitiesRequestHandler : PartialAccommodationUpdateRequestHandler<UpdateAccommodationAmenitiesRequest>
    {
        private readonly IAmenityRepository _amenityRepository;

        public UpdateAccommodationAmenitiesRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IAmenityRepository amenityRepository,
            IStringManager stringManager,
            CurrentUserContext currentUserContext
        ) : base(unitOfWork, accommodationRepository, stringManager, currentUserContext)
        {
            _amenityRepository = amenityRepository;
        }

        protected override async Task<bool> AdditionalValidations(UpdateAccommodationAmenitiesRequest request, CancellationToken ct)
        {
            var amenities = (await _amenityRepository.ReadByIds(request.Amenities.Select(a => a.AmenityId), ct)).ToList();
            if (amenities.Count < request.Amenities.Count)
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity",
                    string.Join(", ", request.Amenities.Where(aa => !amenities.Any(a => a.Id == aa.AmenityId)).Select(aa => aa.AmenityId)))
                );
            return true;
        }

        protected override async Task<Accommodation> UpdateAccommodation(UpdateAccommodationAmenitiesRequest accommodationUpdate, CancellationToken ct)
            => await _accommodationRepository.Update(accommodationUpdate, ct);
    }


    public class UpdateAccommodationAvailabilitiesRequestHandler : PartialAccommodationUpdateRequestHandler<UpdateAccommodationAvailabilitiesRequest>
    {
        public UpdateAccommodationAvailabilitiesRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IStringManager stringManager,
            CurrentUserContext currentUserContext
        ) : base(unitOfWork, accommodationRepository, stringManager, currentUserContext) { }

        protected override Task<bool> AdditionalValidations(UpdateAccommodationAvailabilitiesRequest request, CancellationToken ct)
        {
            // TODO check existing reservations
            return base.AdditionalValidations(request, ct);
        }

        protected override async Task<Accommodation> UpdateAccommodation(UpdateAccommodationAvailabilitiesRequest accommodationUpdate, CancellationToken ct)
            => await _accommodationRepository.Update(accommodationUpdate, ct);
    }


    public class UpdateAccommodationPricingRequestHandler : PartialAccommodationUpdateRequestHandler<UpdateAccommodationPricingRequest>
    {
        public UpdateAccommodationPricingRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IStringManager stringManager,
            CurrentUserContext currentUserContext
        ) : base(unitOfWork, accommodationRepository, stringManager, currentUserContext) { }

        protected override Task<bool> AdditionalValidations(UpdateAccommodationPricingRequest request, CancellationToken ct)
        {
            // TODO check existing reservations
            return base.AdditionalValidations(request, ct);
        }

        protected override async Task<Accommodation> UpdateAccommodation(UpdateAccommodationPricingRequest accommodationUpdate, CancellationToken ct)
            => await _accommodationRepository.Update(accommodationUpdate, ct);
    }


    public abstract class PartialAccommodationUpdateRequestHandler<T> : IRequestHandler<T, Accommodation> where T : PartialAccommodationUpdateRequest
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IAccommodationRepository _accommodationRepository;
        protected readonly IStringManager _stringManager;
        protected readonly CurrentUserContext _currentUserContext;

        public PartialAccommodationUpdateRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IStringManager stringManager,
            CurrentUserContext currentUserContext)
        {
            _unitOfWork = unitOfWork;
            _accommodationRepository = accommodationRepository;
            _stringManager = stringManager;
            _currentUserContext = currentUserContext;
        }

        public async Task<Accommodation> Handle(T request, CancellationToken cancellationToken)
        {
            await CheckIfCanModify(request.Id);
            await AdditionalValidations(request, cancellationToken);
            await _unitOfWork.Begin(cancellationToken);
            var updatedAccommodation = await UpdateAccommodation(request, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return updatedAccommodation;
        }

        protected async Task<bool> CheckIfCanModify(Guid id)
        {
            var accommodation = await _accommodationRepository.ReadSimple(id);
            if (accommodation == null)
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", id));
            //if (accommodation.HostId.ToString() != _currentUserContext.Id)
            //    throw new ForbiddenException();
            return true;
        }

        protected virtual Task<bool> AdditionalValidations(T request, CancellationToken ct) => Task.FromResult(true);

        protected abstract Task<Accommodation> UpdateAccommodation(T accommodationUpdate, CancellationToken ct);
    }
}
