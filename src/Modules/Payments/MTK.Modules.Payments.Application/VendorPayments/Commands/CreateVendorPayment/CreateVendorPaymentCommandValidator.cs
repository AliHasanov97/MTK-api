using FluentValidation;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.VendorPayments.Commands.CreateVendorPayment;

internal sealed class CreateVendorPaymentCommandValidator : AbstractValidator<CreateVendorPaymentCommand>
{
    public CreateVendorPaymentCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .NotEmpty()
            .WithMessage("Tədarükçü seçilməyib");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Ödəniş məbləği müsbət olmalıdır");

        // Hazırda yalnız nağd ödəniş qəbul edilir — bank/kart hələ dəstəklənmir.
        RuleFor(x => x.PaymentMethod)
            .Equal(PaymentMethod.Cash)
            .WithMessage("Yalnız nağd ödəniş qəbul edilir");

        RuleFor(x => x.Reference)
            .MaximumLength(200)
            .WithMessage("Sənəd nömrəsi çox uzundur");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Qeyd çox uzundur");
    }
}
