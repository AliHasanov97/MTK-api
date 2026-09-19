using FluentValidation;

namespace MTK.Modules.Identity.Application.Roles.CreateRealmRole;

internal sealed class CreateRealmRoleCommandValidator : AbstractValidator<CreateRealmRoleCommand>
{
    public CreateRealmRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.RoleType)
            .IsInEnum();
    }
}
