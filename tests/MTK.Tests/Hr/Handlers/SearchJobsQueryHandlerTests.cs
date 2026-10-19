using AutoMapper;
using Moq;
using MTK.Modules.Hr.Application.Jobs.SearchJobs;
using MTK.Modules.Hr.Domain.Jobs;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class SearchJobsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsMappedPageAndUsesRequestedPaging()
    {
        var repository = new Mock<IJobRepository>();
        var mapper = new Mock<IMapper>();
        var jobs = new List<Job> { Job.Create(Guid.NewGuid(), "Accountant") };
        var mapped = new List<SearchJobsResponseItem> { new() { Name = "Accountant" } };
        repository.Setup(x => x.SearchAsync(null, null, "account", 2, 5, It.IsAny<CancellationToken>())).ReturnsAsync(jobs);
        repository.Setup(x => x.CountAsync(null, null, "account", It.IsAny<CancellationToken>())).ReturnsAsync(11);
        mapper.Setup(x => x.Map<List<SearchJobsResponseItem>>(jobs)).Returns(mapped);

        var handler = new SearchJobsQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new SearchJobsQuery { SearchTerm = "account", Page = 2, PageSize = 5 }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(11, result.Value.TotalCount);
        Assert.Equal(2, result.Value.Page);
        Assert.Equal(5, result.Value.PageSize);
        Assert.Same(mapped, result.Value.Data);
        repository.Verify(x => x.SearchAsync(null, null, "account", 2, 5, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.CountAsync(null, null, "account", It.IsAny<CancellationToken>()), Times.Once);
    }
}
