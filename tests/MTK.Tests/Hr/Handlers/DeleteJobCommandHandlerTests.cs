using System.Linq.Expressions;
using Moq;
using MTK.Modules.Hr.Application.Jobs.DeleteJob;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.Jobs;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class DeleteJobCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenJobIsUsedByEmployee_RejectsDeletion()
    {
        var jobId = Guid.NewGuid();
        var jobRepository = new Mock<IJobRepository>();
        var employeeRepository = new Mock<IEmployeeRepository>();
        employeeRepository.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Employee, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([Employee.Create(
                1, "Ali", "Mammadov", "Rashad", Gender.Male,
                DateTimeOffset.UtcNow, jobId, Guid.NewGuid())]);
        var jobApplicationRepository = new Mock<IJobApplicationRepository>();
        var changeRepository = new Mock<IApplicationForChangeOfPositionRepository>();
        var handler = new DeleteJobCommandHandler(
            jobRepository.Object,
            employeeRepository.Object,
            jobApplicationRepository.Object,
            changeRepository.Object);

        var result = await handler.Handle(new DeleteJobCommand(jobId), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(MTK.Modules.Hr.Application.Jobs.JobErrors.InUse, result.Error);
        jobRepository.Verify(x => x.DeleteAsync(jobId, null, It.IsAny<CancellationToken>()), Times.Never);
        jobApplicationRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenNoRecordsUseJob_DeletesIt()
    {
        var jobId = Guid.NewGuid();
        var jobRepository = new Mock<IJobRepository>();
        var employeeRepository = new Mock<IEmployeeRepository>();
        var jobApplicationRepository = new Mock<IJobApplicationRepository>();
        var changeRepository = new Mock<IApplicationForChangeOfPositionRepository>();
        employeeRepository.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Employee, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        jobApplicationRepository.Setup(x => x.ListAsync(It.IsAny<Expression<Func<JobApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        changeRepository.Setup(x => x.ListAsync(It.IsAny<Expression<Func<ApplicationForChangeOfPosition, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        jobRepository.Setup(x => x.DeleteAsync(jobId, null, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var handler = new DeleteJobCommandHandler(
            jobRepository.Object,
            employeeRepository.Object,
            jobApplicationRepository.Object,
            changeRepository.Object);

        var result = await handler.Handle(new DeleteJobCommand(jobId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        jobRepository.Verify(x => x.DeleteAsync(jobId, null, It.IsAny<CancellationToken>()), Times.Once);
    }
}
