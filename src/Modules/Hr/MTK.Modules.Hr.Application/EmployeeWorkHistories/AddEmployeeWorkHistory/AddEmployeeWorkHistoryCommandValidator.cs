using FluentValidation;

namespace MTK.Modules.Hr.Application.EmployeeWorkHistories.AddEmployeeWorkHistory;

public sealed class AddEmployeeWorkHistoryCommandValidator : AbstractValidator<AddEmployeeWorkHistoryCommand>
{
    public AddEmployeeWorkHistoryCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("İşçi ID boş ola bilməz.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Şirkət adı boş ola bilməz.")
            .MaximumLength(200).WithMessage("Şirkət adı maksimum 200 simvol ola bilər.");

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Vəzifə boş ola bilməz.")
            .MaximumLength(200).WithMessage("Vəzifə maksimum 200 simvol ola bilər.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Başlama tarixi boş ola bilməz.");

        RuleFor(x => x.EndDate)
            .Must((command, endDate) => !endDate.HasValue || endDate.Value >= command.StartDate)
            .WithMessage("Bitmə tarixi başlama tarixindən əvvəl ola bilməz.")
            .When(x => x.EndDate.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Qeydlər maksimum 1000 simvol ola bilər.")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
