using System.ComponentModel.DataAnnotations;

namespace ClincManagement.API.Contracts.Payment.Requests;

public class CreatePaymentRequest
{
    [Required]
    [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; init; }

    [Required]
    public PaymentMethod Method { get; init; }

    public string? Notes { get; init; }
}
