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

        RuleFor(x => x)
            .Must(x => x.VendorId.HasValue != x.CustomerId.HasValue)
            .WithMessage("Müqavilə tərəfi seçilməlidir: tədarükçü və ya müştəri (ikisi birdən yox)");

        RuleFor(x => x.VendorId)
            .NotEqual(Guid.Empty)
            .When(x => x.VendorId.HasValue)
            .WithMessage("Tədarükçü seçilməyib");

        RuleFor(x => x.CustomerId)
            .NotEqual(Guid.Empty)
            .When(x => x.CustomerId.HasValue)
            .WithMessage("Müştəri seçilməyib");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Başlanğıc tarixi tələb olunur");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("Bitmə tarixi tələb olunur")
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("Bitmə tarixi başlanğıc tarixindən əvvəl ola bilməz");

        RuleFor(x => x.Note)
            .MaximumLength(1000);
    }
}
