using MTK.Modules.Hr.Domain.JobApplications;
using System.Text.RegularExpressions;
using FluentValidation;

namespace MTK.Modules.Hr.Application.JobApplications.UpdateJobApplication;

public sealed class UpdateJobApplicationCommandValidator : AbstractValidator<UpdateJobApplicationCommand>
{
    private static readonly Regex PhoneRegex = new(@"^[\+]?[0-9\-\(\)\s]{7,20}$", RegexOptions.Compiled);

    public UpdateJobApplicationCommandValidator()
    {
RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Ünvan boş ola bilməz.")
            .MaximumLength(500).WithMessage("Ünvan maksimum 500 simvol ola bilər.")
            .When(x => x.Address is not null);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ad boş ola bilməz.")
            .MaximumLength(100).WithMessage("Ad maksimum 100 simvol ola bilər.")
            .When(x => x.Name is not null);

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Soyad boş ola bilməz.")
            .MaximumLength(100).WithMessage("Soyad maksimum 100 simvol ola bilər.")
            .When(x => x.Surname is not null);

        RuleFor(x => x.FathersName)
            .NotEmpty().WithMessage("Ata adı boş ola bilməz.")
            .MaximumLength(100).WithMessage("Ata adı maksimum 100 simvol ola bilər.")
            .When(x => x.FathersName is not null);

        RuleFor(x => x.Telephone)
            .NotEmpty().WithMessage("Telefon boş ola bilməz.")
            .Matches(PhoneRegex).WithMessage("Telefon nömrəsinin formatı düzgün deyil.")
            .When(x => x.Telephone is not null);

        RuleFor(x => x.HomeTelephoneNumber)
            .Matches(PhoneRegex).WithMessage("Ev telefon nömrəsinin formatı düzgün deyil.")
            .When(x => !string.IsNullOrEmpty(x.HomeTelephoneNumber));

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Cinsin dəyəri düzgün deyil.")
            .When(x => x.Gender.HasValue);

        RuleFor(x => x.JobId)
            .NotEmpty().WithMessage("Departament-vəzifə boş ola bilməz.")
            .When(x => x.JobId.HasValue);

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Başlama tarixi boş ola bilməz.")
            .When(x => x.StartDate.HasValue);
    }
}
