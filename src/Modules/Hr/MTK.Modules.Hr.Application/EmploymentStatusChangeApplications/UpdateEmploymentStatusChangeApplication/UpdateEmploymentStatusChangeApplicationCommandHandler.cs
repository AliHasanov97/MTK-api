using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.UpdateEmploymentStatusChangeApplication;

internal sealed class UpdateEmploymentStatusChangeApplicationCommandHandler
    : ICommandHandler<UpdateEmploymentStatusChangeApplicationCommand>
{
    private readonly IEmploymentStatusChangeApplicationRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmploymentStatusChangeApplicationCommandHandler(
        IEmploymentStatusChangeApplicationRepository repository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateEmploymentStatusChangeApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (application == null)
            return Result.Failure(EmploymentStatusChangeApplicationErrors.NotFound);

        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure(EmploymentStatusChangeApplicationErrors.CannotUpdateConvertedApplication);

        var employee = await _employeeRepository.GetByIdDefaultAsync(application.EmployeeId, cancellationToken);
        if (employee == null)
            return Result.Failure(EmployeeErrors.NotFound);

        // NewEmploymentType dəyişirsə, işçinin cari statusu ilə eyni olmamalıdır
        if (request.NewEmploymentType.HasValue && employee.EmploymentType == request.NewEmploymentType.Value)
            return Result.Failure(EmploymentStatusChangeApplicationErrors.SameEmploymentType);

        var supervisor = await _employeeRepository.GetByIdDefaultAsync(request.OrderExecutionSupervisorId, cancellationToken);
        if (supervisor == null)
            return Result.Failure(EmploymentStatusChangeApplicationErrors.SupervisorNotFound);

        application.Update(
            request.NewEmploymentType,
            request.OrderExecutionSupervisorId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
