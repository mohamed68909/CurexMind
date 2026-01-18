using ClincManagement.API.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ClincManagement.API.Errors
{
    public static class PaymentErrors
    {
        public static readonly Error NotFound =
            new("Payment.NotFound", "Payment record not found.", StatusCodes.Status404NotFound);

        public static readonly Error AlreadyPaid =
            new("Payment.AlreadyPaid", "This appointment has already been paid.", StatusCodes.Status400BadRequest);

        public static readonly Error AlreadyCancelled =
            new("Payment.AlreadyCancelled", "Payment has already been cancelled.", StatusCodes.Status400BadRequest);

        public static readonly Error AlreadyProcessed =
            new("Payment.AlreadyProcessed", "The appointment payment was already processed and cannot be repeated.", StatusCodes.Status400BadRequest);

        public static readonly Error CancellationNotAllowed =
            new("Payment.CancellationNotAllowed", "Only pending payments can be cancelled.", StatusCodes.Status400BadRequest);

        public static readonly Error InvalidMethod =
            new("Payment.InvalidMethod", "The specified payment method is not supported.", StatusCodes.Status400BadRequest);

        public static readonly Error ConfirmationNotRequired =
            new("Payment.ConfirmationNotRequired", "This payment is already confirmed.", StatusCodes.Status400BadRequest);

        public static readonly Error ConfirmationNotSupported =
            new("Payment.ConfirmationNotSupported", "This payment method does not support confirmation.", StatusCodes.Status400BadRequest);

        public static readonly Error GatewayFailed =
            new("Payment.GatewayFailed", "External payment gateway failed to process the request.", StatusCodes.Status503ServiceUnavailable);

        public static Error GatewayFailure(string details) =>
            new(
                "Payment.GatewayFailed",
                $"External payment gateway failed. Details: {details}",
                StatusCodes.Status503ServiceUnavailable
            );
    }
}
