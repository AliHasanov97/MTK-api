using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Domain.Users;

namespace MTK.Modules.Buildings.Application.Users.Commands.UpdateUser;

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

        // Snapshot hələ mövcud deyilsə (UserCreated hadisəsi hər səbəbdənsə buraya
        // çatmayıb), update-dən create yarada bilərik — itən məlumat olmasın.
        if (user is null)
        {
            user = User.Create(request.UserId, request.FirstName, request.LastName, request.Email, request.PhoneNumber, request.IdentityId);
            _userRepository.Add(user);
        }
        else
        {
            user.Update(request.FirstName, request.LastName, request.Email, request.PhoneNumber, request.IdentityId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
