using FluentValidation;

namespace MTK.Modules.Hr.Application.EmployeeWorkHistories.UpdateEmployeeWorkHistory;

public sealed class UpdateEmployeeWorkHistoryCommandValidator : AbstractValidator<UpdateEmployeeWorkHistoryCommand>
{
    public UpdateEmployeeWorkHistoryCommandValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Şirkət adı boş ola bilməz.")
            .MaximumLength(200).WithMessage("Şirkət adı maksimum 200 simvol ola bilər.")
            .When(x => x.CompanyName is not null);

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Vəzifə boş ola bilməz.")
            .MaximumLength(200).WithMessage("Vəzifə maksimum 200 simvol ola bilər.")
            .When(x => x.Position is not null);

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Qeydlər maksimum 1000 simvol ola bilər.")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
