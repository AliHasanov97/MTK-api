using FluentValidation;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Charges.Commands.CreateOneTimeServiceExpense;

internal sealed class CreateOneTimeServiceExpenseCommandValidator : AbstractValidator<CreateOneTimeServiceExpenseCommand>
{
    public CreateOneTimeServiceExpenseCommandValidator()
    {
        RuleFor(x => x.ContractId).NotEmpty();
        RuleFor(x => x.ContractServiceId).NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Məbləğ 0-dan böyük olmalıdır");

        // Hazırda yalnız nağd ödəniş qəbul edilir — bank/kart hələ dəstəklənmir.
        RuleFor(x => x.PaymentMethod)
            .Equal(PaymentMethod.Cash)
            .WithMessage("Yalnız nağd ödəniş qəbul edilir");

        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
