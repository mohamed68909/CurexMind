namespace ClincManagement.API.Contracts.Review.Respones
{
    public record AddReviewResponse
    {
        public string Message { get; init; } = "تم إضافة التقييم بنجاح";
        public Guid ReviewId { get; init; }
    }

}
