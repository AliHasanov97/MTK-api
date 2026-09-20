using System.Data.Common;

namespace MTK.Common.Application.Data;

public interface IDbTransactionAccessor
{
    DbConnection? Connection { get; }
    DbTransaction? Transaction { get; }
}
