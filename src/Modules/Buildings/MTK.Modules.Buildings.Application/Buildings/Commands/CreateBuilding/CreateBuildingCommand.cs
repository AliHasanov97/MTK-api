using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Buildings.Commands.CreateBuilding;

public sealed record CreateBuildingCommand(
    string Name,
    string Street,
    string City,
    string? District,
    int TotalFloors,
    int ApartmentsPerFloor,
    string? Description = null) : ICommand<Guid>;
