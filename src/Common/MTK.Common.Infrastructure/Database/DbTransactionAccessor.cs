using MTK.Common.Application.Data;
using System.Data.Common;

namespace MTK.Common.Infrastructure.Database;

internal sealed class DbTransactionAccessor : IDbTransactionAccessor
{
    public DbConnection? Connection { get; set; }
    public DbTransaction? Transaction { get; set; }
}
