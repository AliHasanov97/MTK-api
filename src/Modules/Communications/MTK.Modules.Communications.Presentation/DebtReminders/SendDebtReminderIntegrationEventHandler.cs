using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTK.Common.Application.EventBus;
using MTK.Modules.Communications.Presentation.Branding;
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

        string displayName = EmailBranding.DisplayName(integrationEvent.CompanyName, email.FromName);
        for (int attempt = 1; ; attempt++)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, "emails")
                {
                    Content = JsonContent.Create(new
                    {
                        from = $"{displayName} <{email.FromAddress}>",
                        to = new[] { integrationEvent.Email },
                        subject = $"{displayName} | Borc bildiri\u015Fi",
                        html = BuildHtml(integrationEvent),
                        text = BuildText(integrationEvent),
                        attachments = new[] { EmailBranding.LogoAttachment }
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
        string statusLabel = accountBalance > 0 ? "&Ouml;d&#601;nil&#601;c&#601;k &uuml;mumi qal&#305;q" : "Hesab &uuml;zr&#601; qal&#305;q";
        string balanceColor = accountBalance > 0 ? "#b42318" : "#345c4a";

        return $$"""
            <!doctype html>
            <html lang="az">
            <head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"></head>
            <body style="margin:0;background:#f7f8f4;font-family:Arial,Helvetica,sans-serif;color:#162522">
              <div style="display:none;max-height:0;overflow:hidden;opacity:0">{{FormatMoney(reminder.OutstandingAmount)}} AZN gecikmi&#351; borcunuz var.</div>
              <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background:#f7f8f4;padding:30px 12px">
                <tr><td align="center">
                  <table role="presentation" width="640" cellspacing="0" cellpadding="0" style="width:100%;max-width:640px;background:#fff;border:1px solid #e5e9e1;border-radius:14px;overflow:hidden">
                    <tr><td style="background:#345c4a;padding:25px 30px;color:#fff">
                      <table role="presentation" cellspacing="0" cellpadding="0"><tr>
                        <td width="88" height="72" align="center" valign="middle" style="background:#fff;border-radius:8px"><img src="cid:{{EmailBranding.LogoContentId}}" alt="{{Html(reminder.CompanyName)}}" width="78" style="display:block;width:78px;height:auto;max-height:66px;border:0"></td>
                        <td style="padding-left:14px"><div style="font-size:18px;font-weight:700;letter-spacing:-.3px">{{Html(reminder.CompanyName)}}</div><div style="font-size:10px;letter-spacing:1.2px;text-transform:uppercase;color:#d7e5dc;margin-top:4px">Sakin hesab&#305;</div></td>
                      </tr></table>
                    </td></tr>
                    <tr><td style="padding:28px 30px 8px">
                      <div style="font-size:22px;line-height:1.3;font-weight:700;letter-spacing:-.5px">Borc bildiri&#351;i</div>
                      <p style="font-size:15px;margin:18px 0 10px">H&ouml;rm&#601;tli <strong>{{Html(reminder.OwnerName)}}</strong>,</p>
                      <p style="font-size:13px;line-height:1.7;color:#64716c;margin:0">Hesab&#305;n&#305;zda &#601;n az&#305; iki ay &uuml;zr&#601; &ouml;d&#601;nilm&#601;mi&#351; m&uuml;lk haqq&#305; qeyd&#601; al&#305;n&#305;b. A&#351;a&#287;&#305;da &uuml;mumi hesab v&#601; m&uuml;lkl&#601;r &uuml;zr&#601; b&ouml;lg&uuml; g&ouml;st&#601;rilir.</p>
                    </td></tr>
                    <tr><td style="padding:18px 26px 8px">
                      <table role="presentation" width="100%" cellspacing="0" cellpadding="0"><tr>
                        {{SummaryCell("&Uuml;mumi hesablan&#305;b", FormatMoney(reminder.TotalDebt), "#f7f8f4", "#162522")}}
                        {{SummaryCell("&Uuml;mumi &ouml;d&#601;nilib", FormatMoney(reminder.TotalPaid), "#eef4e8", "#345c4a")}}
                        {{SummaryCell(statusLabel, FormatMoney(Math.Abs(accountBalance)), "#fff5f3", balanceColor)}}
                      </tr></table>
                    </td></tr>
                    <tr><td style="padding:14px 30px 6px">
                      <div style="padding:16px 18px;border:1px solid #f3c8c3;border-radius:9px;background:#fffaf9">
                        <div style="font-size:11px;color:#7a271a;text-transform:uppercase;letter-spacing:.6px">Bu bildiri&#351; &uuml;zr&#601; &ouml;d&#601;nilm&#601;li borc</div>
                        <div style="font-size:27px;line-height:1.3;font-weight:700;color:#b42318;margin-top:5px">{{FormatMoney(reminder.OutstandingAmount)}} <span style="font-size:14px">AZN</span></div>
                        <div style="font-size:12px;color:#7a271a;margin-top:4px">Gecikmi&#351; aylar: {{Html(string.Join(", ", reminder.UnpaidPeriods))}}</div>
                      </div>
                    </td></tr>
                    <tr><td style="padding:24px 30px 7px">
                      <div style="font-size:16px;font-weight:700;letter-spacing:-.2px">M&uuml;lkl&#601;r &uuml;zr&#601; borc b&ouml;lg&uuml;s&uuml;</div>
                      <div style="font-size:12px;color:#64716c;margin-top:5px">Hesablanm&#305;&#351; m&#601;bl&#601;&#287;, &ouml;d&#601;ni&#351; v&#601; qal&#305;q</div>
                    </td></tr>
                    <tr><td style="padding:8px 30px 18px">{{propertyCards}}</td></tr>
                    <tr><td style="padding:0 30px 26px">
                      <p style="font-size:12px;line-height:1.7;color:#64716c;margin:0">Z&#601;hm&#601;t olmasa, g&ouml;st&#601;ril&#601;n qal&#305;q borcu &ouml;d&#601;yin. &Ouml;d&#601;ni&#351;i art&#305;q etmisinizs&#601;, bu bildiri&#351;i n&#601;z&#601;r&#601; almay&#305;n; &ouml;d&#601;ni&#351;in hesabda &#601;ks olunmas&#305; bir q&#601;d&#601;r vaxt ala bil&#601;r.</p>
                    </td></tr>
                    <tr><td style="padding:18px 30px;background:#f7f8f4;border-top:1px solid #e5e9e1">
                      <div style="font-size:12px;font-weight:700;color:#345c4a">H&ouml;rm&#601;tl&#601;, {{Html(reminder.CompanyName)}}</div>
                      <div style="font-size:11px;color:#87918b;margin-top:6px">Bu mesaj avtomatik g&ouml;nd&#601;rilib. Suallar&#305;n&#305;z &uuml;&ccedil;&uuml;n bina r&#601;hb&#601;rliyi il&#601; &#601;laq&#601; saxlay&#305;n.</div>
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
                    <th style="padding:9px 7px;font-weight:600">Ay</th><th align="right" style="padding:9px 7px">Hesablan&#305;b</th><th align="right" style="padding:9px 7px">&Ouml;d&#601;nilib</th><th align="right" style="padding:9px 7px">Qal&#305;q</th>
                  </tr></thead><tbody>{{rows}}</tbody>
                  <tfoot><tr style="font-weight:700;background:#fafbf9">
                    <td style="padding:10px 7px">C&#601;mi</td><td align="right" style="padding:10px 7px">{{FormatMoney(property.TotalAmount)}}</td><td align="right" style="padding:10px 7px;color:#345c4a">{{FormatMoney(property.PaidAmount)}}</td><td align="right" style="padding:10px 7px;color:#b42318">{{FormatMoney(property.OutstandingAmount)}}</td>
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
            $"H\u00F6rm\u0259tli {reminder.OwnerName},",
            "Hesab\u0131n\u0131zda \u0259n az\u0131 iki ay \u00FCzr\u0259 \u00F6d\u0259nilm\u0259mi\u015F m\u00FClk haqq\u0131 qeyd\u0259 al\u0131n\u0131b.",
            $"\u00DCmumi hesablan\u0131b: {FormatMoney(reminder.TotalDebt)} AZN",
            $"\u00DCmumi \u00F6d\u0259nilib: {FormatMoney(reminder.TotalPaid)} AZN",
            $"Hesab \u00FCzr\u0259 qal\u0131q: {FormatMoney(accountBalance)} AZN",
            $"Bu bildiri\u015F \u00FCzr\u0259 \u00F6d\u0259nilm\u0259li borc: {FormatMoney(reminder.OutstandingAmount)} AZN",
            $"Gecikmi\u015F aylar: {string.Join(", ", reminder.UnpaidPeriods)}",
            "M\u00FClkler \u00FCzr\u0259 b\u00F6lg\u00FC:"
        };

        foreach (var property in reminder.Properties)
        {
            lines.Add($"{property.PropertyLabel}: hesablan\u0131b {FormatMoney(property.TotalAmount)} AZN, \u00F6d\u0259nilib {FormatMoney(property.PaidAmount)} AZN, qal\u0131q {FormatMoney(property.OutstandingAmount)} AZN");
            lines.AddRange(property.Periods.Select(period =>
                $"  {period.Period}: hesablan\u0131b {FormatMoney(period.Amount)} AZN, \u00F6d\u0259nilib {FormatMoney(period.PaidAmount)} AZN, qal\u0131q {FormatMoney(period.OutstandingAmount)} AZN"));
        }

        lines.Add("\u00D6d\u0259ni\u015Fi art\u0131q etmisinizs\u0259, bu bildirisi n\u0259z\u0259r\u0259 almay\u0131n.");
        lines.Add($"H\u00F6rm\u0259tl\u0259, {reminder.CompanyName}");
        return string.Join(Environment.NewLine, lines);
    }

    private static string FormatMoney(decimal amount) => amount.ToString("N2", AzerbaijaniCulture);
    private static string Html(string value) => WebUtility.HtmlEncode(value);
}
