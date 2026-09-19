using FluentValidation;

namespace MTK.Modules.Identity.Application.Profile.ChangeUserInfo;

internal sealed class ChangeUserInfoCommandValidator : AbstractValidator<ChangeUserInfoCommand>
{
    public ChangeUserInfoCommandValidator()
    {
        RuleFor(x => x.Password)
            .MinimumLength(8)
            .When(x => !string.IsNullOrWhiteSpace(x.Password))
            .WithMessage("Password must be at least 8 characters long");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
    }
}
