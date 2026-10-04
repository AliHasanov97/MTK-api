using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.FileAttachments.Queries.GetFileAttachmentDownloadLink;

public sealed record GetFileAttachmentDownloadLinkQuery(Guid FileAttachmentId) : IQuery<string>;
