using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Communications.Application.Abstractions.Data;
using MTK.Modules.Communications.Domain.Users;

namespace MTK.Modules.Communications.Application.Users.Commands.UpdateUser;

internal sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdDefaultAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            // Integration event-lər ardıcıl olmaya bilər — əgər update event-i
            // create-dən əvvəl gəlibsə, sadəcə yaradırıq.
            var newUser = User.Create(
                request.UserId,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.IdentityId);

            _userRepository.Add(newUser);
        }
        else
        {
            user.Update(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.IdentityId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
