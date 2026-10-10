using FluentValidation;

namespace MTK.Modules.Hr.Application.Employees.UpdateEmployee;

public sealed class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Nationality)
            .NotEmpty().WithMessage("Milliyyət boş ola bilməz.")
            .MaximumLength(100).WithMessage("Milliyyət maksimum 100 simvol ola bilər.")
            .When(x => x.Nationality is not null);

        RuleFor(x => x.FinCode)
            .NotEmpty().WithMessage("FIN kod boş ola bilməz.")
            .MaximumLength(20).WithMessage("FIN kod maksimum 20 simvol ola bilər.")
            .When(x => x.FinCode is not null);

        RuleFor(x => x.IdCardNumber)
            .NotEmpty().WithMessage("Şəxsiyyət vəsiqəsi nömrəsi boş ola bilməz.")
            .MaximumLength(50).WithMessage("Şəxsiyyət vəsiqəsi nömrəsi maksimum 50 simvol ola bilər.")
            .When(x => x.IdCardNumber is not null);

        RuleFor(x => x.SocialSecurityNumber)
            .NotEmpty().WithMessage("Sosial sığorta nömrəsi boş ola bilməz.")
            .MaximumLength(50).WithMessage("Sosial sığorta nömrəsi maksimum 50 simvol ola bilər.")
            .When(x => x.SocialSecurityNumber is not null);

        RuleFor(x => x.ContractNumber)
            .NotEmpty().WithMessage("Müqavilə nömrəsi boş ola bilməz.")
            .MaximumLength(50).WithMessage("Müqavilə nömrəsi maksimum 50 simvol ola bilər.")
            .When(x => x.ContractNumber is not null);

        RuleFor(x => x.SalaryBankName)
            .NotEmpty().WithMessage("Bank adı boş ola bilməz.")
            .MaximumLength(200).WithMessage("Bank adı maksimum 200 simvol ola bilər.")
            .When(x => x.SalaryBankName is not null);

        RuleFor(x => x.EmployeeBankAccountNumber)
            .NotEmpty().WithMessage("Bank hesab nömrəsi boş ola bilməz.")
            .MaximumLength(50).WithMessage("Bank hesab nömrəsi maksimum 50 simvol ola bilər.")
            .When(x => x.EmployeeBankAccountNumber is not null);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email boş ola bilməz.")
            .EmailAddress().WithMessage("Email formatı düzgün deyil.")
            .MaximumLength(256).WithMessage("Email maksimum 256 simvol ola bilər.")
            .When(x => x.Email is not null);

        RuleFor(x => x.RegisteredAddress)
            .NotEmpty().WithMessage("Qeydiyyat ünvanı boş ola bilməz.")
            .MaximumLength(500).WithMessage("Qeydiyyat ünvanı maksimum 500 simvol ola bilər.")
            .When(x => x.RegisteredAddress is not null);

        RuleFor(x => x.CurrentAddress)
            .NotEmpty().WithMessage("Faktiki ünvan boş ola bilməz.")
            .MaximumLength(500).WithMessage("Faktiki ünvan maksimum 500 simvol ola bilər.")
            .When(x => x.CurrentAddress is not null);

        RuleFor(x => x.NumberOfChildren)
            .GreaterThanOrEqualTo(0).WithMessage("Uşaqların sayı 0-dan kiçik ola bilməz.")
            .When(x => x.NumberOfChildren.HasValue);

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Cinsin dəyəri düzgün deyil.")
            .When(x => x.Gender.HasValue);

        RuleFor(x => x.Education)
            .IsInEnum().WithMessage("Təhsil dəyəri düzgün deyil.")
            .When(x => x.Education.HasValue);

        RuleFor(x => x.EmploymentType)
            .IsInEnum().WithMessage("İş rejimi dəyəri düzgün deyil.")
            .When(x => x.EmploymentType.HasValue);

        RuleFor(x => x.MaritalStatus)
            .IsInEnum().WithMessage("Ailə vəziyyəti dəyəri düzgün deyil.")
            .When(x => x.MaritalStatus.HasValue);

        RuleFor(x => x.MilitaryService)
            .IsInEnum().WithMessage("Hərbi xidmət dəyəri düzgün deyil.")
            .When(x => x.MilitaryService.HasValue);
        
        RuleFor(x => x.WorkingDays)
            .IsInEnum().WithMessage("İş günləri dəyəri düzgün deyil.")
            .When(x => x.WorkingDays.HasValue);

        RuleFor(x => x.VacationDays)
            .GreaterThanOrEqualTo(0).WithMessage("Məzuniyyət günləri 0-dan kiçik ola bilməz.")
            .When(x => x.VacationDays.HasValue);

        RuleFor(x => x.IsActive)
            .IsInEnum().WithMessage("Status dəyəri düzgün deyil.")
            .When(x => x.IsActive.HasValue);
    }
}
