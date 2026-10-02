using FluentValidation;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payments.Commands.CreatePayment;

internal sealed class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        // Hazırda yalnız nağd ödəniş qəbul edilir — bank/kart hələ dəstəklənmir.
        RuleFor(x => x.PaymentMethod)
            .Equal(PaymentMethod.Cash)
            .WithMessage("Yalnız nağd ödəniş qəbul edilir");
    }
}
