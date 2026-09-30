using FluentValidation;

namespace MTK.Modules.Payments.Application.Vendors.Commands.UpdateVendor;

internal sealed class UpdateVendorCommandValidator : AbstractValidator<UpdateVendorCommand>
{
    public UpdateVendorCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .NotEmpty()
            .WithMessage("Tədarükçü seçilməyib");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tədarükçünün adı tələb olunur")
            .MaximumLength(300);

        RuleFor(x => x.VendorType)
            .IsInEnum()
            .WithMessage("Tədarükçü tipi düzgün deyil");

        RuleFor(x => x.Voen)
            .MaximumLength(50);

        RuleFor(x => x.Director)
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("E-poçt ünvanı düzgün deyil")
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .MaximumLength(50);

        RuleFor(x => x.Address)
            .MaximumLength(500);

        RuleFor(x => x.Note)
            .MaximumLength(1000);
    }
}
