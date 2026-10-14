using FluentValidation;

namespace MTK.Modules.Buildings.Application.Buildings.Commands.CreateBuilding;

internal sealed class CreateBuildingCommandValidator
    : AbstractValidator<CreateBuildingCommand>
{
    public CreateBuildingCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Bina adı boş ola bilməz")
            .MaximumLength(100)
            .WithMessage("Bina adı 100 simvoldan çox ola bilməz");

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Küçə adı boş ola bilməz");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("Şəhər adı boş ola bilməz");
    }
}
