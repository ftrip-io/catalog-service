using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace ftrip.io.catalog_service.Accommodations.UseCases.CreateAccommodation
{
    public class CreateAccommodationRequestValidator : AbstractValidator<CreateAccommodationRequest>
    {
        public CreateAccommodationRequestValidator(
            CreateAccommodationAmenityRequestValidator createAccommodationAmenityRequestValidator,
            CreateLocationRequestValidator createLocationRequestValidator,
            CreateAvailabilityRequestValidator createAvailabilityRequestValidator,
            CreatePriceDiffRequestValidator createPriceDiffRequestValidator
        )
        {
            RuleFor(request => request.Title)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(request => request.Description)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(request => request.PlaceType)
                .IsInEnum();

            RuleFor(request => request.MinGuests)
                .GreaterThanOrEqualTo(1);

            RuleFor(request => request.MaxGuests)
                .GreaterThanOrEqualTo(1);

            RuleFor(request => request.BedroomCount)
                .GreaterThanOrEqualTo(0);

            RuleFor(request => request.BedCount)
                .GreaterThanOrEqualTo(0);

            RuleFor(request => request.BathroomCount)
                .GreaterThanOrEqualTo(0);

            RuleFor(request => request.NoticePeriod)
                .InclusiveBetween(0, 7);

            RuleFor(request => request.BookingAdvancePeriod)
                .InclusiveBetween(-1, 12);

            RuleFor(request => request.CheckInFrom)
                .InclusiveBetween(0, 24);

            RuleFor(request => request.CheckInTo)
                .InclusiveBetween(0, 24);

            RuleFor(request => request.CheckInTo - request.CheckInFrom)
                .GreaterThanOrEqualTo(2)
                .WithMessage("The check-in window should be at least 2 hours");

            RuleFor(request => request.BookBeforeTime)
                .InclusiveBetween(0, 24);

            RuleFor(request => request.MinNights)
                .GreaterThanOrEqualTo(1);

            RuleFor(request => request.MaxNights)
                .GreaterThanOrEqualTo(1);

            RuleFor(request => request.Price)
                .GreaterThan(0m);

            RuleFor(request => request.PropertyTypeId)
                .NotEmpty();

            RuleFor(request => request.Location)
                .SetValidator(createLocationRequestValidator);

            RuleForEach(request => request.Amenities)
                .SetValidator(createAccommodationAmenityRequestValidator);

            RuleForEach(request => request.Availabilities)
                .SetValidator(createAvailabilityRequestValidator);

            RuleFor(request => request.Availabilities)
                .Must(BeNonOverlapping)
                .WithMessage("The availability intervals should not overlap");

            RuleForEach(request => request.PriceDiffs)
                .SetValidator(createPriceDiffRequestValidator);
        }

        private bool BeNonOverlapping(IEnumerable<CreateAvailabilityRequest> availabilities)
        {
            var ordered = availabilities.OrderBy(a => a.FromDate);
            return ordered.Zip(ordered.Skip(1), (a1, a2) => a1.ToDate < a2.FromDate).All(v => v);
        }
    }

    public class CreateAccommodationAmenityRequestValidator : AbstractValidator<CreateAccommodationAmenityRequest>
    {
        public CreateAccommodationAmenityRequestValidator()
        {
            RuleFor(request => request.AmenityId)
                .NotEmpty();
        }
    }

    public class CreateLocationRequestValidator : AbstractValidator<CreateLocationRequest>
    {
        public CreateLocationRequestValidator()
        {
            RuleFor(request => request.Country)
                .NotEmpty();

            RuleFor(request => request.City)
                .NotEmpty();

            RuleFor(request => request.Address)
                .NotEmpty();

            RuleFor(request => request.Longitude)
                .InclusiveBetween(-180, 180);

            RuleFor(request => request.Latitude)
                .InclusiveBetween(-90, 90);
        }
    }

    public class CreateAvailabilityRequestValidator : AbstractValidator<CreateAvailabilityRequest>
    {
        public CreateAvailabilityRequestValidator()
        {
            RuleFor(request => request.FromDate)
                .NotEmpty();

            RuleFor(request => request.ToDate)
                .NotEmpty();
        }
    }

    public class CreatePriceDiffRequestValidator : AbstractValidator<CreatePriceDiffRequest>
    {
        public CreatePriceDiffRequestValidator()
        {
            RuleFor(request => request.Percentage)
                .NotEmpty();

            RuleFor(request => request.When)
                .Matches(@"0 0( (\*|(\d+(-\d+)?)(,(\d+(-\d+)?))*)){3}");
        }
    }
}
