using FluentValidation;

namespace MTK.Modules.Payments.Application.VendorCharges.Commands.RecordGoodsDelivery;

internal sealed class RecordGoodsDeliveryCommandValidator : AbstractValidator<RecordGoodsDeliveryCommand>
{
    public RecordGoodsDeliveryCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty()
            .WithMessage("Müqavilə seçilməyib");

        RuleFor(x => x.GoodsItemId)
            .NotEmpty()
            .WithMessage("Mal sətri seçilməyib");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Tədarük miqdarı müsbət olmalıdır");

        RuleFor(x => x.Reference)
            .MaximumLength(200)
            .WithMessage("Qaimə nömrəsi çox uzundur");
    }
}
