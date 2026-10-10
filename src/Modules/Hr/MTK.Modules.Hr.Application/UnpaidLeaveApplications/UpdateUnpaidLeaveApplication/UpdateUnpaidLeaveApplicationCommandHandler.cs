using MTK.Modules.Hr.Application.Services;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.UpdateUnpaidLeaveApplication;

internal sealed class UpdateUnpaidLeaveApplicationCommandHandler
    : ICommandHandler<UpdateUnpaidLeaveApplicationCommand>
{
    private readonly IUnpaidLeaveApplicationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILeaveOverlapService _leaveOverlap;

    public UpdateUnpaidLeaveApplicationCommandHandler(
        IUnpaidLeaveApplicationRepository repository,
        IUnitOfWork unitOfWork,
        ILeaveOverlapService leaveOverlap)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _leaveOverlap = leaveOverlap;
    }

    public async Task<Result> Handle(
        UpdateUnpaidLeaveApplicationCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Application tap
        var application = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (application == null)
            return Result.Failure(UnpaidLeaveApplicationErrors.NotFound);

        // 2. Convert olunubsa, yeniləmək olmaz
        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure(UnpaidLeaveApplicationErrors.CannotUpdateConvertedApplication);

        var overlap = await _leaveOverlap.EnsureNoOverlapAsync(
            application.EmployeeId, request.StartDate, request.EndDate, application.Id, cancellationToken);
        if (overlap.IsFailure)
            return overlap;

        // 3. Tarixləri yenilə
        var updateDatesResult = application.UpdateDates(request.StartDate, request.EndDate);
        if (updateDatesResult.IsFailure)
            return updateDatesResult;

        // 4. Qeydləri yenilə
        application.UpdateNotes(request.Notes);

        // 5. Saxla
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}