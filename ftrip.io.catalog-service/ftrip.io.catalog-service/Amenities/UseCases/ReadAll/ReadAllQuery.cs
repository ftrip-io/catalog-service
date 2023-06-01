using ftrip.io.catalog_service.Amenities.Domain;
using MediatR;
using System.Collections.Generic;

namespace ftrip.io.catalog_service.Amenities.UseCases.ReadAll
{
    public class ReadAllQuery : IRequest<IEnumerable<Amenity>> { }
}
