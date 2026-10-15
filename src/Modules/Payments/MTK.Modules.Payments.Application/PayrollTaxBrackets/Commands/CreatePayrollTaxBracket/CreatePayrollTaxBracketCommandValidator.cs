using FluentValidation;

namespace MTK.Modules.Payments.Application.PayrollTaxBrackets.Commands.CreatePayrollTaxBracket;

internal sealed class CreatePayrollTaxBracketCommandValidator : AbstractValidator<CreatePayrollTaxBracketCommand>
{
    public CreatePayrollTaxBracketCommandValidator()
    {
        RuleFor(x => x.RateType)
            .IsInEnum()
            .WithMessage("Dərəcə növü düzgün deyil");

        RuleFor(x => x.Party)
            .IsInEnum()
            .WithMessage("Tərəf (işçi/işəgötürən) düzgün deyil");

        RuleFor(x => x.LowerBound)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Aşağı həd 0-dan kiçik ola bilməz");

        RuleFor(x => x.UpperBound)
            .GreaterThan(x => x.LowerBound)
            .WithMessage("Yuxarı həd aşağı həddən böyük olmalıdır")
            .When(x => x.UpperBound.HasValue);

        RuleFor(x => x.Rate)
            .InclusiveBetween(0m, 1m)
            .WithMessage("Faiz 0 ilə 1 arasında olmalıdır (məs. 0.22 = 22%)");

        RuleFor(x => x.EffectiveFrom)
            .NotEmpty()
            .WithMessage("Qüvvəyə minmə tarixi tələb olunur");
    }
}
