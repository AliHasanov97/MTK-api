using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.GetApplicationForChangeOfPositionById;

public sealed class GetApplicationForChangeOfPositionByIdQuery : IQuery<GetApplicationForChangeOfPositionByIdResponse>
{
    public Guid Id { get; set; }
}
