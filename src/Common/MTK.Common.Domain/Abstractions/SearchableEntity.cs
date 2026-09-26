using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace MTK.Common.Domain.Abstractions;

/// <summary>
/// Base class for entities that support full-text search using PostgreSQL
/// </summary>
public abstract class SearchableEntity : Entity
{
    protected SearchableEntity(Guid id) : base(id)
    {
    }

    protected SearchableEntity() : base()
    {
    }

    /// <summary>
    /// Search vector for PostgreSQL full-text search
    /// This property is computed by the database
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public NpgsqlTsVector SearchVector { get; set; } = null!;
}
