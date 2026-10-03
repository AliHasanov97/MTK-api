using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.GetGroupById;

internal sealed class GetGroupByIdQueryHandler : IQueryHandler<GetGroupByIdQuery, GroupDetailResponse>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public GetGroupByIdQueryHandler(IAuthenticationService authenticationService, IMapper mapper)
    {
        _authenticationService = authenticationService;
        _mapper = mapper;
    }

    public async Task<Result<GroupDetailResponse>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        Abstractions.GroupDto? group = await _authenticationService.GetGroupByIdAsync(request.Id, cancellationToken);

        if (group is null)
        {
            return Result.Failure<GroupDetailResponse>(
                new Error("Group.NotFound", $"Group with ID '{request.Id}' was not found."));
        }

        return Result.Success(_mapper.Map<GroupDetailResponse>(group));
    }
}
