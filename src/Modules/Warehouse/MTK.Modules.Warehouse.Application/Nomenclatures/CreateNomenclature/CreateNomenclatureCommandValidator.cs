using FluentValidation;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.CreateNomenclature;

public sealed class CreateNomenclatureCommandValidator : AbstractValidator<CreateNomenclatureCommand>
{
    public CreateNomenclatureCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Nomenklatura kodu boş ola bilməz")
            .MaximumLength(50).WithMessage("Nomenklatura kodu maksimum 50 simvol ola bilər");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nomenklatura adı boş ola bilməz")
            .MaximumLength(200).WithMessage("Nomenklatura adı maksimum 200 simvol ola bilər");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Təsvir maksimum 1000 simvol ola bilər")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Düzgün kateqoriya seçin");

        RuleFor(x => x.Unit)
            .IsInEnum().WithMessage("Düzgün ölçü vahidi seçin");

        RuleFor(x => x.MinStockLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum ehtiyat səviyyəsi 0-dan kiçik ola bilməz")
            .When(x => x.MinStockLevel.HasValue);
    }
}
