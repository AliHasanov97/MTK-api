namespace MTK.Modules.Communications.Presentation.Branding;

internal static class EmailBranding
{
    public const string LogoContentId = "mtk-brand-logo";
    private const string LogoResourceName = "MTK.EmailBranding.logo.png";
    private static readonly Lazy<string> LogoBase64 = new(LoadLogoBase64);

    public static object LogoAttachment => new
    {
        filename = "logo.png",
        content = LogoBase64.Value,
        content_type = "image/png",
        content_id = LogoContentId
    };

    public static string DisplayName(string companyName, string fallbackName) =>
        string.IsNullOrWhiteSpace(companyName) ? fallbackName : companyName.Trim();

    private static string LoadLogoBase64()
    {
        using Stream stream = typeof(EmailBranding).Assembly.GetManifestResourceStream(LogoResourceName)
            ?? throw new InvalidOperationException($"Embedded email logo resource '{LogoResourceName}' was not found.");
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return Convert.ToBase64String(buffer.ToArray());
    }
}
