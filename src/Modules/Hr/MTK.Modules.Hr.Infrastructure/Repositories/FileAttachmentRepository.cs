using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.FileAttachments;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

public class FileAttachmentRepository : SearchableRepository<FileAttachment>, IFileAttachmentRepository
{
    public FileAttachmentRepository(HrDbContext context) : base(context) { }
}
