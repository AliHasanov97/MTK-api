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
    }
}
