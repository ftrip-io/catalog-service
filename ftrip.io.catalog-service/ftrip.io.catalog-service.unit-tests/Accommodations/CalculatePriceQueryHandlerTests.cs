using FluentAssertions;
using ftrip.io.catalog_service.Accommodations;
using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.catalog_service.Accommodations.UseCases.CalculatePrice;
using ftrip.io.framework.Globalization;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.catalog_service.unit_tests.Accommodations
{
    public class CalculatePriceQueryHandlerTests
    {
        private readonly Mock<IAccommodationRepository> _accommodationRepositoryMock = new Mock<IAccommodationRepository>();
        private readonly Mock<IStringManager> _stringManagerMock = new Mock<IStringManager>();

        private readonly CalculatePriceQueryHandler _handler;

        public CalculatePriceQueryHandlerTests()
        {
            _handler = new CalculatePriceQueryHandler(_accommodationRepositoryMock.Object, _stringManagerMock.Object);
        }

        [Theory]
        [InlineData(4, 8, 0, "few", "guests")]
        [InlineData(4, 8, 9, "many", "guests")]
        [InlineData(4, 5, 4, "few", "nights")]
        [InlineData(4, 14, 4, "many", "nights")]
        [InlineData(2, 8, 4, "short", "notice")]
        public async Task Handle_ConstraintViolation_Returns_PriceInfo_Problems(int checkInInDays, int checkOutInDays, int guests, params string[] problem)
        {
            var request = new CalculatePriceQuery
            {
                AccommodationId = Guid.NewGuid(),
                CheckInDate = DateTime.Now.AddDays(checkInInDays),
                CheckOutDate = DateTime.Now.AddDays(checkOutInDays),
                Guests = guests
            };

            _accommodationRepositoryMock
                .Setup(r => r.Read(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(new Accommodation
                {
                    Id = id,
                    MinGuests = 1,
                    MaxGuests = 6,
                    MinNights = 2,
                    MaxNights = 8,
                    NoticePeriod = 3,
                }));

            var priceInfo = await _handler.Handle(request, CancellationToken.None);

            priceInfo.Problems.Should().ContainSingle().Which.Should().ContainAll(problem);
            priceInfo.TotalPrice.Should().Be(0m);
        }

        [Fact]
        public async Task Handle_NotAvailableInPeriod_Returns_PriceInfo_Problems()
        {
            var request = new CalculatePriceQuery
            {
                AccommodationId = Guid.NewGuid(),
                CheckInDate = DateTime.Now.AddDays(4),
                CheckOutDate = DateTime.Now.AddDays(8),
                Guests = 4
            };

            _accommodationRepositoryMock
                .Setup(r => r.Read(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(new Accommodation
                {
                    Id = id,
                    MinGuests = 1,
                    MaxGuests = 6,
                    MinNights = 2,
                    MaxNights = 8,
                    NoticePeriod = 3,
                    Availabilities = new List<Availability>
                    {
                        new Availability
                        {
                            IsAvailable = false,
                            FromDate = DateTime.Now.AddDays(6),
                            ToDate = DateTime.Now.AddDays(10)
                        }
                    },
                    BookingAdvancePeriod = 0,
                }));

            var priceInfo = await _handler.Handle(request, CancellationToken.None);

            priceInfo.Problems.Should().ContainSingle().Which.Should().ContainAll("not", "available");
            priceInfo.TotalPrice.Should().Be(0m);
        }

        [Fact]
        public async Task Handle_NotAvailableTooFarInAdvance_Returns_PriceInfo_Problems()
        {
            var request = new CalculatePriceQuery
            {
                AccommodationId = Guid.NewGuid(),
                CheckInDate = DateTime.Now.AddDays(34),
                CheckOutDate = DateTime.Now.AddDays(38),
                Guests = 4
            };

            _accommodationRepositoryMock
                .Setup(r => r.Read(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(new Accommodation
                {
                    Id = id,
                    MinGuests = 1,
                    MaxGuests = 6,
                    MinNights = 2,
                    MaxNights = 8,
                    NoticePeriod = 3,
                    Availabilities = new List<Availability>(),
                    BookingAdvancePeriod = 1
                }));

            var priceInfo = await _handler.Handle(request, CancellationToken.None);

            priceInfo.Problems.Should().ContainSingle().Which.Should().ContainAll("not", "available");
            priceInfo.TotalPrice.Should().Be(0m);
        }

        [Theory]
        [InlineData(28, false, 530)]
        [InlineData(28, true, 2120)]
        [InlineData(20, false, 550)]
        [InlineData(40, false, 500)]
        [InlineData(1, false, 551)]
        public async Task Handle_Successful_Returns_PriceInfo_NoProblems(int checkInAddDays, bool isPerGuest, decimal expectedPrice)
        {
            var nextJune = new DateTime(DateTime.Now.Year + 1, 5, 31);
            int nights = 5;
            decimal price = 100m;
            var priceDiffInJune = new PriceDiff { When = "* * * 6 *", Percentage = +10 };
            var priceDiffOnEvery4th = new PriceDiff { When = "* * 4 * *", Percentage = +1 };

            var request = new CalculatePriceQuery
            {
                AccommodationId = Guid.NewGuid(),
                CheckInDate = nextJune.AddDays(checkInAddDays),
                CheckOutDate = nextJune.AddDays(checkInAddDays + nights),
                Guests = 4
            };

            _accommodationRepositoryMock
                .Setup(r => r.Read(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(new Accommodation
                {
                    Id = id,
                    MinGuests = 1,
                    MaxGuests = 6,
                    MinNights = 2,
                    MaxNights = 8,
                    NoticePeriod = 3,
                    PriceDiffs = new List<PriceDiff> { priceDiffInJune, priceDiffOnEvery4th },
                    Availabilities = new List<Availability>
                    {
                        new Availability
                        {
                            IsAvailable = false,
                            FromDate = nextJune.AddDays(10),
                            ToDate = nextJune.AddDays(14)
                        }
                    },
                    BookingAdvancePeriod = 0,
                    IsPerGuest = isPerGuest,
                    Price = price
                }));

            var priceInfo = await _handler.Handle(request, CancellationToken.None);

            priceInfo.Problems.Should().BeEmpty();
            priceInfo.Days.Should().Be(nights);
            priceInfo.TotalPrice.Should().Be(expectedPrice);
        }
    }
}
