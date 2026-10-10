using AutoMapper;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.EmployeeEducationHistories;

namespace MTK.Modules.Hr.Application.EmployeeEducationHistories.UpdateEmployeeEducationHistory;

internal sealed class UpdateEmployeeEducationHistoryCommandHandler
    : ICommandHandler<UpdateEmployeeEducationHistoryCommand, UpdateEmployeeEducationHistoryResponse>
{
    private readonly IEmployeeEducationHistoryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateEmployeeEducationHistoryCommandHandler(
        IEmployeeEducationHistoryRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UpdateEmployeeEducationHistoryResponse>> Handle(
        UpdateEmployeeEducationHistoryCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var history = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
            if (history is null)
                return Result.Failure<UpdateEmployeeEducationHistoryResponse>(EmployeeEducationHistoryErrors.NotFound);

            history.Update(
                educationalInstitutionId: request.EducationalInstitutionId,
                educationLevel: request.EducationLevel,
                faculty: request.Faculty,
                specialty: request.Specialty,
                startDate: request.StartDate,
                endDate: request.EndDate,
                diplomaNumber: request.DiplomaNumber,
                registerNumber: request.RegisterNumber);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(_mapper.Map<UpdateEmployeeEducationHistoryResponse>(history));
        }
        catch (NullReferenceException)
        {
            return Result.Failure<UpdateEmployeeEducationHistoryResponse>(EmployeeEducationHistoryErrors.NotFound);
        }
        catch (Exception)
        {
            return Result.Failure<UpdateEmployeeEducationHistoryResponse>(
                EmployeeEducationHistoryErrors.UpdateFailed);
        }
    }
}
