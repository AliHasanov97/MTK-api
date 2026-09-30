using FluentValidation;

namespace MTK.Modules.Payments.Application.Contracts.Commands.CreateContract;

internal sealed class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreateContractCommandValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage("Müqavilə nömrəsi tələb olunur")
            .MaximumLength(100);

        RuleFor(x => x.VendorId)
            .NotEmpty()
            .WithMessage("Tədarükçü seçilməyib");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Başlanğıc tarixi tələb olunur");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("Bitmə tarixi tələb olunur")
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("Bitmə tarixi başlanğıc tarixindən əvvəl ola bilməz");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .WithMessage("Valyuta ISO 4217 kodu olmalıdır (məs. AZN)");

        RuleFor(x => x.Note)
            .MaximumLength(1000);
    }
}
