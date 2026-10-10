using MTK.Modules.Hr.Application.Services;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.EducationLeaveApplications;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications.UpdateEducationLeaveApplication;

internal sealed class UpdateEducationLeaveApplicationCommandHandler
    : ICommandHandler<UpdateEducationLeaveApplicationCommand>
{
    private readonly IEducationLeaveApplicationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILeaveOverlapService _leaveOverlap;

    public UpdateEducationLeaveApplicationCommandHandler(
        IEducationLeaveApplicationRepository repository,
        IUnitOfWork unitOfWork,
        ILeaveOverlapService leaveOverlap)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _leaveOverlap = leaveOverlap;
    }

    public async Task<Result> Handle(
        UpdateEducationLeaveApplicationCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Application tap
        var application = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (application == null)
            return Result.Failure(EducationLeaveApplicationErrors.NotFound);

        // 2. Convert olunubsa, yeniləmək olmaz
        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure(EducationLeaveApplicationErrors.CannotUpdateConvertedApplication);

        var overlap = await _leaveOverlap.EnsureNoOverlapAsync(
            application.EmployeeId, request.StartDate, request.EndDate, application.Id, cancellationToken);
        if (overlap.IsFailure)
            return overlap;

        // 3. Məlumatları yenilə
        var updateResult = application.Update(request.StartDate, request.EndDate, request.Reason);
        if (updateResult.IsFailure)
            return updateResult;

        // 4. Saxla
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
