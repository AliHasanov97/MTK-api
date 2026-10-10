using AutoMapper;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.EducationalInstitutions;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Domain.EmployeeEducationHistories;

namespace MTK.Modules.Hr.Application.EmployeeEducationHistories.AddEmployeeEducationHistory;

internal sealed class AddEmployeeEducationHistoryCommandHandler
    : ICommandHandler<AddEmployeeEducationHistoryCommand, AddEmployeeEducationHistoryResponse>
{
    private readonly IEmployeeEducationHistoryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public AddEmployeeEducationHistoryCommandHandler(
        IEmployeeEducationHistoryRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserContext userContext)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<AddEmployeeEducationHistoryResponse>> Handle(
        AddEmployeeEducationHistoryCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var history = EmployeeEducationHistory.Create(
                employeeId: request.EmployeeId,
                educationalInstitutionId: request.EducationalInstitutionId,
                educationLevel: request.EducationLevel,
                faculty: request.Faculty,
                specialty: request.Specialty,
                startDate: request.StartDate,
                endDate: request.EndDate,
                diplomaNumber: request.DiplomaNumber,
                registerNumber: request.RegisterNumber,
                createdById: _userContext.UserId);

            await _repository.AddAsync(history, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var saved = await _repository.GetByIdDefaultAsync(history.Id, cancellationToken);
            return Result.Success(_mapper.Map<AddEmployeeEducationHistoryResponse>(saved));
        }
        catch (Exception)
        {
            return Result.Failure<AddEmployeeEducationHistoryResponse>(
                EmployeeEducationHistoryErrors.AddFailed);
        }
    }
}
