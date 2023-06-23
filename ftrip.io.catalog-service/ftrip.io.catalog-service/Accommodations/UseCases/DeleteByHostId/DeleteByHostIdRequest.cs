using ftrip.io.catalog_service.Accommodations.Domain;
using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.catalog_service.Accommodations.UseCases.DeleteByHostId
{
    public class DeleteByHostIdRequest : IRequest<IEnumerable<Accommodation>>
    {
        public Guid HostId { get; set; }
    }
}
