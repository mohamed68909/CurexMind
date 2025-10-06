using ClincManagement.API.Abstractions.Consts;
using System.ComponentModel.DataAnnotations;

namespace ClincManagement.API.Settings
{
    public sealed class PaymentGatewaySettings
    {
        public const string SectionName = "Stripe";
        [Required]
        public string SecretKey { get; set; } = string.Empty;
        [Required]
        public string PublishableKey { get; set; } = string.Empty;
        [Required]
        public string WebhookSecret { get; set; } = string.Empty;
        [Required, RegularExpression(RegexPatterns.Currency)]
        public string Currency { get; set; } = "USD";


    }
}
