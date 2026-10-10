using System.Text.RegularExpressions;
using FluentValidation;

namespace MTK.Modules.Hr.Application.Employees.AddEmployee;

public sealed class AddEmployeeCommandValidator : AbstractValidator<AddEmployeeCommand>
{
    private static readonly Regex PhoneRegex = new(@"^[\+]?[0-9\-\(\)\s]{7,20}$", RegexOptions.Compiled);

    public AddEmployeeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ad tələb olunur.")
            .MaximumLength(100).WithMessage("Ad maksimum 100 simvol ola bilər.");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Soyad tələb olunur.")
            .MaximumLength(100).WithMessage("Soyad maksimum 100 simvol ola bilər.");

        RuleFor(x => x.FathersName)
            .NotEmpty().WithMessage("Ata adı tələb olunur.")
            .MaximumLength(100).WithMessage("Ata adı maksimum 100 simvol ola bilər.");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Cinsin dəyəri düzgün deyil.");

        RuleFor(x => x.StartWorkDate)
            .NotEmpty().WithMessage("İşə başlama tarixi tələb olunur.");

RuleFor(x => x.JobId)
            .NotEmpty().WithMessage("Vəzifə tələb olunur.");

        RuleFor(x => x.PhoneNumber)
            .Matches(PhoneRegex).WithMessage("Telefon nömrəsinin formatı düzgün deyil.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.HomePhoneNumber)
            .Matches(PhoneRegex).WithMessage("Ev telefon nömrəsinin formatı düzgün deyil.")
            .When(x => !string.IsNullOrEmpty(x.HomePhoneNumber));

        RuleFor(x => x.CurrentAddress)
            .MaximumLength(500).WithMessage("Ünvan maksimum 500 simvol ola bilər.")
            .When(x => x.CurrentAddress is not null);
    }
}
