using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MTK.Modules.Hr.Presentation.Filters;

/// <summary>
/// Verilənlər bazasının məhdudiyyət pozuntularını (silinən qeyd başqa yerdə istifadədədir, təkrar qeyd) 500 əvəzinə
/// başa düşülən 400 cavabına çevirir — HR-də silmə "istifadədədir" yoxlamasını bazanın FK-sı edir.
/// </summary>
public sealed class HrDbExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not DbUpdateException { InnerException: PostgresException pg })
        {
            return;
        }

        (string Code, string Message)? mapped = pg.SqlState switch
        {
            PostgresErrorCodes.ForeignKeyViolation => ("Hr.InUse", "Bu qeyd başqa qeydlər tərəfindən istifadə olunur və silinə bilməz."),
            PostgresErrorCodes.UniqueViolation => ("Hr.Duplicate", "Bu məlumat artıq mövcuddur."),
            _ => null
        };

        if (mapped is null)
        {
            return;
        }

        context.Result = new BadRequestObjectResult(new { error = mapped.Value.Message, code = mapped.Value.Code });
        context.ExceptionHandled = true;
    }
}
