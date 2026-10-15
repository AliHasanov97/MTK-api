using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Payroll;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Commands.ApprovePayrollRun;

internal sealed class ApprovePayrollRunCommandHandler : ICommandHandler<ApprovePayrollRunCommand, PayrollRunResponse>
{
    private readonly IPayrollRunRepository _payrollRunRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApprovePayrollRunCommandHandler(IPayrollRunRepository payrollRunRepository, IUnitOfWork unitOfWork)
    {
        _payrollRunRepository = payrollRunRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PayrollRunResponse>> Handle(ApprovePayrollRunCommand request, CancellationToken cancellationToken)
    {
        var run = await _payrollRunRepository.GetWithPayslipsAsync(request.Id, cancellationToken);
        if (run is null)
        {
            return Result.Failure<PayrollRunResponse>(PayrollErrors.RunNotFound(request.Id));
        }

        if (run.Status != PayrollRunStatus.Calculated)
        {
            return Result.Failure<PayrollRunResponse>(PayrollErrors.NotCalculated);
        }

        run.Approve();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(PayrollRunResponse.FromDomain(run));
    }
}
