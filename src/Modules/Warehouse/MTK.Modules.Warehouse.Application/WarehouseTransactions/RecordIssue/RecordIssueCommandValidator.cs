using FluentValidation;

namespace MTK.Modules.Warehouse.Application.WarehouseTransactions.RecordIssue;

public sealed class RecordIssueCommandValidator : AbstractValidator<RecordIssueCommand>
{
    public RecordIssueCommandValidator()
    {
        RuleFor(x => x.NomenclatureId)
            .NotEmpty().WithMessage("Nomenklatura ID boş ola bilməz");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Miqdar 0-dan böyük olmalıdır");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Qeydlər maksimum 500 simvol ola bilər")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
