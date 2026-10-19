using AutoMapper;
using Moq;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Application.Employees.GetEmployeeById;
using MTK.Modules.Hr.Domain.Employees;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class GetEmployeeByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenEmployeeIsMissing_ReturnsNotFound()
    {
        var repository = new Mock<IEmployeeRepository>();
        repository.Setup(x => x.GetByIdWithWorkHistoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);
        var handler = new GetEmployeeByIdQueryHandler(repository.Object, Mock.Of<IMapper>());

        var result = await handler.Handle(new GetEmployeeByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(EmployeeErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenEmployeeExists_AddsCalculatedExperienceToResponse()
    {
        var employee = Employee.Create(
            3, "Leyla", "Aliyeva", "Rauf", MTK.Modules.Hr.Domain.JobApplications.Gender.Female,
            DateTimeOffset.UtcNow.AddYears(-2), Guid.NewGuid(), Guid.NewGuid());
        var repository = new Mock<IEmployeeRepository>();
        repository.Setup(x => x.GetByIdWithWorkHistoriesAsync(employee.Id, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        var response = new GetEmployeeByIdResponse();
        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<GetEmployeeByIdResponse>(employee)).Returns(response);
        var handler = new GetEmployeeByIdQueryHandler(repository.Object, mapper.Object);

        var result = await handler.Handle(new GetEmployeeByIdQuery(employee.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(response, result.Value);
        Assert.NotNull(response.TotalWorkExperience);
        Assert.NotNull(response.OrganizationWorkExperience);
        Assert.True(response.TotalWorkExperience!.TotalDays >= 365);
    }
}
