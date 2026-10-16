using FluentValidation;

namespace MTK.Modules.Payments.Application.SalaryPayments.Commands.CreateSalaryPayment;

internal sealed class CreateSalaryPaymentCommandValidator : AbstractValidator<CreateSalaryPaymentCommand>
{
    public CreateSalaryPaymentCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("İşçi seçilməlidir");

        RuleFor(x => x.SalaryMonth)
            .InclusiveBetween(1, 12)
            .WithMessage("Maaş ayı 1-12 arasında olmalıdır");

        RuleFor(x => x.SalaryYear)
            .GreaterThan(2020)
            .LessThanOrEqualTo(2100)
            .WithMessage("İl düzgün deyil");

        RuleFor(x => x.PaymentDate)
            .NotEmpty()
            .WithMessage("Ödəniş tarixi tələb olunur");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
