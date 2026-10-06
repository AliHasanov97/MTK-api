using FluentValidation;

namespace MTK.Modules.Warehouse.Application.WarehouseTransactions.RecordReceipt;

public sealed class RecordReceiptCommandValidator : AbstractValidator<RecordReceiptCommand>
{
    public RecordReceiptCommandValidator()
    {
        RuleFor(x => x.NomenclatureId)
            .NotEmpty().WithMessage("Nomenklatura ID boş ola bilməz");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Miqdar 0-dan böyük olmalıdır");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Vahid qiymət 0-dan kiçik ola bilməz")
            .When(x => x.UnitPrice.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Qeydlər maksimum 500 simvol ola bilər")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
