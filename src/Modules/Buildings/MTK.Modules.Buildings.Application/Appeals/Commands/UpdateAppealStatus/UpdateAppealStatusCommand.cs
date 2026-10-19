using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Domain.Appeals;

namespace MTK.Modules.Buildings.Application.Appeals.Commands.UpdateAppealStatus;

public sealed record UpdateAppealStatusCommand(
    Guid AppealId,
    AppealStatus NewStatus) : ICommand;
