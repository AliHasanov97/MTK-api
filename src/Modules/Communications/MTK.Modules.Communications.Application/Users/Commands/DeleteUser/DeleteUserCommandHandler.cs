using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Communications.Application.Abstractions.Data;
using MTK.Modules.Communications.Domain.Users;

namespace MTK.Modules.Communications.Application.Users.Commands.DeleteUser;

internal sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdDefaultAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            // Artıq silinib və ya heç yaradılmayıb
            return Result.Success();
        }

        user.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
