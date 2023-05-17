using FluentValidation;
using ftrip.io.framework.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace ftrip.io.catalog_service.Accommodations.UseCases.UpdateAccommodation
{
    public class UpdateAccommodationRequestValidator : AbstractValidator<UpdateAccommodationRequest>
    {
        public UpdateAccommodationRequestValidator(IStringManager stringManager)
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

            RuleFor(request => request.CheckInFrom)
                .InclusiveBetween(0, 24);

            RuleFor(request => request.CheckInTo)
                .InclusiveBetween(0, 24);

            RuleFor(request => request.CheckInTo - request.CheckInFrom)
                .GreaterThanOrEqualTo(2)
                .WithMessage(stringManager.Format("Validation_CheckIn_WindowLengthAtLeast", 2));

            RuleFor(request => request.BookBeforeTime)
                .InclusiveBetween(0, 24);

            RuleFor(request => request.MinNights)
                .GreaterThanOrEqualTo(1);

            RuleFor(request => request.MaxNights)
                .GreaterThanOrEqualTo(1);

            RuleFor(request => request.PropertyTypeId)
                .NotEmpty();
        }
    }

    public class LocationUpdateValidator : AbstractValidator<LocationUpdate>
    {
        public LocationUpdateValidator()
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

    public class AccommodationAmenityUpdateValidator : AbstractValidator<AccommodationAmenityUpdate>
    {
        public AccommodationAmenityUpdateValidator()
        {
            RuleFor(request => request.AmenityId)
                .NotEmpty();
        }
    }

    public class AvailabilityUpdateValidator : AbstractValidator<AvailabilityUpdate>
    {
        public AvailabilityUpdateValidator()
        {
            RuleFor(request => request.FromDate)
                .NotEmpty();

            RuleFor(request => request.ToDate)
                .NotEmpty();
        }
    }

    public class PriceDiffUpdateValidator : AbstractValidator<PriceDiffUpdate>
    {
        public PriceDiffUpdateValidator()
        {
            RuleFor(request => request.Percentage)
                .NotEmpty();

            RuleFor(request => request.When)
                .Matches(@"0 0( (\*|(\d+(-\d+)?)(,(\d+(-\d+)?))*)){3}");
        }
    }

    public class UpdateAccommodationLocationRequestValidator : AbstractValidator<UpdateAccommodationLocationRequest>
    {
        public UpdateAccommodationLocationRequestValidator(LocationUpdateValidator locationUpdateValidator)
        {
            RuleFor(request => request.Location)
                .SetValidator(locationUpdateValidator);
        }
    }

    public class UpdateAccommodationAmenitiesRequestValidator : AbstractValidator<UpdateAccommodationAmenitiesRequest>
    {
        public UpdateAccommodationAmenitiesRequestValidator(AccommodationAmenityUpdateValidator accommodationAmenityUpdateValidator)
        {
            RuleForEach(request => request.Amenities)
                .SetValidator(accommodationAmenityUpdateValidator);
        }
    }

    public class UpdateAccommodationAvailabilitiesRequestValidator : AbstractValidator<UpdateAccommodationAvailabilitiesRequest>
    {
        public UpdateAccommodationAvailabilitiesRequestValidator(AvailabilityUpdateValidator availabilityUpdateValidator, IStringManager stringManager)
        {
            RuleFor(request => request.BookingAdvancePeriod)
                .InclusiveBetween(-1, 12);

            RuleForEach(request => request.Availabilities)
                .SetValidator(availabilityUpdateValidator);

            RuleFor(request => request.Availabilities)
                .Must(BeNonOverlapping)
                .WithMessage(stringManager.GetString("Availability_Intervals_No_Overlap"));
        }

        private bool BeNonOverlapping(IEnumerable<AvailabilityUpdate> availabilities)
        {
            var ordered = availabilities.OrderBy(a => a.FromDate);
            return ordered.Zip(ordered.Skip(1), (a1, a2) => a1.ToDate < a2.FromDate).All(v => v);
        }
    }

    public class UpdateAccommodationPricingRequestValidator : AbstractValidator<UpdateAccommodationPricingRequest>
    {
        public UpdateAccommodationPricingRequestValidator(PriceDiffUpdateValidator priceDiffUpdateValidator)
        {
            RuleFor(request => request.Price)
                .GreaterThan(0m);

            RuleForEach(request => request.PriceDiffs)
                .SetValidator(priceDiffUpdateValidator);
        }
    }
}
