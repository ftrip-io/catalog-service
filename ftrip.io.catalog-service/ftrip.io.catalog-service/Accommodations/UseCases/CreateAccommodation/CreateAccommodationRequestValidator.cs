using FluentValidation;
using ftrip.io.framework.Globalization;

namespace ftrip.io.catalog_service.Accommodations.UseCases.CreateAccommodation
{
    public class CreateAccommodationRequestValidator : AbstractValidator<CreateAccommodationRequest>
    {
        public CreateAccommodationRequestValidator(IStringManager stringManager)
        {
            RuleFor(request => request.Title)
                .NotEmpty()
                .WithMessage(stringManager.Format("Common_Validation_FieldIsRequired", "Title"));
        }
    }
}
