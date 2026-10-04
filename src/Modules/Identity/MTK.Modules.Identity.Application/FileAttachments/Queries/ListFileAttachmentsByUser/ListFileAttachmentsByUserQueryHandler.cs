using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.FileAttachments;

namespace MTK.Modules.Identity.Application.FileAttachments.Queries.ListFileAttachmentsByUser;

internal sealed class ListFileAttachmentsByUserQueryHandler
    : IQueryHandler<ListFileAttachmentsByUserQuery, List<FileAttachmentResponse>>
{
    private readonly IFileAttachmentRepository _fileAttachmentRepository;

    public ListFileAttachmentsByUserQueryHandler(IFileAttachmentRepository fileAttachmentRepository)
    {
        _fileAttachmentRepository = fileAttachmentRepository;
    }

    public async Task<Result<List<FileAttachmentResponse>>> Handle(
        ListFileAttachmentsByUserQuery request,
        CancellationToken cancellationToken)
    {
        var attachments = await _fileAttachmentRepository.ListByUserIdAsync(request.UserId, cancellationToken);

        return attachments
            .Select(a => new FileAttachmentResponse(a.Id, a.FileName, a.ContentType, a.SizeBytes, a.CreatedAt))
            .ToList();
    }
}
