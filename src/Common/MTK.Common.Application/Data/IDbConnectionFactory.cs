using System.Data.Common;

namespace MTK.Common.Application.Data;

public interface IDbConnectionFactory
{
    Task<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default);
}
