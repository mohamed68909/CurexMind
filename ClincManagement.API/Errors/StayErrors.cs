using ClincManagement.API.Abstractions;

namespace ClincManagement.API.Errors
{
    public static class StayErrors
    {
        public static readonly Error NotFound =
            new("Stay.NotFound", "Stay not found in the system.", StatusCodes.Status404NotFound);

        public static readonly Error AlreadyExists =
            new("Stay.AlreadyExists", "A stay for this patient already exists or conflicts with another stay.", StatusCodes.Status409Conflict);

        public static readonly Error InvalidPatient =
            new("Stay.InvalidPatient", "The specified patient does not exist in the system.", StatusCodes.Status404NotFound);

        public static readonly Error CreationFailed =
            new("Stay.CreationFailed", "Failed to create the new stay record.", StatusCodes.Status422UnprocessableEntity);

        public static readonly Error UpdateFailed =
            new("Stay.UpdateFailed", "Failed to update the stay information.", StatusCodes.Status422UnprocessableEntity);

        public static readonly Error DeletionFailed =
            new("Stay.DeletionFailed", "Failed to delete the stay record.", StatusCodes.Status422UnprocessableEntity);

        public static readonly Error InvalidStayType =
            new("Stay.InvalidStayType", "The specified stay type is not valid.", StatusCodes.Status400BadRequest);
    }
}