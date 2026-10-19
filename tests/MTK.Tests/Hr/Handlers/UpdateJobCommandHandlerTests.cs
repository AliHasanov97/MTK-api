using AutoMapper;
using Moq;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Jobs;
using MTK.Modules.Hr.Application.Jobs.UpdateJob;
using MTK.Modules.Hr.Domain.Jobs;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class UpdateJobCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenJobIsMissing_ReturnsNotFound()
    {
        var repository = new Mock<IJobRepository>();
        repository.Setup(x => x.GetByIdDefaultAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Job?)null);
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new UpdateJobCommandHandler(repository.Object, unitOfWork.Object, Mock.Of<IMapper>());

        var result = await handler.Handle(new UpdateJobCommand { JobId = Guid.NewGuid(), Name = "New" }, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(MTK.Modules.Hr.Application.Jobs.JobErrors.NotFound, result.Error);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenJobExists_UpdatesAndSaves()
    {
        var job = Job.Create(Guid.NewGuid(), "Old");
        var repository = new Mock<IJobRepository>();
        repository.Setup(x => x.GetByIdDefaultAsync(job.Id, It.IsAny<CancellationToken>())).ReturnsAsync(job);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var response = new UpdateJobResponse { Id = job.Id, Name = "New" };
        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<UpdateJobResponse>(job)).Returns(response);
        var handler = new UpdateJobCommandHandler(repository.Object, unitOfWork.Object, mapper.Object);

        var result = await handler.Handle(new UpdateJobCommand { JobId = job.Id, Name = "New" }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New", job.Name);
        Assert.Same(response, result.Value);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
