using MTK.Common.Application.Messaging;
using System.ComponentModel.DataAnnotations;

namespace MTK.Modules.Hr.Application.FileAttachments.DeleteFileAttachment;

public class DeleteFileAttachmentCommand : ICommand
{
    [Required]
    public Guid Id { get; set; }
}
