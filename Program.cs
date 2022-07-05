using System.Diagnostics;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using static System.Console;

string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")!;
EmailClient emailClient = new (connectionString);

WriteLine("Subject:");
string subject = ReadLine()!;
WriteLine();

WriteLine("Body:");
string body = ReadLine()!;
WriteLine();

WriteLine("Sender:");
string sender = ReadLine()!;
WriteLine();

WriteLine("Recipient:");
string recipient = ReadLine()!;
WriteLine();

EmailContent emailContent = new (subject) { PlainText = body };
List<EmailAddress> emailAddresses = new List<EmailAddress>{ new (recipient) };
EmailRecipients emailRecipients = new (emailAddresses);
EmailMessage emailMessage = new (sender, emailContent, emailRecipients);
SendEmailResult emailResult = await emailClient.SendAsync(emailMessage,CancellationToken.None);

SendStatus status;

do
{
    status = emailClient.GetSendStatus(emailResult.MessageId).Value.Status;
    if (status == SendStatus.Queued || status == SendStatus.OutForDelivery)
    {
        WriteLine("Waiting to be sent...");
        Thread.Sleep(TimeSpan.FromSeconds(1));
        continue;
    }
    WriteLine("Error sending mail: ${status}");
    return;
}
while (status != SendStatus.OutForDelivery);

WriteLine("Mail sent");