using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations.UseCases.DeleteByHostId
{
    public class DeleteByHostIdRequestHandler : IRequestHandler<DeleteByHostIdRequest, IEnumerable<Accommodation>>
    {
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteByHostIdRequestHandler(IAccommodationRepository accommodationRepository, IUnitOfWork unitOfWork)
        {
            _accommodationRepository = accommodationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Accommodation>> Handle(DeleteByHostIdRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);
            var deleted = await _accommodationRepository.DeleteByHostId(request.HostId, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return deleted;
        }
    }
}
