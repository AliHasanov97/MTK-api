using FluentValidation;

namespace MTK.Modules.Buildings.Application.Garages.Commands.CreateGarage;

internal sealed class CreateGarageCommandValidator : AbstractValidator<CreateGarageCommand>
{
    public CreateGarageCommandValidator()
    {
        RuleFor(x => x.GarageNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.GarageType)
            .NotEmpty()
            .Must(type => type is "OpenParking" or "CoveredGarage" or "Storage");
    }
}
