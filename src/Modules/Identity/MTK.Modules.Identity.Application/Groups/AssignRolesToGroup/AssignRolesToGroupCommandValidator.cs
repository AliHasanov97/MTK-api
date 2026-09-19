using FluentValidation;

namespace MTK.Modules.Identity.Application.Groups.AssignRolesToGroup;

internal sealed class AssignRolesToGroupCommandValidator : AbstractValidator<AssignRolesToGroupCommand>
{
    public AssignRolesToGroupCommandValidator()
    {
        RuleFor(x => x.GroupId)
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
