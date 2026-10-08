using FluentValidation;

namespace MTK.Modules.Payments.Application.Purchases.Commands.CreatePurchase;

internal sealed class CreatePurchaseCommandValidator : AbstractValidator<CreatePurchaseCommand>
{
    public CreatePurchaseCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .NotEmpty()
            .WithMessage("Tədarükçü seçilməlidir");

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("Alışın ən azı bir sətri olmalıdır");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.NomenclatureId)
                .NotEmpty()
                .WithMessage("Nomenklatura seçilməlidir");

            line.RuleFor(l => l.Quantity)
                .GreaterThan(0)
                .WithMessage("Miqdar 0-dan böyük olmalıdır");

            line.RuleFor(l => l.UnitPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Vahid qiymət mənfi ola bilməz");
        });

        RuleFor(x => x.Note)
            .MaximumLength(1000);
    }
}
