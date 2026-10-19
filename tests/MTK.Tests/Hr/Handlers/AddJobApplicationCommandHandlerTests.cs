using AutoMapper;
using Moq;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.JobApplications.AddJobApplication;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.Jobs;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class AddJobApplicationCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenJobDoesNotExist_ReturnsFailureWithoutSaving()
    {
        var applications = new Mock<IJobApplicationRepository>();
        var jobs = new Mock<IJobRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
        var user = new Mock<IUserContext>();
        var jobId = Guid.NewGuid();
        jobs.Setup(x => x.GetByIdDefaultAsync(jobId, It.IsAny<CancellationToken>())).ReturnsAsync((Job?)null);

        var handler = new AddJobApplicationCommandHandler(applications.Object, jobs.Object, unitOfWork.Object, mapper.Object, user.Object);
        var result = await handler.Handle(new AddJobApplicationCommand { JobId = jobId }, CancellationToken.None);

        Assert.True(result.IsFailure);
        applications.Verify(x => x.AddAsync(It.IsAny<JobApplication>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenJobExists_PersistsApplicationAndReturnsMappedResponse()
    {
        var applications = new Mock<IJobApplicationRepository>();
        var jobs = new Mock<IJobRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
        var user = new Mock<IUserContext>();
        var jobId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var job = Job.Create(jobId, "Accountant");
        JobApplication? saved = null;
        var expected = new AddJobApplicationResponse { Name = "Aysel" };

        user.SetupGet(x => x.UserId).Returns(userId);
        jobs.Setup(x => x.GetByIdDefaultAsync(jobId, It.IsAny<CancellationToken>())).ReturnsAsync(job);
        applications.Setup(x => x.AddAsync(It.IsAny<JobApplication>(), It.IsAny<CancellationToken>()))
            .Callback<JobApplication, CancellationToken>((application, _) => saved = application)
            .Returns(Task.CompletedTask);
        applications.Setup(x => x.GetByIdDefaultAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => saved);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mapper.Setup(x => x.Map<AddJobApplicationResponse>(It.IsAny<JobApplication>())).Returns(expected);

        var handler = new AddJobApplicationCommandHandler(applications.Object, jobs.Object, unitOfWork.Object, mapper.Object, user.Object);
        var result = await handler.Handle(new AddJobApplicationCommand
        {
            JobId = jobId,
            Name = "Aysel",
            Surname = "Aliyeva",
            Address = "Baku",
            Telephone = "0500000000",
            Gender = Gender.Female,
            StartDate = new DateTimeOffset(2026, 1, 2, 12, 0, 0, TimeSpan.Zero)
        }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(expected, result.Value);
        Assert.NotNull(saved);
        Assert.Equal("Aysel", saved!.Name);
        Assert.Equal(jobId, saved.JobId);
        Assert.Equal(userId, saved.CreatedById);
        applications.Verify(x => x.AddAsync(saved, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
