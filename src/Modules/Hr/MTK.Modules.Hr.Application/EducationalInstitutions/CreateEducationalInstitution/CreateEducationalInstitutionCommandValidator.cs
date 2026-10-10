using FluentValidation;

namespace MTK.Modules.Hr.Application.EducationalInstitutions.CreateEducationalInstitution;

public sealed class CreateEducationalInstitutionCommandValidator : AbstractValidator<CreateEducationalInstitutionCommand>
{
    public CreateEducationalInstitutionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ad tələb olunur.")
            .MaximumLength(200).WithMessage("Ad maksimum 200 simvol ola bilər.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Təhsil müəssisəsi tipinin dəyəri düzgün deyil.");
    }
}
