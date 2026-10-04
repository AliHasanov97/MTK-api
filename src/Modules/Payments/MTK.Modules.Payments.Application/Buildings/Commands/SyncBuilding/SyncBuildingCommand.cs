using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Buildings.Commands.SyncBuilding;

/// <summary>
/// Standalone Building upsert — for BuildingCreated/BuildingUpdated, which carry no
/// apartment at all (unlike SyncApartmentCommand, which upserts Building only as a
/// side effect of an apartment sync). Without this, an empty/apartment-less building
/// never reached Payments.
/// </summary>
public sealed record SyncBuildingCommand(Guid BuildingId, string Name, string Address) : ICommand;
