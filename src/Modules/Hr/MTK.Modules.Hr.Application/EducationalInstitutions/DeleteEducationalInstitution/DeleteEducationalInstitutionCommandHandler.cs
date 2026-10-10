using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.EducationalInstitutions;
using MTK.Modules.Hr.Domain.EmployeeEducationHistories;

namespace MTK.Modules.Hr.Application.EducationalInstitutions.DeleteEducationalInstitution;

internal sealed class DeleteEducationalInstitutionCommandHandler : ICommandHandler<DeleteEducationalInstitutionCommand>
{
    private readonly IEducationalInstitutionRepository _repository;
    private readonly IEmployeeEducationHistoryRepository _educationHistoryRepository;

    public DeleteEducationalInstitutionCommandHandler(
        IEducationalInstitutionRepository repository,
        IEmployeeEducationHistoryRepository educationHistoryRepository)
    {
        _repository = repository;
        _educationHistoryRepository = educationHistoryRepository;
    }

    public async Task<Result> Handle(DeleteEducationalInstitutionCommand request, CancellationToken cancellationToken)
    {
        // Silmə "soft"dur və FK pozuntusu yaratmır, ona görə istifadədə olan təhsil ocağı əvvəlcədən yoxlanılır.
        var usages = await _educationHistoryRepository.ListAsync(
            h => h.EducationalInstitutionId == request.Id, cancellationToken);
        if (usages.Count > 0)
        {
            return Result.Failure(EducationalInstitutionErrors.InUse);
        }

        await _repository.DeleteAsync(request.Id, null, cancellationToken);
        return Result.Success();
    }
}
