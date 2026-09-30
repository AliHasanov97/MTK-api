using FluentValidation;

namespace MTK.Modules.Payments.Application.Contracts.Commands.UpdateContract;

internal sealed class UpdateContractCommandValidator : AbstractValidator<UpdateContractCommand>
{
    public UpdateContractCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty()
            .WithMessage("Müqavilə seçilməyib");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("Bitmə tarixi başlanğıc tarixindən əvvəl ola bilməz");

        RuleFor(x => x.Note)
            .MaximumLength(1000);
    }
}
