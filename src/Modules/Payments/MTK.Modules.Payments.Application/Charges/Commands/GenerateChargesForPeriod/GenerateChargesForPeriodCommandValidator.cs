using FluentValidation;
using MTK.Modules.Payments.Application.Charges.Services;

namespace MTK.Modules.Payments.Application.Charges.Commands.GenerateChargesForPeriod;

internal sealed class GenerateChargesForPeriodCommandValidator : AbstractValidator<GenerateChargesForPeriodCommand>
{
    public GenerateChargesForPeriodCommandValidator()
    {
        RuleFor(x => x.Period)
            .Must(ChargePeriod.IsValid)
            .WithMessage("Dövr \"yyyy-MM\" formatında olmalıdır (məs. 2026-09)");
    }
}
