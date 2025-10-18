using ClincManagement.API.Abstractions;

namespace ClinicManagement.API.Errors
{
    public static class InvoiceErrors
    {

        public static readonly Error NotFound = new Error(
            code: "Invoice.NotFound",
            message: "Invoice not found or already deleted.",
            StatusCodes.Status404NotFound);

        public static readonly Error InvalidPatient = new Error(
            code: "Invoice.InvalidPatient",
            message: "The specified Patient ID is invalid or does not exist.",
            StatusCodes.Status400BadRequest);

        public static readonly Error InvalidDoctor = new Error(
            code: "Invoice.InvalidDoctor",
            message: "The specified Doctor ID is invalid or does not exist.",
            StatusCodes.Status400BadRequest);


        public static readonly Error InvalidAmount = new Error(
            code: "Invoice.InvalidAmount",
            message: "Financial amounts cannot be negative or incorrectly calculated.",
            StatusCodes.Status400BadRequest);


        public static readonly Error AlreadyPaid = new Error(
            code: "Invoice.AlreadyPaid",
            message: "Cannot modify a fully paid invoice.",
            StatusCodes.Status409Conflict);

        public static readonly Error ServiceMismatch = new Error(
            code: "Invoice.ServiceMismatch",
            message: "The selected service is not provided by the specified doctor.",
            StatusCodes.Status422UnprocessableEntity);


        public static readonly Error GeneralFailure = new Error(
            code: "Invoice.GeneralFailure",
            message: "An unexpected error occurred during invoice processing.",
            StatusCodes.Status500InternalServerError);
    }
}