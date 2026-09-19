using FluentValidation;

namespace MTK.Modules.Identity.Application.Groups.AddUserToGroup;

internal sealed class AddUserToGroupCommandValidator : AbstractValidator<AddUserToGroupCommand>
{
    public AddUserToGroupCommandValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
