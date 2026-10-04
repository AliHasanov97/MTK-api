using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.FileAttachments;

public interface IFileAttachmentRepository : IRepository<FileAttachment>
{
    Task<List<FileAttachment>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
