using FluentValidation;

namespace MTK.Modules.Payments.Application.Charges.Commands.CreateCharge;

internal sealed class CreateChargeCommandValidator : AbstractValidator<CreateChargeCommand>
{
    public CreateChargeCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.PropertyId).NotEmpty();

        RuleFor(x => x.PropertyType)
            .IsInEnum()
            .WithMessage("Əmlak tipi düzgün deyil");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Məbləğ 0-dan böyük olmalıdır");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Təsvir tələb olunur")
            .MaximumLength(500);
    }
}
