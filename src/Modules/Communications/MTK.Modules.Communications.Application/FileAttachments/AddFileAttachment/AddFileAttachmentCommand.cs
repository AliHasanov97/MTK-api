using MTK.Common.Application.Messaging;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MTK.Modules.Communications.Application.FileAttachments.AddFileAttachment;

public class AddFileAttachmentCommand : ICommand<AddFileAttachmentResponse>
{
    [Required]
    public IFormFile File { get; set; } = null!;

    [Required]
    public Guid ArticleId { get; set; }

    public bool IsPublic { get; set; } = false;
}
