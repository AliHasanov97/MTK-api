using FluentValidation;

namespace MTK.Modules.Payments.Application.Contracts.Commands.UpdateContractService;

internal sealed class UpdateContractServiceCommandValidator : AbstractValidator<UpdateContractServiceCommand>
{
    public UpdateContractServiceCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty();

        RuleFor(x => x.ServiceId)
            .NotEmpty()
            .WithMessage("Xidmət seçilməyib");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Xidmətin adı tələb olunur")
            .MaximumLength(300);

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Vahid qiymət mənfi ola bilməz");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Miqdar 0-dan böyük olmalıdır");

        RuleFor(x => x.BillingPeriod)
            .IsInEnum()
            .WithMessage("Hesablaşma dövrü düzgün deyil");

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.ServiceEndDate)
            .GreaterThanOrEqualTo(x => x.ServiceStartDate!.Value)
            .When(x => x.ServiceStartDate.HasValue && x.ServiceEndDate.HasValue)
            .WithMessage("Xidmətin bitmə tarixi başlanğıcdan əvvəl ola bilməz");
    }
}
