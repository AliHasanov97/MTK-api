using FluentValidation;

namespace MTK.Modules.Identity.Application.Groups.RemoveUserFromGroup;

internal sealed class RemoveUserFromGroupCommandValidator : AbstractValidator<RemoveUserFromGroupCommand>
{
    public RemoveUserFromGroupCommandValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
