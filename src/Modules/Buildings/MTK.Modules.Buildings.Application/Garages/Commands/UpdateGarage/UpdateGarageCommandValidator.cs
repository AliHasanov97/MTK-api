using FluentValidation;
using MTK.Modules.Buildings.Domain.Enums;

namespace MTK.Modules.Buildings.Application.Garages.Commands.UpdateGarage;

internal sealed class UpdateGarageCommandValidator : AbstractValidator<UpdateGarageCommand>
{
    public UpdateGarageCommandValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Düzgün qaraj növü seçilməlidir");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Təsvir maksimum 500 simvol ola bilər");
    }
}
