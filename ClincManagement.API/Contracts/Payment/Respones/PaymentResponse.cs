namespace ClincManagement.API.Contracts.Payment.Responses;



public class PaymentResponse
{
    public Guid Id { get; init; }
    public Guid AppointmentId { get; init; }
    public Guid? InvoiceId { get; init; }

    public string? ReferenceCode { get; init; }

    public PaymentMethod Method { get; init; }
    public PaymentStatus Status { get; init; }
    public AppointmentStatus AppointmentStatus { get; init; }

    public decimal Amount { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime? ConfirmedAt { get; init; }

    // UI Helpers
    public bool IsPaid => Status == PaymentStatus.Success;
    public bool IsConfirmed => ConfirmedAt.HasValue;
}

