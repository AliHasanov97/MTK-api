using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Domain.Enums;
using System.Text.Json.Serialization;

namespace MTK.Modules.Buildings.Application.Garages.Commands.UpdateGarage;

public sealed class UpdateGarageCommand : ICommand
{
    public GarageType Type { get; set; }
    public string? Description { get; set; }

    [JsonIgnore]
    public Guid GarageId { get; set; }
}
