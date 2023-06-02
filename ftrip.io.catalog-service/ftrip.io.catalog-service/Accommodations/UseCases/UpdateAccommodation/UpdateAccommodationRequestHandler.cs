using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.catalog_service.Amenities;
using ftrip.io.catalog_service.PropertyTypes;
using ftrip.io.framework.Contexts;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
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
            CurrentUserContext currentUserContext,
            ILogger logger
        ) : base(unitOfWork, accommodationRepository, stringManager, currentUserContext, logger)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        protected override async Task<bool> AdditionalValidations(UpdateAccommodationRequest request, CancellationToken ct)
        {
            if (await _propertyTypeRepository.Read(request.PropertyTypeId, ct) == null)
            {
                _logger.Error("Cannot update accommodation because property type is not found - PropertyId[{id}]", request.PropertyTypeId);
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", request.PropertyTypeId));
            }
            return true;
        }

        protected override async Task<Accommodation> UpdateAccommodation(UpdateAccommodationRequest accommodationUpdate, CancellationToken ct)
        {
            var updated = await _accommodationRepository.Update(accommodationUpdate, ct);
            _logger.Information("Accommodation updated - AccommodationId[{id}]", accommodationUpdate.Id);
            return updated;
        }
    }


    public class UpdateAccommodationLocationRequestHandler : PartialAccommodationUpdateRequestHandler<UpdateAccommodationLocationRequest>
    {
        public UpdateAccommodationLocationRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IStringManager stringManager,
            CurrentUserContext currentUserContext,
            ILogger logger
        ) : base(unitOfWork, accommodationRepository, stringManager, currentUserContext, logger) { }

        protected override async Task<Accommodation> UpdateAccommodation(UpdateAccommodationLocationRequest accommodationUpdate, CancellationToken ct)
        {
            var updated = await _accommodationRepository.Update(accommodationUpdate, ct);
            _logger.Information("Accommodation location updated - AccommodationId[{id}]", accommodationUpdate.Id);
            return updated;
        }
    }


    public class UpdateAccommodationAmenitiesRequestHandler : PartialAccommodationUpdateRequestHandler<UpdateAccommodationAmenitiesRequest>
    {
        private readonly IAmenityRepository _amenityRepository;

        public UpdateAccommodationAmenitiesRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IAmenityRepository amenityRepository,
            IStringManager stringManager,
            CurrentUserContext currentUserContext,
            ILogger logger
        ) : base(unitOfWork, accommodationRepository, stringManager, currentUserContext, logger)
        {
            _amenityRepository = amenityRepository;
        }

        protected override async Task<bool> AdditionalValidations(UpdateAccommodationAmenitiesRequest request, CancellationToken ct)
        {
            var amenities = (await _amenityRepository.ReadByIds(request.Amenities.Select(a => a.AmenityId), ct)).ToList();
            if (amenities.Count < request.Amenities.Count)
            {
                var notFoundIds = string.Join(", ", request.Amenities.Where(aa => !amenities.Any(a => a.Id == aa.AmenityId)).Select(aa => aa.AmenityId));
                _logger.Error("Cannot update accommodation because some amenities are not found - AmenityIds[[{ids}]]", notFoundIds);
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", notFoundIds));
            }
            return true;
        }

        protected override async Task<Accommodation> UpdateAccommodation(UpdateAccommodationAmenitiesRequest accommodationUpdate, CancellationToken ct)
        {
            var updated = await _accommodationRepository.Update(accommodationUpdate, ct);
            _logger.Information("Accommodation amenities updated - AccommodationId[{id}]", accommodationUpdate.Id);
            return updated;
        }
    }


    public class UpdateAccommodationAvailabilitiesRequestHandler : PartialAccommodationUpdateRequestHandler<UpdateAccommodationAvailabilitiesRequest>
    {
        protected readonly IBookingServiceClient _bookingServiceClient;

        public UpdateAccommodationAvailabilitiesRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IStringManager stringManager,
            CurrentUserContext currentUserContext,
            IBookingServiceClient bookingServiceClient,
            ILogger logger
        ) : base(unitOfWork, accommodationRepository, stringManager, currentUserContext, logger)
        {
            _bookingServiceClient = bookingServiceClient;
        }

        protected override async Task<bool> AdditionalValidations(UpdateAccommodationAvailabilitiesRequest request, CancellationToken ct)
        {
            var reservations = await _bookingServiceClient.GetOccupancies(request.Id, DateTime.Now, DateTime.MaxValue);
            var bookingEnd = DateTime.Now.AddMonths(request.BookingAdvancePeriod);
            var availabilities = request.Availabilities
                .Select(a => new Availability { IsAvailable = a.IsAvailable, FromDate = a.FromDate, ToDate = a.ToDate }).ToList();

            foreach (var reservation in reservations)
                for (var date = reservation.DatePeriod.DateFrom; date < reservation.DatePeriod.DateTo; date = date.AddDays(1))
                    if (!date.IsAvailable(request.BookingAdvancePeriod, bookingEnd, availabilities))
                    {
                        _logger.Error("Cannot update accommodation availabilities because of some existing reservations - AccommodationId[{id}]", request.Id);
                        throw new BadLogicException(_stringManager.GetString("Existing_reservation_availability"));
                    }
            return true;
        }

        protected override async Task<Accommodation> UpdateAccommodation(UpdateAccommodationAvailabilitiesRequest accommodationUpdate, CancellationToken ct)
        {
            var updated = await _accommodationRepository.Update(accommodationUpdate, ct);
            _logger.Information("Accommodation availabilities updated - AccommodationId[{id}]", accommodationUpdate.Id);
            return updated;
        }
    }


    public class UpdateAccommodationPricingRequestHandler : PartialAccommodationUpdateRequestHandler<UpdateAccommodationPricingRequest>
    {
        protected readonly IBookingServiceClient _bookingServiceClient;

        public UpdateAccommodationPricingRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IStringManager stringManager,
            CurrentUserContext currentUserContext,
            IBookingServiceClient bookingServiceClient,
            ILogger logger
        ) : base(unitOfWork, accommodationRepository, stringManager, currentUserContext, logger)
        {
            _bookingServiceClient = bookingServiceClient;
        }

        protected override async Task<bool> AdditionalValidations(UpdateAccommodationPricingRequest request, CancellationToken ct)
        {
            var reservations = await _bookingServiceClient.GetOccupancies(request.Id, DateTime.Now, DateTime.MaxValue);
            var priceDiffData = request.PriceDiffs.Select(pd => new ParsedPriceDiff(pd.When, pd.Percentage)).ToList();

            foreach (var reservation in reservations)
            {
                var totalPrice = 0m;
                for (var date = reservation.DatePeriod.DateFrom; date < reservation.DatePeriod.DateTo; date = date.AddDays(1))
                    totalPrice += date.CalculatePrice(request.IsPerGuest, reservation.Guests, request.Price, priceDiffData).price;

                if (totalPrice != reservation.Price)
                {
                    _logger.Error("Cannot update accommodation pricing because it would change the price of some existing reservations - AccommodationId[{id}]", request.Id);
                    throw new BadLogicException(_stringManager.GetString("Existing_reservation_price"));
                }
            }
            return true;
        }

        protected override async Task<Accommodation> UpdateAccommodation(UpdateAccommodationPricingRequest accommodationUpdate, CancellationToken ct)
        {
            var updated = await _accommodationRepository.Update(accommodationUpdate, ct);
            _logger.Information("Accommodation pricing updated - AccommodationId[{id}]", accommodationUpdate.Id);
            return updated;
        }
    }


    public abstract class PartialAccommodationUpdateRequestHandler<T> : IRequestHandler<T, Accommodation> where T : PartialAccommodationUpdateRequest
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IAccommodationRepository _accommodationRepository;
        protected readonly IStringManager _stringManager;
        protected readonly CurrentUserContext _currentUserContext;
        protected readonly ILogger _logger;

        protected PartialAccommodationUpdateRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IStringManager stringManager,
            CurrentUserContext currentUserContext,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _accommodationRepository = accommodationRepository;
            _stringManager = stringManager;
            _currentUserContext = currentUserContext;
            _logger = logger;
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
            {
                _logger.Error("Cannot update accommodation because it is not found - AccommodationId[{id}]", id);
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", id));
            }
            if (accommodation.HostId.ToString() != _currentUserContext.Id)
            {
                _logger.Error("Cannot update accommodation because the user is not the host - UserId[{userId}], HostId[{hostId}]",
                    _currentUserContext.Id, accommodation.HostId);
                throw new ForbiddenException(_stringManager.GetString("Only_the_host"));
            }
            return true;
        }

        protected virtual Task<bool> AdditionalValidations(T request, CancellationToken ct) => Task.FromResult(true);

        protected abstract Task<Accommodation> UpdateAccommodation(T accommodationUpdate, CancellationToken ct);
    }
}
