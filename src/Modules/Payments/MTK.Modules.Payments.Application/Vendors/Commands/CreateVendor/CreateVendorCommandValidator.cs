using FluentValidation;

namespace MTK.Modules.Payments.Application.Vendors.Commands.CreateVendor;

internal sealed class CreateVendorCommandValidator : AbstractValidator<CreateVendorCommand>
{
    public CreateVendorCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tədarükçünün adı tələb olunur")
            .MaximumLength(300);

        RuleFor(x => x.VendorType)
            .IsInEnum()
            .WithMessage("Tədarükçü tipi düzgün deyil");

        RuleFor(x => x.Voen)
            .MaximumLength(50)
            .WithMessage("VÖEN maksimum 50 simvol ola bilər");

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
