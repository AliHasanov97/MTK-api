using AutoMapper;
using MTK.Modules.Hr.Application.FileAttachments;
using MTK.Modules.Hr.Application.FileAttachments.AddBulkFileAttachment;
using MTK.Modules.Hr.Application.FileAttachments.AddFileAttachment;
using MTK.Modules.Hr.Application.FileAttachments.SearchFileAttachments;
using MTK.Modules.Hr.Domain.FileAttachments;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class FileAttachmentProfile : Profile
{
    public FileAttachmentProfile()
    {
        CreateMap<FileAttachment, AddFileAttachmentResponse>();
        CreateMap<FileAttachment, FileAttachmentItem>();
        CreateMap<FileAttachment, SearchFileAttachmentsResponseItem>();
    }
}
