using AutoMapper;
using Moq;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees.AddEmployee;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Jobs;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class AddEmployeeCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenPersistenceFails_ReturnsAddFailed()
    {
        var employees = new Mock<IEmployeeRepository>();
        employees.Setup(x => x.GetNextRegisterNumberAsync(It.IsAny<CancellationToken>())).ReturnsAsync(42);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("database unavailable"));
        var handler = new AddEmployeeCommandHandler(
            employees.Object,
            Mock.Of<IJobRepository>(),
            unitOfWork.Object,
            Mock.Of<IMapper>(),
            Mock.Of<IUserContext>());

        var result = await handler.Handle(new AddEmployeeCommand
        {
            Name = "Leyla",
            Surname = "Aliyeva",
            FathersName = "Rauf",
            Gender = MTK.Modules.Hr.Domain.JobApplications.Gender.Female,
            StartWorkDate = DateTimeOffset.UtcNow,
            JobId = Guid.NewGuid()
        }, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(EmployeeErrors.AddFailed, result.Error);
    }
}
