using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Users;

namespace MTK.Modules.Payments.Application.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Integration event-lər təkrar çatdırıla bilər (at-least-once) — artıq
        // mövcuddursa, yenidən yaratmaq əvəzinə heç nə etmirik.
        var existing = await _userRepository.GetByIdDefaultAsync(request.UserId, cancellationToken);
        if (existing is not null)
        {
            return Result.Success();
        }

        var user = User.Create(
            request.UserId,
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.IdentityId);

        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
