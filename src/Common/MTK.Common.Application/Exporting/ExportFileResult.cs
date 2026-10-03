namespace MTK.Common.Application.Exporting;

/// <summary>
/// Shared shape for a generated-document query (PDF/Excel/Word) response — every
/// export endpoint's controller action just forwards this straight into
/// <c>File(FileContent, ContentType, FileName)</c>.
/// </summary>
public sealed record ExportFileResult(byte[] FileContent, string FileName, string ContentType);
