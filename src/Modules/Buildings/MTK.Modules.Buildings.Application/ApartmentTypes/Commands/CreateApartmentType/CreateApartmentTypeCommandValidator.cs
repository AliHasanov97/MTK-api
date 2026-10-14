using FluentValidation;

namespace MTK.Modules.Buildings.Application.ApartmentTypes.Commands.CreateApartmentType;

internal sealed class CreateApartmentTypeCommandValidator
    : AbstractValidator<CreateApartmentTypeCommand>
{
    public CreateApartmentTypeCommandValidator()
    {
        RuleFor(x => x.RoomCount)
            .GreaterThan(0)
            .WithMessage("Otaq sayı müsbət olmalıdır");

        RuleFor(x => x.AreaSquareMeters)
            .GreaterThan(0)
            .WithMessage("Sahə müsbət olmalıdır");
    }
}
