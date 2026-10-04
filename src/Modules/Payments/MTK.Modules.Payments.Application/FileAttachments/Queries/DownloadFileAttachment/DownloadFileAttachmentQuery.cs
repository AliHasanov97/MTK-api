using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.FileAttachments.Queries.DownloadFileAttachment;

public sealed record DownloadFileAttachmentQuery(Guid FileAttachmentId) : IQuery<ExportFileResult>;
