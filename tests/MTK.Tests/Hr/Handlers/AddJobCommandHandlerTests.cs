using AutoMapper;
using Moq;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Jobs.AddJob;
using MTK.Modules.Hr.Domain.Jobs;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class AddJobCommandHandlerTests
{
    [Fact]
    public async Task Handle_AddsJobSavesAndReturnsMappedResponse()
    {
        var repository = new Mock<IJobRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
        Job? addedJob = null;
        var expectedResponse = new AddJobResponse { Name = "Accountant" };

        repository.Setup(x => x.AddAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
            .Callback<Job, CancellationToken>((job, _) => addedJob = job)
            .Returns(Task.CompletedTask);
        repository.Setup(x => x.GetByIdDefaultAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => addedJob);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mapper.Setup(x => x.Map<AddJobResponse>(It.IsAny<Job>())).Returns(expectedResponse);

        var handler = new AddJobCommandHandler(repository.Object, unitOfWork.Object, mapper.Object);
        var result = await handler.Handle(new AddJobCommand { Name = "Accountant" }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(expectedResponse, result.Value);
        Assert.NotNull(addedJob);
        Assert.Equal("Accountant", addedJob!.Name);
        repository.Verify(x => x.AddAsync(addedJob, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
