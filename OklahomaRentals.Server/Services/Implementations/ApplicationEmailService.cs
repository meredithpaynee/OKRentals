using System.Net;
using System.Net.Mail;
using System.Text;
using OklahomaRentals.Server.API.Requests;
using OklahomaRentals.Server.Services.Interfaces;

namespace OklahomaRentals.Server.Services.Implementations
{
    // Sends rental applications over SMTP using the "Email" config section:
    //   Email:ApplicationsTo   inbox that receives applications
    //   Email:From             sender address (for Gmail, the account below)
    //   Email:Smtp:Host/Port/Username/Password
    public class ApplicationEmailService : IApplicationEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ApplicationEmailService> _logger;

        public ApplicationEmailService(IConfiguration config, ILogger<ApplicationEmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(RentalApplicationRequest application)
        {
            var to = _config["Email:ApplicationsTo"];
            var from = _config["Email:From"];
            var host = _config["Email:Smtp:Host"];
            var username = _config["Email:Smtp:Username"];
            var password = _config["Email:Smtp:Password"];

            if (string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(host)
                || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Email isn't configured. Set Email:ApplicationsTo, Email:From and Email:Smtp:* (see appsettings.json).");
            }

            using var message = new MailMessage
            {
                From = new MailAddress(from, "Oklahoma College Rentals website"),
                Subject = $"Rental application: {OneLine(application.Name)} for {OneLine(application.Property)}",
                Body = BuildHtml(application),
                IsBodyHtml = true,
            };
            message.To.Add(to);
            // Replying to the email goes straight to the applicant.
            message.ReplyToList.Add(new MailAddress(application.Email, OneLine(application.Name)));

            using var client = new SmtpClient(host, _config.GetValue("Email:Smtp:Port", 587))
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(username, password),
            };

            await client.SendMailAsync(message);
            _logger.LogInformation("Rental application emailed for property {Property}", application.Property);
        }

        // Subject lines and display names can't contain line breaks.
        private static string OneLine(string value) => value.Replace('\r', ' ').Replace('\n', ' ').Trim();

        private static string BuildHtml(RentalApplicationRequest a)
        {
            var html = new StringBuilder();
            html.Append("<div style=\"font-family:Arial,sans-serif;font-size:14px;color:#1f1d1a;max-width:640px\">");
            html.Append("<h2 style=\"font-size:20px;margin:0 0 4px\">New rental application</h2>");
            html.Append($"<p style=\"margin:0 0 16px;color:#5c564e\">Submitted {DateTime.Now:MMMM d, yyyy h:mm tt}. Reply to this email to contact the applicant.</p>");

            Section(html, "Personal information", new()
            {
                ["Name"] = a.Name,
                ["Email"] = a.Email,
                ["Phone"] = a.Phone,
                ["Date of birth"] = a.DateOfBirth,
                ["Driver's license"] = a.DriversLicense,
            });
            Section(html, "School", new()
            {
                ["College / university"] = a.College,
                ["Major"] = a.Major,
                ["Expected graduation"] = a.ExpectedGraduation,
            });
            Section(html, "Property and lease", new()
            {
                ["Property"] = a.Property,
                ["Requested start"] = a.LeaseStart,
                ["Lease term"] = a.LeaseTerm,
            });
            Residence(html, "Current residence", a.CurrentResidence);
            Residence(html, "Previous residence", a.PreviousResidence);
            Section(html, "Background", new()
            {
                ["Ever been evicted"] = YesNo(a.Evicted),
                ["Ever broken a lease"] = YesNo(a.BrokenLease),
                ["Ever filed for bankruptcy"] = YesNo(a.Bankruptcy),
                ["Ever convicted of a felony or misdemeanor"] = YesNo(a.Convicted),
                ["Explanation"] = a.Explanation,
            });

            html.Append("</div>");
            return html.ToString();
        }

        private static void Residence(StringBuilder html, string title, ResidenceHistory? r)
        {
            if (r == null) return;
            Section(html, title, new()
            {
                ["Address"] = r.Address,
                ["Landlord / manager"] = r.Landlord,
                ["Landlord contact"] = r.LandlordContact,
                ["From"] = r.From,
                ["To"] = r.To,
                ["Reason for leaving"] = r.ReasonForLeaving,
            });
        }

        private static string? YesNo(string? answer) => answer?.ToLowerInvariant() switch
        {
            "yes" => "Yes",
            "no" => "No",
            _ => null,
        };

        // Renders a titled table of label/value rows, skipping empty values.
        // Every value is applicant input, so it's HTML-encoded.
        private static void Section(StringBuilder html, string title, Dictionary<string, string?> rows)
        {
            var filled = rows.Where(r => !string.IsNullOrWhiteSpace(r.Value)).ToList();
            if (filled.Count == 0) return;

            html.Append($"<h3 style=\"font-size:15px;margin:20px 0 6px\">{WebUtility.HtmlEncode(title)}</h3>");
            html.Append("<table style=\"border-collapse:collapse;width:100%\">");
            foreach (var (label, value) in filled)
            {
                html.Append("<tr>");
                html.Append($"<td style=\"padding:6px 12px 6px 0;border-bottom:1px solid #ddd7cc;color:#5c564e;width:40%;vertical-align:top\">{WebUtility.HtmlEncode(label)}</td>");
                html.Append($"<td style=\"padding:6px 0;border-bottom:1px solid #ddd7cc;vertical-align:top;white-space:pre-line\">{WebUtility.HtmlEncode(value)}</td>");
                html.Append("</tr>");
            }
            html.Append("</table>");
        }
    }
}
