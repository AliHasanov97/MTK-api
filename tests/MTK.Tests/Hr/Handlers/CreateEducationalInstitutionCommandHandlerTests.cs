using AutoMapper;
using Moq;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.EducationalInstitutions.CreateEducationalInstitution;
using MTK.Modules.Hr.Domain.EducationalInstitutions;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class CreateEducationalInstitutionCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesInstitutionPersistsAndReturnsMappedResponse()
    {
        var repository = new Mock<IEducationalInstitutionRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
        EducationalInstitution? savedInstitution = null;
        repository.Setup(x => x.AddAsync(It.IsAny<EducationalInstitution>(), It.IsAny<CancellationToken>()))
            .Callback<EducationalInstitution, CancellationToken>((institution, _) => savedInstitution = institution)
            .Returns(Task.CompletedTask);
        repository.Setup(x => x.GetByIdDefaultAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => savedInstitution);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var response = new CreateEducationalInstitutionResponse { Name = "State University" };
        mapper.Setup(x => x.Map<CreateEducationalInstitutionResponse>(It.IsAny<EducationalInstitution>()))
            .Returns(response);
        var handler = new CreateEducationalInstitutionCommandHandler(repository.Object, unitOfWork.Object, mapper.Object);

        var result = await handler.Handle(new CreateEducationalInstitutionCommand
        {
            Name = "State University",
            Type = EducationalInstitutionType.Ali
        }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(response, result.Value);
        Assert.NotNull(savedInstitution);
        Assert.Equal(EducationalInstitutionType.Ali, savedInstitution!.Type);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
