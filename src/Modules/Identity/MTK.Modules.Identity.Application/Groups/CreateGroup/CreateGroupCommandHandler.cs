using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Groups;

namespace MTK.Modules.Identity.Application.Groups.CreateGroup;

internal sealed class CreateGroupCommandHandler : ICommandHandler<CreateGroupCommand, Guid>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGroupCommandHandler(
        IGroupRepository groupRepository,
        IAuthenticationService authenticationService,
        IUnitOfWork unitOfWork)
    {
        _groupRepository = groupRepository;
        _authenticationService = authenticationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        // Create group in Keycloak first
        Guid keycloakGroupId = await _authenticationService.CreateGroupAsync(
            request.Name,
            request.Description);

        // Create group in database
        Group group = Group.Create(
            keycloakGroupId,
            request.Name,
            request.Description,
            request.ParentGroupId);

        _groupRepository.Add(group);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(group.Id);
    }
}
