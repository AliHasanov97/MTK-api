using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Payroll.Services;

namespace MTK.Modules.Payments.Application.Payroll.Commands.CalculatePayrollRun;

internal sealed class CalculatePayrollRunCommandHandler : ICommandHandler<CalculatePayrollRunCommand, PayrollRunResponse>
{
    private readonly IPayrollRunGenerationService _payrollRunGenerationService;

    public CalculatePayrollRunCommandHandler(IPayrollRunGenerationService payrollRunGenerationService)
    {
        _payrollRunGenerationService = payrollRunGenerationService;
    }

    public Task<Result<PayrollRunResponse>> Handle(CalculatePayrollRunCommand request, CancellationToken cancellationToken)
    {
        return _payrollRunGenerationService.GenerateAsync(request.Year, request.Month, cancellationToken);
    }
}
