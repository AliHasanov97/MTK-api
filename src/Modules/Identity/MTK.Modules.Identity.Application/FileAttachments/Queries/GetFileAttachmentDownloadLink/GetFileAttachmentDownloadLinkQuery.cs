using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.FileAttachments.Queries.GetFileAttachmentDownloadLink;

public sealed record GetFileAttachmentDownloadLinkQuery(Guid FileAttachmentId) : IQuery<string>;
