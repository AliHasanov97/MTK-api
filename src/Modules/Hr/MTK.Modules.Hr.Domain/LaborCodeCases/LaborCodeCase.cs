using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.LaborCodeCases;

public sealed class LaborCodeCase : SearchableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public Guid? ParentId { get; private set; }
    public bool IsActive { get; private set; }

    public LaborCodeCase? Parent { get; private set; }

    private readonly List<LaborCodeCase> _children = new();
    public IReadOnlyCollection<LaborCodeCase> Children => _children.AsReadOnly();

    private LaborCodeCase() { }
}
