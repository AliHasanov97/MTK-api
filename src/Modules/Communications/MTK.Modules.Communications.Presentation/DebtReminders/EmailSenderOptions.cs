namespace MTK.Modules.Communications.Presentation.DebtReminders;

public sealed class EmailSenderOptions
{
    public string ApiKey { get; init; } = string.Empty;
    public string FromAddress { get; init; } = "no-reply@vahid.az";
    public string FromName { get; init; } = "MTK";
}
