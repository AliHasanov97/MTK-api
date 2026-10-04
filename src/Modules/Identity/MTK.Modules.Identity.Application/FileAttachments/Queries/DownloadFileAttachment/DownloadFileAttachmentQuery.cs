using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.FileAttachments.Queries.DownloadFileAttachment;

public sealed record DownloadFileAttachmentQuery(Guid FileAttachmentId) : IQuery<ExportFileResult>;
