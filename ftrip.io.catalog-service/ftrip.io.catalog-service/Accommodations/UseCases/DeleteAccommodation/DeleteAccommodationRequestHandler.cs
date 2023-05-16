using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.framework.Contexts;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations.UseCases.DeleteAccommodation
{
    public class DeleteAccommodationRequestHandler : IRequestHandler<DeleteAccommodationRequest, Accommodation>
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IAccommodationRepository _accommodationRepository;
        protected readonly IStringManager _stringManager;
        protected readonly CurrentUserContext _currentUserContext;

        public DeleteAccommodationRequestHandler(
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

        public async Task<Accommodation> Handle(DeleteAccommodationRequest request, CancellationToken cancellationToken)
        {
            await CheckIfCanModify(request.Id);
            await _unitOfWork.Begin(cancellationToken);
            var deletedAccommodation = await _accommodationRepository.Delete(request.Id);
            await _unitOfWork.Commit(cancellationToken);
            return deletedAccommodation;
        }

        private async Task<bool> CheckIfCanModify(Guid id)
        {
            var accommodation = await _accommodationRepository.ReadSimple(id);
            if (accommodation == null)
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", id));
            //if (accommodation.HostId.ToString() != _currentUserContext.Id)
            //    throw new ForbiddenException();
            return true;
        }
    }
}
