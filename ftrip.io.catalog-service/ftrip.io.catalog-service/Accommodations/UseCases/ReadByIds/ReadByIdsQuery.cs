using ftrip.io.catalog_service.Accommodations.Domain;
using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.catalog_service.Accommodations.UseCases.ReadByIds
{
    public class ReadByIdsQuery : IRequest<IEnumerable<Accommodation>>
    {
        public Guid[] Ids { get; set; }
    }
}
