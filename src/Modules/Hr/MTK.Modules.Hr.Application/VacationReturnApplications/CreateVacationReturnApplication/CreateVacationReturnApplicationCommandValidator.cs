using FluentValidation;

namespace MTK.Modules.Hr.Application.VacationReturnApplications.CreateVacationReturnApplication;

public sealed class CreateVacationReturnApplicationCommandValidator
    : AbstractValidator<CreateVacationReturnApplicationCommand>
{
    public CreateVacationReturnApplicationCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("EmployeeId boş ola bilməz");

        RuleFor(x => x.ReturnDate)
            .NotEmpty().WithMessage("ReturnDate boş ola bilməz");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Qeydlər maksimum 1000 simvol ola bilər")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}