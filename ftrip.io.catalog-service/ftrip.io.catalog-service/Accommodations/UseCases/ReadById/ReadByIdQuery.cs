using ftrip.io.catalog_service.Accommodations.Domain;
using MediatR;
using System;

namespace ftrip.io.catalog_service.Accommodations.UseCases.ReadById
{
    public class ReadByIdQuery : IRequest<Accommodation>
    {
        public Guid Id { get; set; }
        public bool Simple { get; set; } = false;
    }
}
