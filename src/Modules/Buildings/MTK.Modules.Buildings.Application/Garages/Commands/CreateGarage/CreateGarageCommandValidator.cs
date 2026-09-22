using FluentValidation;

namespace MTK.Modules.Buildings.Application.Garages.Commands.CreateGarage;

internal sealed class CreateGarageCommandValidator
    : AbstractValidator<CreateGarageCommand>
{
    public CreateGarageCommandValidator()
    {
        RuleFor(x => x.ApartmentId)
            .NotEmpty()
            .WithMessage("Mənzil ID-si boş ola bilməz");

        RuleFor(x => x.GarageNumber)
            .NotEmpty()
            .WithMessage("Qaraj nömrəsi boş ola bilməz")
            .MaximumLength(20)
            .WithMessage("Qaraj nömrəsi 20 simvoldan çox ola bilməz");

        RuleFor(x => x.GarageType)
            .NotEmpty()
            .WithMessage("Qaraj növü boş ola bilməz")
            .Must(type => type is "OpenParking" or "CoveredGarage" or "Storage")
            .WithMessage("Qaraj növü yalnız OpenParking, CoveredGarage və ya Storage ola bilər");
    }
}
