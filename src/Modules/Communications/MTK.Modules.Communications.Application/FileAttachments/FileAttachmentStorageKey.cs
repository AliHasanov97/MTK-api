namespace MTK.Modules.Communications.Application.FileAttachments;

public static class FileAttachmentStorageKey
{
    public static string For(Guid fileAttachmentId) => $"communications/file-attachments/{fileAttachmentId}";
}
