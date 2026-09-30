using FluentValidation;

namespace MTK.Modules.Payments.Application.Contracts.Commands.AddContractGoodsItem;

internal sealed class AddContractGoodsItemCommandValidator : AbstractValidator<AddContractGoodsItemCommand>
{
    public AddContractGoodsItemCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty()
            .WithMessage("Müqavilə seçilməyib");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Malın adı tələb olunur")
            .MaximumLength(300);

        RuleFor(x => x.Unit)
            .NotEmpty()
            .WithMessage("Ölçü vahidi tələb olunur")
            .MaximumLength(50);

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Vahid qiymət mənfi ola bilməz");

        RuleFor(x => x.AgreedQuantity)
            .GreaterThan(0)
            .When(x => x.AgreedQuantity.HasValue)
            .WithMessage("Gözlənilən miqdar 0-dan böyük olmalıdır");

        RuleFor(x => x.PaymentTermDays)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PaymentTermDays.HasValue)
            .WithMessage("Ödəniş müddəti mənfi ola bilməz");

        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}
