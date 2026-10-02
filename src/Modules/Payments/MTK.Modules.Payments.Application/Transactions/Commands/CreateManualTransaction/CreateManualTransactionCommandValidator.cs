using FluentValidation;

namespace MTK.Modules.Payments.Application.Transactions.Commands.CreateManualTransaction;

internal sealed class CreateManualTransactionCommandValidator : AbstractValidator<CreateManualTransactionCommand>
{
    public CreateManualTransactionCommandValidator()
    {
        RuleFor(x => x.Direction)
            .IsInEnum()
            .WithMessage("İstiqamət düzgün deyil");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Kateqoriya tələb olunur")
            .MaximumLength(100);

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Məbləğ 0-dan böyük olmalıdır");

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
