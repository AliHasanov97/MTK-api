using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.DeleteNomenclature;

public sealed record DeleteNomenclatureCommand(Guid Id) : ICommand<Result>;
