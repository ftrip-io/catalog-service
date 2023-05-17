using ftrip.io.catalog_service.Accommodations.Domain;
using MediatR;
using System;

namespace ftrip.io.catalog_service.Accommodations.UseCases.DeleteAccommodation
{
    public class DeleteAccommodationRequest : IRequest<Accommodation>
    {
        public Guid Id { get; set; }
    }
}
