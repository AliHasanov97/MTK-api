using FluentValidation;

namespace MTK.Modules.Buildings.Application.Apartments.Commands.CreateApartment;

internal sealed class CreateApartmentCommandValidator
    : AbstractValidator<CreateApartmentCommand>
{
    public CreateApartmentCommandValidator()
    {
        RuleFor(x => x.BuildingId)
            .NotEmpty()
            .WithMessage("Bina ID-si boş ola bilməz");

        RuleFor(x => x.ApartmentNumber)
            .NotEmpty()
            .WithMessage("Mənzil nömrəsi boş ola bilməz")
            .MaximumLength(20)
            .WithMessage("Mənzil nömrəsi 20 simvoldan çox ola bilməz");

        RuleFor(x => x.Floor)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Mərtəbə 0 və ya daha böyük olmalıdır");

        RuleFor(x => x.ApartmentTypeId)
            .NotEmpty()
            .WithMessage("Mənzil tipi seçilməlidir");
    }
}
