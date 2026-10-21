using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTK.Common.Application.EventBus;
using MTK.Modules.Communications.Presentation.DebtReminders;
using MTK.Modules.Payments.IntegrationEvents.Payments;

namespace MTK.Modules.Communications.Presentation.Payments;

internal sealed class SendPaymentReceiptIntegrationEventHandler(
    IHttpClientFactory httpClientFactory,
    IOptions<EmailSenderOptions> options,
    ILogger<SendPaymentReceiptIntegrationEventHandler> logger)
    : IntegrationEventHandler<PaymentReceiptEmailRequestedIntegrationEvent>
{
    public override async Task Handle(
        PaymentReceiptEmailRequestedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        EmailSenderOptions email = options.Value;
        if (string.IsNullOrWhiteSpace(email.ApiKey) || string.IsNullOrWhiteSpace(email.FromAddress))
            throw new InvalidOperationException("Communications:Email:ApiKey and FromAddress must be configured to send payment receipts.");

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
                        subject = $"MTK | Ödəniş qəbzi № {integrationEvent.ReceiptNumber}",
                        html = BuildHtml(integrationEvent),
                        text = BuildText(integrationEvent),
                        attachments = new[]
                        {
                            new
                            {
                                filename = integrationEvent.FileName,
                                content = Convert.ToBase64String(integrationEvent.PdfContent)
                            }
                        }
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
                        $"Resend payment receipt request failed with {(int)response.StatusCode}: {responseBody}",
                        null,
                        response.StatusCode);
                }

                break;
            }
            catch (Exception ex) when (attempt < 3 && !cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex,
                    "Communications - Payment receipt email attempt {Attempt} failed for payment {PaymentId}",
                    attempt, integrationEvent.PaymentId);
                await Task.Delay(TimeSpan.FromSeconds(attempt * 5), cancellationToken);
            }
        }

        logger.LogInformation(
            "Communications - Payment receipt accepted by Resend for payment {PaymentId} and owner {OwnerId}",
            integrationEvent.PaymentId,
            integrationEvent.OwnerId);
    }

    private static string BuildHtml(PaymentReceiptEmailRequestedIntegrationEvent receipt) => $$"""
        <!doctype html>
        <html lang="az"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"></head>
        <body style="margin:0;background:#f7f8f4;font-family:Arial,Helvetica,sans-serif;color:#162522;padding:30px 12px">
          <table role="presentation" width="100%" cellspacing="0" cellpadding="0"><tr><td align="center">
            <table role="presentation" width="600" cellspacing="0" cellpadding="0" style="width:100%;max-width:600px;background:#fff;border:1px solid #e5e9e1;border-radius:14px;overflow:hidden">
              <tr><td style="background:#345c4a;padding:23px 30px;color:#fff">
                <table role="presentation" cellspacing="0" cellpadding="0"><tr>
                  <td width="40" height="40" align="center" valign="middle" style="background:#c9e3a1;border-radius:10px;color:#345c4a;font-family:Georgia,serif;font-size:24px;font-weight:bold">V</td>
                  <td style="padding-left:12px"><div style="font-size:18px;font-weight:700">MTK</div><div style="font-size:10px;letter-spacing:1.2px;text-transform:uppercase;color:#d7e5dc;margin-top:4px">Sakin hesabı</div></td>
                </tr></table>
              </td></tr>
              <tr><td style="padding:28px 30px">
                <div style="font-size:21px;line-height:1.3;font-weight:700;letter-spacing:-.4px;margin-bottom:18px">Ödəniş qəbzi</div>
                <p style="font-size:15px;margin:0 0 12px">Hörmətli <strong>{{Html(receipt.OwnerName)}}</strong>,</p>
                <p style="font-size:13px;line-height:1.7;color:#64716c;margin:0">Ödənişiniz qeydə alındı. Qəbzin PDF nüsxəsini bu məktuba əlavə etmişik.</p>
                <div style="margin-top:20px;padding:15px 16px;background:#f7f8f4;border:1px solid #e5e9e1;border-radius:9px">
                  <div style="font-size:11px;color:#64716c">Qəbz nömrəsi</div>
                  <div style="font-size:18px;font-weight:700;color:#345c4a;margin-top:5px">№ {{Html(receipt.ReceiptNumber)}}</div>
                </div>
              </td></tr>
              <tr><td style="padding:18px 30px;background:#f7f8f4;border-top:1px solid #e5e9e1;font-size:11px;color:#64716c">Bu mesaj avtomatik göndərilib. Hörmətlə, MTK.</td></tr>
            </table>
          </td></tr></table>
        </body></html>
        """;

    private static string BuildText(PaymentReceiptEmailRequestedIntegrationEvent receipt) =>
        $"Hörmətli {receipt.OwnerName},{Environment.NewLine}{Environment.NewLine}" +
        $"Ödənişiniz qeydə alındı. Qəbz № {receipt.ReceiptNumber} PDF faylı məktuba əlavə olunub.{Environment.NewLine}" +
        "Hörmətlə, MTK";

    private static string Html(string value) => WebUtility.HtmlEncode(value);
}
