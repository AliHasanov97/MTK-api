using FluentValidation;
using MTK.Modules.Payments.Domain.Rates;

namespace MTK.Modules.Payments.Application.Rates.Commands.CreateRate;

internal sealed class CreateRateCommandValidator : AbstractValidator<CreateRateCommand>
{
    public CreateRateCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Tarif məbləği 0-dan böyük olmalıdır");

        RuleFor(x => x.RateType)
            .IsInEnum()
            .WithMessage("Tarif tipi düzgün deyil");

        RuleFor(x => x.EffectiveFrom)
            .NotEmpty()
            .WithMessage("Qüvvəyə minmə tarixi tələb olunur");

        RuleFor(x => x.GarageType)
            .Null()
            .When(x => x.RateType != RateType.FixedGarage)
            .WithMessage("Qaraj növü yalnız 'FixedGarage' tarif tipi üçün göstərilə bilər");
    }
}
