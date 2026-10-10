using MTK.Modules.Hr.Domain.JobApplications;
using System.Text.RegularExpressions;
using FluentValidation;

namespace MTK.Modules.Hr.Application.JobApplications.AddJobApplication;

public sealed class AddJobApplicationCommandValidator : AbstractValidator<AddJobApplicationCommand>
{
    private static readonly Regex PhoneRegex = new(@"^[\+]?[0-9\-\(\)\s]{7,20}$", RegexOptions.Compiled);

    public AddJobApplicationCommandValidator()
    {
RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Ünvan tələb olunur.")
            .MaximumLength(500).WithMessage("Ünvan maksimum 500 simvol ola bilər.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ad tələb olunur.")
            .MaximumLength(100).WithMessage("Ad maksimum 100 simvol ola bilər.");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Soyad tələb olunur.")
            .MaximumLength(100).WithMessage("Soyad maksimum 100 simvol ola bilər.");

        RuleFor(x => x.FathersName)
            .NotEmpty().WithMessage("Ata adı tələb olunur.")
            .MaximumLength(100).WithMessage("Ata adı maksimum 100 simvol ola bilər.");

        RuleFor(x => x.Telephone)
            .NotEmpty().WithMessage("Telefon tələb olunur.")
            .Matches(PhoneRegex).WithMessage("Telefon nömrəsinin formatı düzgün deyil.");

        RuleFor(x => x.HomeTelephoneNumber)
            .Matches(PhoneRegex).WithMessage("Ev telefon nömrəsinin formatı düzgün deyil.")
            .When(x => !string.IsNullOrEmpty(x.HomeTelephoneNumber));

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Cinsin dəyəri düzgün deyil.");

        RuleFor(x => x.JobId)
            .NotEmpty().WithMessage("Departament-vəzifə tələb olunur.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Başlama tarixi tələb olunur.");
    }
}
