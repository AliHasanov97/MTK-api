using Moq;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Users.Commands.CreateUser;
using MTK.Modules.Hr.Application.Users.Commands.DeactivateUser;
using MTK.Modules.Hr.Application.Users.Commands.UpdateUser;
using MTK.Modules.Hr.Domain.Users;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class UserSynchronizationHandlerTests
{
    [Fact]
    public async Task CreateUser_WhenUserAlreadyExists_IsIdempotent()
    {
        var userId = Guid.NewGuid();
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdDefaultAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(User.Create(userId, "Ali", "Mammadov", "ali@example.com", null));
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new CreateUserCommandHandler(repository.Object, unitOfWork.Object);

        var result = await handler.Handle(new CreateUserCommand(userId, "Ali", "Mammadov", "ali@example.com", null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        repository.Verify(x => x.Add(It.IsAny<User>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUser_WhenUserDoesNotExist_CreatesSnapshot()
    {
        var userId = Guid.NewGuid();
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdDefaultAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        User? addedUser = null;
        repository.Setup(x => x.Add(It.IsAny<User>())).Callback<User>(user => addedUser = user);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var handler = new UpdateUserCommandHandler(repository.Object, unitOfWork.Object);

        var result = await handler.Handle(new UpdateUserCommand(userId, "Leyla", "Aliyeva", "leyla@example.com", null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(addedUser);
        Assert.Equal(userId, addedUser!.Id);
        Assert.Equal("Leyla Aliyeva", addedUser.FullName);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeactivateUser_WhenUserDoesNotExist_IsIdempotent()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdDefaultAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new DeactivateUserCommandHandler(repository.Object, unitOfWork.Object);

        var result = await handler.Handle(new DeactivateUserCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsSuccess);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
