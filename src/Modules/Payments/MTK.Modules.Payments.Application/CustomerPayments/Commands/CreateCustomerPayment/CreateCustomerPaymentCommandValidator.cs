using FluentValidation;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.CustomerPayments.Commands.CreateCustomerPayment;

internal sealed class CreateCustomerPaymentCommandValidator : AbstractValidator<CreateCustomerPaymentCommand>
{
    public CreateCustomerPaymentCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Müştəri seçilməyib");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Ödəniş məbləği müsbət olmalıdır");

        RuleFor(x => x.PaymentMethod)
            .Must(m => m is PaymentMethod.Cash or PaymentMethod.BankTransfer)
            .WithMessage("Yalnız nağd və ya bank köçürməsi qəbul edilir");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Qeyd çox uzundur");
    }
}
