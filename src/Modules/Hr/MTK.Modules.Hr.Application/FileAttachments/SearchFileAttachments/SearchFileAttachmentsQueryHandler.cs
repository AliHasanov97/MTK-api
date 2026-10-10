using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.FileAttachments;

namespace MTK.Modules.Hr.Application.FileAttachments.SearchFileAttachments;

public class SearchFileAttachmentsQueryHandler : IQueryHandler<SearchFileAttachmentsQuery, SearchFileAttachmentsResponse>
{
    private readonly IFileAttachmentRepository _fileAttachmentRepository;
    private readonly IMapper _mapper;

    public SearchFileAttachmentsQueryHandler(IFileAttachmentRepository fileAttachmentRepository, IMapper mapper)
    {
        _fileAttachmentRepository = fileAttachmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<SearchFileAttachmentsResponse>> Handle(SearchFileAttachmentsQuery request, CancellationToken cancellationToken)
    {
        var fileAttachments = await _fileAttachmentRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _fileAttachmentRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var data = _mapper.Map<List<SearchFileAttachmentsResponseItem>>(fileAttachments);
        return Result.Success(new SearchFileAttachmentsResponse(data, totalCount, request.Page, request.PageSize));
    }
}
