using MTK.Common.Application.Messaging;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.Commands.DeleteNomenclature;

public sealed record DeleteNomenclatureCommand(Guid Id) : ICommand;
