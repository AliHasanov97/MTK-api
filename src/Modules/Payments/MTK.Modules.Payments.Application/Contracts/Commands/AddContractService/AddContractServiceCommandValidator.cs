using FluentValidation;

namespace MTK.Modules.Payments.Application.Contracts.Commands.AddContractService;

internal sealed class AddContractServiceCommandValidator : AbstractValidator<AddContractServiceCommand>
{
    public AddContractServiceCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty()
            .WithMessage("Müqavilə seçilməyib");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Xidmətin adı tələb olunur")
            .MaximumLength(300);

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Vahid qiymət mənfi ola bilməz");

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
