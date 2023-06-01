using ftrip.io.catalog_service.PropertyTypes.Domain;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.PropertyTypes.UseCases.ReadAll
{
    public class ReadAllQueryHandler : IRequestHandler<ReadAllQuery, IEnumerable<PropertyType>>
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;

        public ReadAllQueryHandler(IPropertyTypeRepository propertyTypeRepository)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        public async Task<IEnumerable<PropertyType>> Handle(ReadAllQuery request, CancellationToken cancellationToken)
        {
            return await _propertyTypeRepository.Read(cancellationToken);
        }
    }
}
