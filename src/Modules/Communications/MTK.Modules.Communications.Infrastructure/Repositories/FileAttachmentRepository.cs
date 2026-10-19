using MTK.Common.Infrastructure.Database;
using MTK.Modules.Communications.Domain.FileAttachments;
using MTK.Modules.Communications.Infrastructure.Database;

namespace MTK.Modules.Communications.Infrastructure.Repositories;

internal sealed class FileAttachmentRepository : SearchableRepository<FileAttachment>, IFileAttachmentRepository
{
    public FileAttachmentRepository(CommunicationsDbContext context) : base(context)
    {
    }
}
