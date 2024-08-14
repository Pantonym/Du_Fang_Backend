using System;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Du_Fang.Services;

public class EmailSender
{

    //dependency injection
    private readonly SendGridClient _client;
    private readonly string _fromEmaill;

    //constructor to initialise EmailSender with needed dependencies
    public EmailSender(IOptions<SendGridOptions> options)
    {
        _client = new SendGridClient(options.Value.ApiKey);
        _fromEmaill = options.Value.FromEmail;
    }

    //method is our service to execute the sending of our email
    public async Task SendEmailAsync(string toEmail, string content, string subject)
    {
        var from = new EmailAddress(_fromEmaill, "Du_Fang");
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);

        await _client.SendEmailAsync(msg);
    }
}

public class SendGridOptions
{
    public string ApiKey { get; set; }
    public string FromEmail { get; set; }
}