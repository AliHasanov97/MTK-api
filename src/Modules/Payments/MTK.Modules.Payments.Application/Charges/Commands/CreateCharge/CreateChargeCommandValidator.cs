using FluentValidation;

namespace MTK.Modules.Payments.Application.Charges.Commands.CreateCharge;

internal sealed class CreateChargeCommandValidator : AbstractValidator<CreateChargeCommand>
{
    public CreateChargeCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.PropertyId).NotEmpty();

        RuleFor(x => x.PropertyType)
            .IsInEnum()
            .WithMessage("Əmlak tipi düzgün deyil");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Məbləğ 0-dan böyük olmalıdır");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Təsvir tələb olunur")
            .MaximumLength(500);

        // Verilərsə "yyyy-MM" olmalıdır: bu dəyər həm unikallıq açarıdır, həm də
        // aylıq generasiya ilə toqquşmamalıdır. Sərbəst mətn (məs. "2026-9") eyni
        // ay üçün ikinci borc yaradır və dövr ardıcıllığını pozur.
        RuleFor(x => x.Period)
            .Matches(@"^\d{4}-(0[1-9]|1[0-2])$")
            .When(x => !string.IsNullOrWhiteSpace(x.Period))
            .WithMessage("Dövr \"yyyy-MM\" formatında olmalıdır (məs. 2026-09)");
    }
}
