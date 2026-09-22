using FluentValidation;

namespace MTK.Modules.Buildings.Application.Apartments.Commands.TransferApartmentOwnership;

internal sealed class TransferApartmentOwnershipCommandValidator
    : AbstractValidator<TransferApartmentOwnershipCommand>
{
    public TransferApartmentOwnershipCommandValidator()
    {
        RuleFor(x => x.ApartmentId)
            .NotEmpty()
            .WithMessage("Mənzil ID-si boş ola bilməz");

        RuleFor(x => x.NewOwnerId)
            .NotEmpty()
            .WithMessage("Yeni sahib ID-si boş ola bilməz");

        RuleFor(x => x.TransferDate)
            .NotEmpty()
            .WithMessage("Transfer tarixi boş ola bilməz")
            .Must(date => date >= DateTime.UtcNow.Date)
            .WithMessage("Transfer tarixi keçmiş ola bilməz");

        RuleFor(x => x.SalePrice)
            .GreaterThan(0)
            .When(x => x.SalePrice.HasValue)
            .WithMessage("Satış qiyməti müsbət olmalıdır");
    }
}
