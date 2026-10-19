using Moq;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Payroll;
using MTK.Modules.Payments.Application.Payroll.Commands.CalculatePayrollRun;
using MTK.Modules.Payments.Application.Payroll.Services;
using MTK.Modules.Payments.Domain.Payroll;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class CalculatePayrollRunCommandHandlerTests
{
    [Fact]
    public async Task Handle_DelegatesRequestedPeriodToGenerationService()
    {
        var generationService = new Mock<IPayrollRunGenerationService>();
        var response = new PayrollRunResponse(Guid.NewGuid(), 2026, 8, PayrollRunStatus.Calculated,
            DateTimeOffset.UtcNow, null, null, 0m, 0m, 0m, Array.Empty<PayslipResponse>());
        generationService.Setup(x => x.GenerateAsync(2026, 8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(response));

        var handler = new CalculatePayrollRunCommandHandler(generationService.Object);
        var result = await handler.Handle(new CalculatePayrollRunCommand(2026, 8), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(response, result.Value);
        generationService.Verify(x => x.GenerateAsync(2026, 8, It.IsAny<CancellationToken>()), Times.Once);
    }
}
