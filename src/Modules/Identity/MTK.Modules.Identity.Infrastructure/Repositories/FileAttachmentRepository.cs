using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Identity.Domain.FileAttachments;
using MTK.Modules.Identity.Infrastructure.Database;

namespace MTK.Modules.Identity.Infrastructure.Repositories;

internal sealed class FileAttachmentRepository : Repository<FileAttachment>, IFileAttachmentRepository
{
    private IdentityDbContext IdentityContext => (IdentityDbContext)Context;

    public FileAttachmentRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<FileAttachment>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return IdentityContext.FileAttachments
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
