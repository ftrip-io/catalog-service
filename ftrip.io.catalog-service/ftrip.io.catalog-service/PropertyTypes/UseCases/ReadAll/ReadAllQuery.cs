using ftrip.io.catalog_service.PropertyTypes.Domain;
using MediatR;
using System.Collections.Generic;

namespace ftrip.io.catalog_service.PropertyTypes.UseCases.ReadAll
{
    public class ReadAllQuery : IRequest<IEnumerable<PropertyType>> { }
}
