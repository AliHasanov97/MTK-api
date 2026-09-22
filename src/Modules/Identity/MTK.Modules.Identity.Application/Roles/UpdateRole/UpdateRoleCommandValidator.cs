using FluentValidation;

namespace MTK.Modules.Identity.Application.Roles.UpdateRole;

internal sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.OldRoleName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.NewRoleName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
