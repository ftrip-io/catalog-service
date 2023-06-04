using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.framework.Contexts;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations.UseCases.DeleteAccommodation
{
    public class DeleteAccommodationRequestHandler : IRequestHandler<DeleteAccommodationRequest, Accommodation>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IStringManager _stringManager;
        private readonly CurrentUserContext _currentUserContext;
        private readonly ILogger _logger;

        public DeleteAccommodationRequestHandler(
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

        public async Task<Accommodation> Handle(DeleteAccommodationRequest request, CancellationToken cancellationToken)
        {
            await CheckIfCanModify(request.Id);
            await _unitOfWork.Begin(cancellationToken);
            var deletedAccommodation = await _accommodationRepository.Delete(request.Id);
            await _unitOfWork.Commit(cancellationToken);
            _logger.Information("Accomodation deleted - AccommodationId[{id}]", request.Id);
            return deletedAccommodation;
        }

        private async Task<bool> CheckIfCanModify(Guid id)
        {
            var accommodation = await _accommodationRepository.ReadSimple(id);
            if (accommodation == null)
            {
                _logger.Error("Cannot delete accommodation because it is not found - AccommodationId[{id}]", id);
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", id));
            }
            if (accommodation.HostId.ToString() != _currentUserContext.Id)
            {
                _logger.Error("Cannot delete accommodation because the user is not the host - UserId[{userId}], HostId[{hostId}]",
                    _currentUserContext.Id, accommodation.HostId);
                throw new ForbiddenException(_stringManager.GetString("Only_the_host"));
            }
            return true;
        }
    }
}
