using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.FileAttachments.DownloadFileAttachment;

public sealed record DownloadFileAttachmentQuery(Guid Id) : IQuery<DownloadFileAttachmentResponse>;
