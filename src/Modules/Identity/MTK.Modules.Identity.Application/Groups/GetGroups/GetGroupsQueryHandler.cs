using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.GetGroups;

internal sealed class GetGroupsQueryHandler : IQueryHandler<GetGroupsQuery, List<GroupResponse>>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public GetGroupsQueryHandler(IAuthenticationService authenticationService, IMapper mapper)
    {
        _authenticationService = authenticationService;
        _mapper = mapper;
    }

    public async Task<Result<List<GroupResponse>>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = await _authenticationService.GetAllGroupsAsync(cancellationToken);

        return Result.Success(_mapper.Map<List<GroupResponse>>(groups));
    }
}
