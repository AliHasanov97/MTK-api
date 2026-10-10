using FluentValidation;

namespace MTK.Modules.Hr.Application.FileAttachments.AddFileAttachment;

public sealed class AddFileAttachmentCommandValidator : AbstractValidator<AddFileAttachmentCommand>
{
    public AddFileAttachmentCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("Fayl tələb olunur.");

        RuleFor(x => x.DocumentType)
            .IsInEnum().WithMessage("Sənəd növünün dəyəri düzgün deyil.")
            .When(x => x.DocumentType.HasValue);
    }
}