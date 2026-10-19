using AutoMapper;
using Moq;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Application.Employees.UpdateEmployee;
using MTK.Modules.Hr.Domain.Employees;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class UpdateEmployeeCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenEmployeeIsMissing_ReturnsNotFound()
    {
        var repository = new Mock<IEmployeeRepository>();
        repository.Setup(x => x.GetByIdWithWorkHistoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new UpdateEmployeeCommandHandler(repository.Object, unitOfWork.Object, Mock.Of<IMapper>());

        var result = await handler.Handle(new UpdateEmployeeCommand { Id = Guid.NewGuid() }, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(EmployeeErrors.NotFound, result.Error);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEmployeeExists_UpdatesAndSaves()
    {
        var employee = Employee.Create(
            3, "Leyla", "Aliyeva", "Rauf", MTK.Modules.Hr.Domain.JobApplications.Gender.Female,
            DateTimeOffset.UtcNow.AddYears(-2), Guid.NewGuid(), Guid.NewGuid());
        var repository = new Mock<IEmployeeRepository>();
        repository.Setup(x => x.GetByIdWithWorkHistoriesAsync(employee.Id, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var response = new UpdateEmployeeResponse();
        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<UpdateEmployeeResponse>(employee)).Returns(response);
        var handler = new UpdateEmployeeCommandHandler(repository.Object, unitOfWork.Object, mapper.Object);

        var result = await handler.Handle(new UpdateEmployeeCommand { Id = employee.Id, Name = "Lala" }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Lala", employee.Name);
        Assert.NotNull(result.Value.TotalWorkExperience);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
