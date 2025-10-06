using ClincManagement.API.Abstractions;

namespace ClincManagement.API.Errors
{
    using Microsoft.AspNetCore.Http;

    namespace ClincManagement.API.Errors
    {
      
      

        public record PaymentErrors
        {
            
            public static readonly Error NotFound =
                new("Payment.NotFound", "Payment record not found.", StatusCodes.Status404NotFound);

            public static readonly Error IsPaid =
                new("Payment.AlreadyPaid", "Cannot cancel a payment that has already been completed.", StatusCodes.Status400BadRequest);

            public static readonly Error Cancelled =
                new("Payment.AlreadyCancelled", "Payment has already been cancelled.", StatusCodes.Status400BadRequest);

          
            public static readonly Error AlreadyProcessed =
                new("Payment.AlreadyProcessed", "The appointment is already paid or cancelled and cannot be processed again.", StatusCodes.Status400BadRequest);

            public static readonly Error CancellationNotAllowed =
                new("Payment.CancellationNotAllowed", "Cancellation is only allowed for pending or unconfirmed payments.", StatusCodes.Status400BadRequest);

           
            public static readonly Error InvalidMethod =
                new("Payment.InvalidMethod", "The specified payment method is not supported by the system.", StatusCodes.Status400BadRequest);

            public static readonly Error ConfirmationNotRequired =
                new("Payment.ConfirmationNotRequired", "This payment is already confirmed or does not require a second confirmation step (OTP).", StatusCodes.Status400BadRequest);

            public static readonly Error ConfirmationNotSupported =
                new("Payment.ConfirmationNotSupported", "The specified payment method does not support confirmation via this endpoint.", StatusCodes.Status400BadRequest);

            public static readonly Error GatewayFailed =
                new("Payment.GatewayFailed", "External payment gateway failed to process the request.", StatusCodes.Status503ServiceUnavailable);
        }
    }
}
