using FluentValidation;

namespace MTK.Modules.Identity.Application.Groups.RemoveRolesFromGroup;

internal sealed class RemoveRolesFromGroupCommandValidator : AbstractValidator<RemoveRolesFromGroupCommand>
{
    public RemoveRolesFromGroupCommandValidator()
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
