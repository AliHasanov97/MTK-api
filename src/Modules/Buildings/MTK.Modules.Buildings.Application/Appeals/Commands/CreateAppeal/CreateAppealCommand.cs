using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Domain.Appeals;

namespace MTK.Modules.Buildings.Application.Appeals.Commands.CreateAppeal;

public sealed record CreateAppealCommand(
    string Title,
    string Description,
    AppealCategory Category,
    Guid? ApartmentId,
    Guid? GarageId) : ICommand<Guid>;
