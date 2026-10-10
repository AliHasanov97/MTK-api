using AutoMapper;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.UnexcusedAbsences;

namespace MTK.Modules.Hr.Application.UnexcusedAbsences.AddUnexcusedAbsence;

internal sealed class AddUnexcusedAbsenceCommandHandler : ICommandHandler<AddUnexcusedAbsenceCommand, AddUnexcusedAbsenceResponse>
{
    private readonly IUnexcusedAbsenceRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public AddUnexcusedAbsenceCommandHandler(
        IUnexcusedAbsenceRepository repository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserContext userContext)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<AddUnexcusedAbsenceResponse>> Handle(AddUnexcusedAbsenceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);

            var absence = UnexcusedAbsenceOrder.Create(
                request.EmployeeId,
                request.SetDate,
                _userContext.UserId);

            await _repository.AddAsync(absence, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(_mapper.Map<AddUnexcusedAbsenceResponse>(absence));
        }
        catch (Exception)
        {
            return Result.Failure<AddUnexcusedAbsenceResponse>(UnexcusedAbsenceErrors.AddFailed);
        }
    }
}
