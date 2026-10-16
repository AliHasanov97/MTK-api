using FluentValidation;

namespace MTK.Modules.Payments.Application.SalaryPayments.Commands.PaySalaryPayment;

internal sealed class PaySalaryPaymentCommandValidator : AbstractValidator<PaySalaryPaymentCommand>
{
    public PaySalaryPaymentCommandValidator()
    {
        RuleFor(x => x.SalaryPaymentId)
            .NotEmpty()
            .WithMessage("Maaş ödənişi ID tələb olunur");
    }
}
