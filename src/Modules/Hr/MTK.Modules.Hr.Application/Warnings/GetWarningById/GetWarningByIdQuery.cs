using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.Warnings.GetWarningById;

public sealed class GetWarningByIdQuery : IQuery<GetWarningByIdResponse>
{
    public Guid Id { get; set; }
}
