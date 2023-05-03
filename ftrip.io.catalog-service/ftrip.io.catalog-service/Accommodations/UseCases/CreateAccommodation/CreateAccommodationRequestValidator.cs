using FluentValidation;

namespace ftrip.io.catalog_service.Accommodations.UseCases.CreateAccommodation
{
    public class CreateAccommodationRequestValidator : AbstractValidator<CreateAccommodationRequest>
    {
        public CreateAccommodationRequestValidator()
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
                .GreaterThanOrEqualTo(2);

            RuleFor(request => request.BookBeforeTime)
                .InclusiveBetween(0, 24);

            RuleFor(request => request.MinNights)
                .GreaterThanOrEqualTo(1);

            RuleFor(request => request.MaxNights)
                .GreaterThanOrEqualTo(1);

            RuleFor(request => request.Price)
                .GreaterThan(0m);

            RuleFor(request => request.HostId)
                .NotEmpty();

            RuleFor(request => request.PropertyTypeId)
                .NotEmpty();
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

    public class CreateLocationRequestValidator: AbstractValidator<CreateLocationRequest>
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
}
