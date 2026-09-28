using FluentValidation;

namespace MTK.Modules.Payments.Application.Rates.Commands.UpdateRate;

internal sealed class UpdateRateCommandValidator : AbstractValidator<UpdateRateCommand>
{
    public UpdateRateCommandValidator()
    {
        RuleFor(x => x.RateId)
            .NotEmpty()
            .WithMessage("Tarif ID-si tələb olunur");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Tarif məbləği 0-dan böyük olmalıdır");
    }
}
