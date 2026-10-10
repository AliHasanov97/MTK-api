using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.ConvertToOrder;

public sealed class ConvertEmploymentStatusChangeApplicationToOrderCommand : ICommand<ConvertEmploymentStatusChangeApplicationToOrderResponse>
{
    public Guid ApplicationId { get; set; }
}
