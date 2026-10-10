using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmploymentOrders.GetEmploymentOrderById;

public sealed class GetEmploymentOrderByIdQuery : IQuery<GetEmploymentOrderByIdResponse>
{
    public GetEmploymentOrderByIdQuery(Guid id) => Id = id;
    public Guid Id { get; }
}
