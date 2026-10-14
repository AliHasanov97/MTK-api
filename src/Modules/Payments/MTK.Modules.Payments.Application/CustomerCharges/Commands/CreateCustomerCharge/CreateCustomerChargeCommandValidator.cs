using FluentValidation;

namespace MTK.Modules.Payments.Application.CustomerCharges.Commands.CreateCustomerCharge;

internal sealed class CreateCustomerChargeCommandValidator : AbstractValidator<CreateCustomerChargeCommand>
{
    public CreateCustomerChargeCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Müştəri seçilməyib");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Borcun təsviri tələb olunur")
            .MaximumLength(1000);

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Məbləğ müsbət olmalıdır");
    }
}
