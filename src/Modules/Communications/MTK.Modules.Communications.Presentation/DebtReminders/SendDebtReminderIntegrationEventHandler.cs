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
                        subject = "MTK | Borc bildirişi",
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
        string balanceColor = accountBalance > 0 ? "#b42318" : "#345c4a";

        return $$"""
            <!doctype html>
            <html lang="az">
            <head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"></head>
            <body style="margin:0;background:#f7f8f4;font-family:Arial,Helvetica,sans-serif;color:#162522">
              <div style="display:none;max-height:0;overflow:hidden;opacity:0">{{FormatMoney(reminder.OutstandingAmount)}} AZN gecikmiş borcunuz var.</div>
              <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background:#f7f8f4;padding:30px 12px">
                <tr><td align="center">
                  <table role="presentation" width="640" cellspacing="0" cellpadding="0" style="width:100%;max-width:640px;background:#fff;border:1px solid #e5e9e1;border-radius:14px;overflow:hidden">
                    <tr><td style="background:#345c4a;padding:25px 30px;color:#fff">
                      <table role="presentation" cellspacing="0" cellpadding="0"><tr>
                        <td width="40" height="40" align="center" valign="middle" style="background:#c9e3a1;border-radius:10px;color:#345c4a;font-family:Georgia,serif;font-size:24px;font-weight:bold">V</td>
                        <td style="padding-left:12px"><div style="font-size:18px;font-weight:700;letter-spacing:-.3px">MTK</div><div style="font-size:10px;letter-spacing:1.2px;text-transform:uppercase;color:#d7e5dc;margin-top:4px">Sakin hesabı</div></td>
                      </tr></table>
                    </td></tr>
                    <tr><td style="padding:28px 30px 8px">
                      <div style="font-size:22px;line-height:1.3;font-weight:700;letter-spacing:-.5px">Borc bildirişi</div>
                      <p style="font-size:15px;margin:18px 0 10px">Hörmətli <strong>{{Html(reminder.OwnerName)}}</strong>,</p>
                      <p style="font-size:13px;line-height:1.7;color:#64716c;margin:0">Hesabınızda ən azı iki ay üzrə ödənilməmiş mülk haqqı qeydə alınıb. Aşağıda ümumi hesab və mülklər üzrə bölgü göstərilir.</p>
                    </td></tr>
                    <tr><td style="padding:18px 26px 8px">
                      <table role="presentation" width="100%" cellspacing="0" cellpadding="0"><tr>
                        {{SummaryCell("Ümumi hesablanıb", FormatMoney(reminder.TotalDebt), "#f7f8f4", "#162522")}}
                        {{SummaryCell("Ümumi ödənilib", FormatMoney(reminder.TotalPaid), "#eef4e8", "#345c4a")}}
                        {{SummaryCell(statusLabel, FormatMoney(Math.Abs(accountBalance)), "#fff5f3", balanceColor)}}
                      </tr></table>
                    </td></tr>
                    <tr><td style="padding:14px 30px 6px">
                      <div style="padding:16px 18px;border:1px solid #f3c8c3;border-radius:9px;background:#fffaf9">
                        <div style="font-size:11px;color:#7a271a;text-transform:uppercase;letter-spacing:.6px">Bu bildiriş üzrə ödənilməli borc</div>
                        <div style="font-size:27px;line-height:1.3;font-weight:700;color:#b42318;margin-top:5px">{{FormatMoney(reminder.OutstandingAmount)}} <span style="font-size:14px">AZN</span></div>
                        <div style="font-size:12px;color:#7a271a;margin-top:4px">Gecikmiş aylar: {{Html(string.Join(", ", reminder.UnpaidPeriods))}}</div>
                      </div>
                    </td></tr>
                    <tr><td style="padding:24px 30px 7px">
                      <div style="font-size:16px;font-weight:700;letter-spacing:-.2px">Mülklər üzrə borc bölgüsü</div>
                      <div style="font-size:12px;color:#64716c;margin-top:5px">Hesablanmış məbləğ, ödəniş və qalıq</div>
                    </td></tr>
                    <tr><td style="padding:8px 30px 18px">{{propertyCards}}</td></tr>
                    <tr><td style="padding:0 30px 26px">
                      <p style="font-size:12px;line-height:1.7;color:#64716c;margin:0">Zəhmət olmasa, göstərilən qalıq borcu ödəyin. Ödənişi artıq etmisinizsə, bu bildirişi nəzərə almayın; ödənişin hesabda əks olunması bir qədər vaxt ala bilər.</p>
                    </td></tr>
                    <tr><td style="padding:18px 30px;background:#f7f8f4;border-top:1px solid #e5e9e1">
                      <div style="font-size:12px;font-weight:700;color:#345c4a">Hörmətlə, MTK</div>
                      <div style="font-size:11px;color:#87918b;margin-top:6px">Bu mesaj avtomatik göndərilib. Suallarınız üçün bina rəhbərliyi ilə əlaqə saxlayın.</div>
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
              <td style="padding:9px 7px;border-bottom:1px solid #e5e9e1;color:#162522">{{Html(period.Period)}}</td>
              <td align="right" style="padding:9px 7px;border-bottom:1px solid #e5e9e1">{{FormatMoney(period.Amount)}}</td>
              <td align="right" style="padding:9px 7px;border-bottom:1px solid #e5e9e1;color:#345c4a">{{FormatMoney(period.PaidAmount)}}</td>
              <td align="right" style="padding:9px 7px;border-bottom:1px solid #e5e9e1;color:#b42318;font-weight:700">{{FormatMoney(period.OutstandingAmount)}}</td>
            </tr>
            """));

        return $$"""
            <div style="border:1px solid #e5e9e1;border-radius:9px;margin:0 0 12px;overflow:hidden">
              <div style="padding:12px 14px;background:#f7f8f4;font-size:13px;font-weight:700;color:#345c4a">{{Html(property.PropertyLabel)}}</div>
              <div style="padding:0 7px;overflow-x:auto">
                <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="font-size:11px;border-collapse:collapse;min-width:430px">
                  <thead><tr style="color:#64716c;text-align:left">
                    <th style="padding:9px 7px;font-weight:600">Ay</th><th align="right" style="padding:9px 7px">Hesablanıb</th><th align="right" style="padding:9px 7px">Ödənilib</th><th align="right" style="padding:9px 7px">Qalıq</th>
                  </tr></thead><tbody>{{rows}}</tbody>
                  <tfoot><tr style="font-weight:700;background:#fafbf9">
                    <td style="padding:10px 7px">Cəmi</td><td align="right" style="padding:10px 7px">{{FormatMoney(property.TotalAmount)}}</td><td align="right" style="padding:10px 7px;color:#345c4a">{{FormatMoney(property.PaidAmount)}}</td><td align="right" style="padding:10px 7px;color:#b42318">{{FormatMoney(property.OutstandingAmount)}}</td>
                  </tr></tfoot>
                </table>
              </div>
            </div>
            """;
    }

    private static string SummaryCell(string label, string amount, string background, string color) => $$"""
        <td width="33.33%" valign="top" style="padding:0 4px">
          <div style="padding:12px 11px;background:{{background}};border:1px solid #e5e9e1;border-radius:8px;min-height:62px">
            <div style="font-size:10px;line-height:1.4;color:#64716c">{{label}}</div>
            <div style="font-size:15px;font-weight:700;color:{{color}};margin-top:6px">{{amount}} <span style="font-size:10px">AZN</span></div>
          </div>
        </td>
        """;

    private static string BuildText(DebtReminderRequestedIntegrationEvent reminder)
    {
        decimal accountBalance = reminder.TotalDebt - reminder.TotalPaid;
        var lines = new List<string>
        {
            $"Hörmətli {reminder.OwnerName},",
            "Hesabınızda ən azı iki ay üzrə ödənilməmiş mülk haqqı qeydə alınıb.",
            $"Ümumi hesablanıb: {FormatMoney(reminder.TotalDebt)} AZN",
            $"Ümumi ödənilib: {FormatMoney(reminder.TotalPaid)} AZN",
            $"Hesab üzrə qalıq: {FormatMoney(accountBalance)} AZN",
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
