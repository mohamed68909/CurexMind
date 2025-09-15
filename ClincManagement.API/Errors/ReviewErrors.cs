using ClincManagement.API.Abstractions;

namespace ClincManagement.API.Errors
{
    public static class ReviewErrors
    {
        public static readonly Error NotFound =
            new("Review.NotFound", "التقييم غير موجود", 404);

        public static readonly Error AlreadyReviewed =
            new("Review.AlreadyReviewed", "المريض أضاف تقييم بالفعل لهذا الطبيب", 400);

        public static readonly Error InvalidRating =
            new("Review.InvalidRating", "قيمة التقييم يجب أن تكون بين 1 و 5", 400);
    }
}
