using FluentValidation;

namespace MTK.Modules.Identity.Application.Users.AssignRolesToUser;

internal sealed class AssignRolesToUserCommandValidator : AbstractValidator<AssignRolesToUserCommand>
{
    public AssignRolesToUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.RoleNames)
            .NotEmpty()
            .Must(roles => roles.Count > 0)
            .WithMessage("At least one role must be specified");

        RuleForEach(x => x.RoleNames)
            .NotEmpty()
            .WithMessage("Role name cannot be empty");
    }
}
