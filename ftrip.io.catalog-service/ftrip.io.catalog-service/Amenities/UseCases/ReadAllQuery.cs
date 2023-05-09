using ftrip.io.catalog_service.Amenities.Domain;
using MediatR;
using System.Collections.Generic;

namespace ftrip.io.catalog_service.Amenities.UseCases
{
    public class ReadAllQuery : IRequest<IEnumerable<Amenity>> { }
}
