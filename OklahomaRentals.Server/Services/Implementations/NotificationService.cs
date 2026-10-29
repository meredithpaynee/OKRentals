using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OklahomaRentals.Server.Services.Interfaces;
using Pingram;
using Pingram.Model;

namespace OklahomaRentals.Server.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _config;

        public NotificationService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendMaintenanceRequestSubmittedAsync(
            string landlordUserId,
            string landlordEmail,
            string landlordPhoneNumber,
            string tenantName,
            string propertyAddress,
            string requestTitle,
            string requestPriority,
            string requestDescription)
        {
            var apiKey = _config["Pingram:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("Pingram API Key is missing.");

            var client = new PingramClient(apiKey, "https://api.pingram.io");

            var body = new SenderPostBody
            {
                Type = "maintenance_request_submitted",
                To = new SenderPostBodyTo
                {
                    Id = landlordUserId,
                    Email = landlordEmail,
                    Number = landlordPhoneNumber
                },
                Parameters = new Dictionary<string, object>
                {
                    { "tenantName", tenantName },
                    { "propertyAddress", propertyAddress },
                    { "requestTitle", requestTitle },
                    { "requestPriority", requestPriority },
                    { "requestDescription", requestDescription }
                },
                TemplateId = "template_one"
            };

            await client.SendAsync(body);
        }

        public async Task SendMaintenanceRequestStatusUpdateAsync(
            string tenantUserId,
            string tenantEmail,
            string tenantPhoneNumber,
            string propertyAddress,
            string requestTitle,
            string requestStatus,
            string requestDescription)
        {
            var apiKey = _config["Pingram:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("Pingram API Key is missing.");

            var client = new PingramClient(apiKey, "https://api.pingram.io");

            var body = new SenderPostBody
            {
                Type = "maintenance_request_status_update",
                To = new SenderPostBodyTo
                {
                    Id = tenantUserId,
                    Email = tenantEmail,
                    Number = tenantPhoneNumber
                },
                Parameters = new Dictionary<string, object>
                {
                    { "propertyAddress", propertyAddress },
                    { "requestTitle", requestTitle },
                    { "requestStatus", requestStatus },
                    { "requestDescription", requestDescription }
                },
                TemplateId = "template_one"
            };

            await client.SendAsync(body);
        }

        public async Task SendApplicationSubmittedAsync(
            string landlordUserId,
            string landlordEmail,
            string landlordPhoneNumber,
            string propertyAddress,
            string applicantName,
            string applicantEmail,
            string applicantPhone,
            string moveInDate
        )
        {
            var apiKey = _config["Pingram:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("Pingram API Key is missing.");

            var client = new PingramClient(apiKey, "https://api.pingram.io");

            var body = new SenderPostBody
            {
                Type = "application_submitted",
                To = new SenderPostBodyTo
                {
                    Id = landlordUserId,
                    Email = landlordEmail,
                    Number = landlordPhoneNumber
                },
                Parameters = new Dictionary<string, object>
                {
                    { "propertyAddress", propertyAddress },
                    { "applicantName", applicantName },
                    { "applicantEmail", applicantEmail },
                    { "applicantPhone", applicantPhone },
                    { "moveInDate", moveInDate }
                },
                TemplateId = "template_one"
            };

            await client.SendAsync(body);
        }

        public async Task SendPaymentSubmittedAsync(
            string landlordUserId,
            string landlordEmail,
            string landlordPhoneNumber,
            string tenantName,
            string propertyAddress,
            string amountPaid,
            string paymentDate
        )
        {
            var apiKey = _config["Pingram:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("Pingram API Key is missing.");

            var client = new PingramClient(apiKey, "https://api.pingram.io");

            var body = new SenderPostBody
            {
                Type = "payment_submitted",
                To = new SenderPostBodyTo
                {
                    Id = landlordUserId,
                    Email = landlordEmail,
                    Number = landlordPhoneNumber
                },
                Parameters = new Dictionary<string, object>
                {
                    { "tenantName", tenantName },
                    { "propertyAddress", propertyAddress },
                    { "amountPaid", amountPaid },
                    { "paymentDate", paymentDate }
                },
                TemplateId = "template_one"
            };

            await client.SendAsync(body);
        }

        public async Task SendTenantDocumentUploadedAsync(
            string landlordUserId,
            string landlordEmail,
            string landlordPhoneNumber,
            string tenantName,
            string propertyAddress,
            string documentType,
            string fileName
        )
        {
            var apiKey = _config["Pingram:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("Pingram API Key is missing.");

            var client = new PingramClient(apiKey, "https://api.pingram.io");

            var body = new SenderPostBody
            {
                Type = "tenant_document_uploaded",
                To = new SenderPostBodyTo
                {
                    Id = landlordUserId,
                    Email = landlordEmail,
                    Number = landlordPhoneNumber
                },
                Parameters = new Dictionary<string, object>
                {
                    { "tenantName", tenantName },
                    { "propertyAddress", propertyAddress },
                    { "documentType", documentType },
                    { "fileName", fileName }
                },
                TemplateId = "template_one"
            };

            await client.SendAsync(body);
        }
    }
}
