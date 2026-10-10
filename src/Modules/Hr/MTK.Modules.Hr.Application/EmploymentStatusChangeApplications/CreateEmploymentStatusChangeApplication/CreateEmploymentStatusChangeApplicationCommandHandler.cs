using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.CreateEmploymentStatusChangeApplication;

internal sealed class CreateEmploymentStatusChangeApplicationCommandHandler
    : ICommandHandler<CreateEmploymentStatusChangeApplicationCommand, CreateEmploymentStatusChangeApplicationResponse>
{
    private readonly IEmploymentStatusChangeApplicationRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public CreateEmploymentStatusChangeApplicationCommandHandler(
        IEmploymentStatusChangeApplicationRepository repository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<CreateEmploymentStatusChangeApplicationResponse>> Handle(
        CreateEmploymentStatusChangeApplicationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);
            if (employee == null)
                return Result.Failure<CreateEmploymentStatusChangeApplicationResponse>(EmployeeErrors.NotFound);

            // Eyni statusa dəyişmək olmaz
            if (employee.EmploymentType == request.NewEmploymentType)
                return Result.Failure<CreateEmploymentStatusChangeApplicationResponse>(
                    EmploymentStatusChangeApplicationErrors.SameEmploymentType);

            var supervisor = await _employeeRepository.GetByIdDefaultAsync(request.OrderExecutionSupervisorId, cancellationToken);
            if (supervisor == null)
                return Result.Failure<CreateEmploymentStatusChangeApplicationResponse>(
                    EmploymentStatusChangeApplicationErrors.SupervisorNotFound);

            var application = EmploymentStatusChangeApplication.Create(
                request.EmployeeId,
                employee.EmploymentType,
                request.NewEmploymentType,
                request.OrderExecutionSupervisorId,
                _userContext.UserId);

            await _repository.AddAsync(application, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new CreateEmploymentStatusChangeApplicationResponse
            {
                Id = application.Id,
                ApplicationNumber = application.ApplicationNumber
            });
        }
        catch (Exception)
        {
            return Result.Failure<CreateEmploymentStatusChangeApplicationResponse>(
                EmploymentStatusChangeApplicationErrors.AddFailed);
        }
    }
}
