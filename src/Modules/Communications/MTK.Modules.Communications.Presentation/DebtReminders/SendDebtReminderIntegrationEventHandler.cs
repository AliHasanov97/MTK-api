using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTK.Common.Application.EventBus;
using MTK.Modules.Payments.IntegrationEvents.DebtReminders;

namespace MTK.Modules.Communications.Presentation.DebtReminders;

internal sealed class SendDebtReminderIntegrationEventHandler(
    IHttpClientFactory httpClientFactory,
    IOptions<EmailSenderOptions> options,
    ILogger<SendDebtReminderIntegrationEventHandler> logger)
    : IntegrationEventHandler<DebtReminderRequestedIntegrationEvent>
{
    private static readonly CultureInfo AzerbaijaniCulture = CultureInfo.GetCultureInfo("az-Latn-AZ");

    public override async Task Handle(
        DebtReminderRequestedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        EmailSenderOptions email = options.Value;
        if (string.IsNullOrWhiteSpace(email.ApiKey) || string.IsNullOrWhiteSpace(email.FromAddress))
            throw new InvalidOperationException("Communications:Email:ApiKey and FromAddress must be configured to send debt reminders.");

        for (int attempt = 1; ; attempt++)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, "emails")
                {
                    Content = JsonContent.Create(new
                    {
                        from = $"{email.FromName} <{email.FromAddress}>",
                        to = new[] { integrationEvent.Email },
                        subject = "MTK | Ödənilməmiş borc barədə bildiriş",
                        html = BuildHtml(integrationEvent),
                        text = BuildText(integrationEvent)
                    })
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", email.ApiKey);
                request.Headers.Add("Idempotency-Key", integrationEvent.IntegrationEventId.ToString("N"));

                using var response = await httpClientFactory.CreateClient("Resend")
                    .SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    throw new HttpRequestException(
                        $"Resend email request failed with {(int)response.StatusCode}: {responseBody}",
                        null,
                        response.StatusCode);
                }

                break;
            }
            catch (Exception ex) when (attempt < 3 && !cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex,
                    "Communications - Debt reminder email attempt {Attempt} failed for owner {OwnerId}",
                    attempt, integrationEvent.OwnerId);
                await Task.Delay(TimeSpan.FromSeconds(attempt * 5), cancellationToken);
            }
        }

        logger.LogInformation(
            "Communications - Debt reminder accepted by Resend for owner {OwnerId} and periods {Periods}",
            integrationEvent.OwnerId,
            string.Join(", ", integrationEvent.UnpaidPeriods));
    }

    private static string BuildHtml(DebtReminderRequestedIntegrationEvent reminder)
    {
        decimal accountBalance = reminder.TotalDebt - reminder.TotalPaid;
        string propertyCards = string.Join("", reminder.Properties.Select(BuildPropertyCard));
        string statusLabel = accountBalance > 0 ? "Ödəniləcək ümumi qalıq" : "Hesab üzrə qalıq";
        string balanceColor = accountBalance > 0 ? "#b42318" : "#067647";

        return $$"""
            <!doctype html>
            <html lang="az">
            <head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"></head>
            <body style="margin:0;background:#f3f5f8;font-family:Arial,Helvetica,sans-serif;color:#182230">
              <div style="display:none;max-height:0;overflow:hidden;opacity:0">{{FormatMoney(reminder.OutstandingAmount)}} AZN ödənilməmiş borcunuz var.</div>
              <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background:#f3f5f8;padding:28px 12px">
                <tr><td align="center">
                  <table role="presentation" width="640" cellspacing="0" cellpadding="0" style="width:100%;max-width:640px;background:#fff;border-radius:16px;overflow:hidden">
                    <tr><td style="background:#123b63;padding:26px 32px;color:#fff">
                      <div style="font-size:13px;letter-spacing:1.5px;text-transform:uppercase;color:#c4d8ed">MTK · SAKİN HESABI</div>
                      <div style="font-size:24px;line-height:1.3;font-weight:700;margin-top:8px">Borc barədə bildiriş</div>
                    </td></tr>
                    <tr><td style="padding:30px 32px 8px">
                      <p style="font-size:16px;margin:0 0 12px">Hörmətli <strong>{{Html(reminder.OwnerName)}}</strong>,</p>
                      <p style="font-size:14px;line-height:1.7;color:#475467;margin:0">Hesabınızda ən azı iki ay üzrə ödənilməmiş mülk haqqı qeydə alınıb. Aşağıda hesabın ümumi xülasəsi və mülklər üzrə bölgü göstərilir.</p>
                    </td></tr>
                    <tr><td style="padding:18px 32px 10px">
                      <table role="presentation" width="100%" cellspacing="0" cellpadding="0"><tr>
                        {{SummaryCell("Ümumi hesablanıb", FormatMoney(reminder.TotalDebt), "#eff6ff", "#1849a9")}}
                        {{SummaryCell("Ümumi ödənilib", FormatMoney(reminder.TotalPaid), "#ecfdf3", "#067647")}}
                        {{SummaryCell(statusLabel, FormatMoney(Math.Abs(accountBalance)), "#fef3f2", balanceColor)}}
                      </tr></table>
                    </td></tr>
                    <tr><td style="padding:10px 32px 4px">
                      <div style="padding:16px 18px;border:1px solid #fecdca;border-radius:10px;background:#fffbfa">
                        <div style="font-size:12px;color:#7a271a;text-transform:uppercase;letter-spacing:.5px">Bu bildiriş üzrə ödənilməli borc</div>
                        <div style="font-size:28px;line-height:1.3;font-weight:700;color:#b42318;margin-top:5px">{{FormatMoney(reminder.OutstandingAmount)}} <span style="font-size:15px">AZN</span></div>
                        <div style="font-size:12px;color:#7a271a;margin-top:4px">Gecikmiş aylar: {{Html(string.Join(", ", reminder.UnpaidPeriods))}}</div>
                      </div>
                    </td></tr>
                    <tr><td style="padding:25px 32px 8px">
                      <div style="font-size:18px;font-weight:700">Mülklər üzrə detallı borc</div>
                      <div style="font-size:12px;color:#667085;margin-top:5px">Hesablanmış məbləğ, həmin borclar üzrə ödəniş və qalıq</div>
                    </td></tr>
                    <tr><td style="padding:8px 32px 20px">{{propertyCards}}</td></tr>
                    <tr><td style="padding:0 32px 28px">
                      <p style="font-size:13px;line-height:1.7;color:#475467;margin:0">Zəhmət olmasa göstərilən qalıq borcu ödəyin. Ödənişi artıq etmisinizsə, bu bildirişi nəzərə almayın; ödənişin hesabda əks olunması bir qədər vaxt ala bilər.</p>
                    </td></tr>
                    <tr><td style="padding:20px 32px;background:#f8fafc;border-top:1px solid #eaecf0">
                      <div style="font-size:13px;font-weight:700;color:#344054">Hörmətlə, MTK</div>
                      <div style="font-size:11px;color:#98a2b3;margin-top:6px">Bu mesaj avtomatik göndərilib. Suallarınız üçün bina rəhbərliyi ilə əlaqə saxlayın.</div>
                    </td></tr>
                  </table>
                </td></tr>
              </table>
            </body>
            </html>
            """;
    }

    private static string BuildPropertyCard(DebtReminderPropertyDetail property)
    {
        string rows = string.Join("", property.Periods.Select(period => $$"""
            <tr>
              <td style="padding:10px 8px;border-bottom:1px solid #eaecf0;color:#344054">{{Html(period.Period)}}</td>
              <td align="right" style="padding:10px 8px;border-bottom:1px solid #eaecf0">{{FormatMoney(period.Amount)}}</td>
              <td align="right" style="padding:10px 8px;border-bottom:1px solid #eaecf0;color:#067647">{{FormatMoney(period.PaidAmount)}}</td>
              <td align="right" style="padding:10px 8px;border-bottom:1px solid #eaecf0;color:#b42318;font-weight:700">{{FormatMoney(period.OutstandingAmount)}}</td>
            </tr>
            """));

        return $$"""
            <div style="border:1px solid #d0d5dd;border-radius:12px;margin:0 0 14px;overflow:hidden">
              <div style="padding:13px 15px;background:#f8fafc;font-size:14px;font-weight:700">{{Html(property.PropertyLabel)}}</div>
              <div style="padding:0 8px;overflow-x:auto">
                <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="font-size:12px;border-collapse:collapse;min-width:440px">
                  <thead><tr style="color:#667085;text-align:left">
                    <th style="padding:9px 8px;font-weight:600">Ay</th><th align="right" style="padding:9px 8px">Hesablanıb</th><th align="right" style="padding:9px 8px">Ödənilib</th><th align="right" style="padding:9px 8px">Qalıq</th>
                  </tr></thead><tbody>{{rows}}</tbody>
                  <tfoot><tr style="font-weight:700;background:#fcfcfd">
                    <td style="padding:11px 8px">Cəmi</td><td align="right" style="padding:11px 8px">{{FormatMoney(property.TotalAmount)}}</td><td align="right" style="padding:11px 8px;color:#067647">{{FormatMoney(property.PaidAmount)}}</td><td align="right" style="padding:11px 8px;color:#b42318">{{FormatMoney(property.OutstandingAmount)}}</td>
                  </tr></tfoot>
                </table>
              </div>
            </div>
            """;
    }

    private static string SummaryCell(string label, string amount, string background, string color) => $$"""
        <td width="33.33%" valign="top" style="padding:0 4px">
          <div style="padding:13px 12px;background:{{background}};border-radius:10px;min-height:66px">
            <div style="font-size:11px;line-height:1.4;color:#475467">{{label}}</div>
            <div style="font-size:17px;font-weight:700;color:{{color}};margin-top:6px">{{amount}} <span style="font-size:11px">AZN</span></div>
          </div>
        </td>
        """;

    private static string BuildText(DebtReminderRequestedIntegrationEvent reminder)
    {
        var lines = new List<string>
        {
            $"Hörmətli {reminder.OwnerName},",
            "Hesabınızda ən azı iki ay üzrə ödənilməmiş mülk haqqı qeydə alınıb.",
            $"Ümumi hesablanıb: {FormatMoney(reminder.TotalDebt)} AZN",
            $"Ümumi ödənilib: {FormatMoney(reminder.TotalPaid)} AZN",
            $"Bu bildiriş üzrə ödənilməli borc: {FormatMoney(reminder.OutstandingAmount)} AZN",
            $"Gecikmiş aylar: {string.Join(", ", reminder.UnpaidPeriods)}",
            "Mülklər üzrə bölgü:"
        };

        foreach (var property in reminder.Properties)
        {
            lines.Add($"{property.PropertyLabel}: hesablanıb {FormatMoney(property.TotalAmount)} AZN, ödənilib {FormatMoney(property.PaidAmount)} AZN, qalıq {FormatMoney(property.OutstandingAmount)} AZN");
            lines.AddRange(property.Periods.Select(period =>
                $"  {period.Period}: hesablanıb {FormatMoney(period.Amount)} AZN, ödənilib {FormatMoney(period.PaidAmount)} AZN, qalıq {FormatMoney(period.OutstandingAmount)} AZN"));
        }

        lines.Add("Ödənişi artıq etmisinizsə, bu bildirişi nəzərə almayın.");
        lines.Add("Hörmətlə, MTK");
        return string.Join(Environment.NewLine, lines);
    }

    private static string FormatMoney(decimal amount) => amount.ToString("N2", AzerbaijaniCulture);
    private static string Html(string value) => WebUtility.HtmlEncode(value);
}
