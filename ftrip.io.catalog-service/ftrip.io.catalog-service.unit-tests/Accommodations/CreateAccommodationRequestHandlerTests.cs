using AutoMapper;
using FluentAssertions;
using ftrip.io.catalog_service.Accommodations;
using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.catalog_service.Accommodations.UseCases.CreateAccommodation;
using ftrip.io.catalog_service.Amenities;
using ftrip.io.catalog_service.Amenities.Domain;
using ftrip.io.catalog_service.PropertyTypes;
using ftrip.io.catalog_service.PropertyTypes.Domain;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.catalog_service.unit_tests.Accommodations
{
    public class CreateAccommodationRequestHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IAccommodationRepository> _accommodationRepositoryMock = new Mock<IAccommodationRepository>();
        private readonly Mock<IAmenityRepository> _amenityRepositoryMock = new Mock<IAmenityRepository>();
        private readonly Mock<IPropertyTypeRepository> _propertyTypeRepositoryMock = new Mock<IPropertyTypeRepository>();
        private readonly Mock<IStringManager> _stringManagerMock = new Mock<IStringManager>();

        private readonly CreateAccommodationRequestHandler _handler;

        public CreateAccommodationRequestHandlerTests()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateAccommodationRequest, Accommodation>();
                cfg.CreateMap<CreateAccommodationAmenityRequest, AccommodationAmenity>();
                cfg.CreateMap<CreateLocationRequest, Location>();
            }).CreateMapper();

            _handler = new CreateAccommodationRequestHandler(
                _unitOfWorkMock.Object,
                _accommodationRepositoryMock.Object,
                _amenityRepositoryMock.Object,
                _propertyTypeRepositoryMock.Object,
                mapper,
                _stringManagerMock.Object
            );
        }

        [Fact]
        public async Task Handle_Successful_Returns_Accommodation()
        {
            var request = GetCreateAccommodationRequest();

            _accommodationRepositoryMock
                .Setup(r => r.Create(It.IsAny<Accommodation>(), It.IsAny<CancellationToken>()))
                .Returns((Accommodation a, CancellationToken _) =>
                {
                    a.Id = Guid.NewGuid();
                    return Task.FromResult(a);
                });
            _propertyTypeRepositoryMock
                .Setup(r => r.Read(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(new PropertyType { Id = id }));
            _amenityRepositoryMock
                .Setup(r => r.ReadByIds(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<Guid> ids, CancellationToken _) => Task.FromResult(ids.Select(id => new Amenity { Id = id })));

            var createdAccommodation = await _handler.Handle(request, CancellationToken.None);

            createdAccommodation.Should().NotBeNull();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_PropertyType_NotFound_ThrowsException()
        {
            var request = GetCreateAccommodationRequest();

            _propertyTypeRepositoryMock
                .Setup(r => r.Read(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult<PropertyType>(null));

            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            await handleAction.Should().ThrowExactlyAsync<MissingEntityException>();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Amenity_NotFound_ThrowsException()
        {
            var request = GetCreateAccommodationRequest();

            _amenityRepositoryMock
                .Setup(r => r.ReadByIds(It.IsAny<ICollection<Guid>>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<Guid> ids, CancellationToken _) => Task.FromResult(Enumerable.Empty<Amenity>()));

            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            await handleAction.Should().ThrowExactlyAsync<MissingEntityException>();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        private CreateAccommodationRequest GetCreateAccommodationRequest()
            => new CreateAccommodationRequest
            {
                Title = "Title",
                Description = "Description",
                PropertyTypeId = new Guid(),
                PlaceType = PlaceType.ENTIRE_PLACE,
                Amenities = new CreateAccommodationAmenityRequest[]
                {
                    new CreateAccommodationAmenityRequest
                    {
                        AmenityId = new Guid(),
                        IsPresent = true
                    }
                },
                MaxGuests = 1,
                MinGuests = 10,
                MaxNights = 1,
                MinNights = 10,
                BedroomCount = 2,
                BedCount = 3,
                BathroomCount = 2,
                BookBeforeTime = 12,
                BookingAdvancePeriod = 3,
                CheckInFrom = 10,
                CheckInTo = 14,
                HostId = new Guid(),
                IsDecicatedForGuests = true,
                Price = 100,
                IsPerGuest = true,
                HouseRules = "",
                NoticePeriod = 10,
                Location = new CreateLocationRequest
                {
                    Country = "",
                    Region = "",
                    City = "",
                    PostalCode = "",
                    Address = "",
                    Apt = "",
                    Latitude = 45,
                    Longitude = 20
                }
            };
    }
}
