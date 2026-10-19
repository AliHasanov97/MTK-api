using AutoMapper;
using Moq;
using MTK.Modules.Hr.Application.Jobs.GetJobById;
using MTK.Modules.Hr.Domain.Jobs;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class GetJobByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenJobDoesNotExist_ReturnsNotFound()
    {
        var repository = new Mock<IJobRepository>();
        repository.Setup(x => x.GetByIdDefaultAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Job?)null);
        var handler = new GetJobByIdQueryHandler(repository.Object, Mock.Of<IMapper>());

        var result = await handler.Handle(new GetJobByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(JobErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenJobExists_ReturnsMappedResponse()
    {
        var job = Job.Create(Guid.NewGuid(), "Accountant");
        var repository = new Mock<IJobRepository>();
        repository.Setup(x => x.GetByIdDefaultAsync(job.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);
        var expectedResponse = new GetJobByIdResponse { Id = job.Id, Name = job.Name };
        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<GetJobByIdResponse>(job)).Returns(expectedResponse);
        var handler = new GetJobByIdQueryHandler(repository.Object, mapper.Object);

        var result = await handler.Handle(new GetJobByIdQuery(job.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(expectedResponse, result.Value);
    }
}
