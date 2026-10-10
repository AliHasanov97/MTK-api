using FluentValidation;

namespace MTK.Modules.Hr.Application.VacationApplications.CreateVacationApplication;

public sealed class CreateVacationApplicationCommandValidator
    : AbstractValidator<CreateVacationApplicationCommand>
{
    public CreateVacationApplicationCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("EmployeeId boş ola bilməz");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("StartDate boş ola bilməz");

        // XOR Validation: EndDate OR RequestedDays (not both, not neither)
        RuleFor(x => x)
            .Must(x => x.EndDate.HasValue ^ x.RequestedDays.HasValue)
            .WithMessage("Yalnız EndDate və ya RequestedDays göndərin")
            .WithName("EndDate/RequestedDays");

        // EndDate validation (when provided)
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("EndDate StartDate-dən əvvəl ola bilməz")
            .When(x => x.EndDate.HasValue);

        // RequestedDays validation (when provided)
        RuleFor(x => x.RequestedDays)
            .GreaterThan(0)
            .WithMessage("RequestedDays 0-dan böyük olmalıdır")
            .When(x => x.RequestedDays.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Qeydlər maksimum 1000 simvol ola bilər")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}