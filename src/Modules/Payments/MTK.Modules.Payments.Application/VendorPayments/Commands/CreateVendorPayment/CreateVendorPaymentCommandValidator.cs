using FluentValidation;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.VendorPayments.Commands.CreateVendorPayment;

internal sealed class CreateVendorPaymentCommandValidator : AbstractValidator<CreateVendorPaymentCommand>
{
    public CreateVendorPaymentCommandValidator()
    {
        RuleFor(x => x.VendorChargeId)
            .NotEmpty()
            .WithMessage("Borc seçilməyib");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Ödəniş məbləği müsbət olmalıdır");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum()
            .WithMessage("Ödəniş üsulu düzgün deyil");

        RuleFor(x => x.Reference)
            .MaximumLength(200)
            .WithMessage("Sənəd nömrəsi çox uzundur");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Qeyd çox uzundur");
    }
}
