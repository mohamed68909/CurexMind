namespace ClincManagement.API.Errors
{
    public record UserErrors
    {
        public static readonly Error DublicatedEmail =
            new("Auth.DuplicatedEmail", "This email is already registered", StatusCodes.Status409Conflict);
        public static readonly Error InvalidCredentials =
            new("Auth.InvalidCredentials", "Invalid email or password", StatusCodes.Status401Unauthorized);
        public static readonly Error UserDisabled =
            new("Auth.UserDisabled", "This account has been disabled", StatusCodes.Status403Forbidden);
        public static readonly Error UserLockedOut =
            new("Auth.UserLockedOut", "This account is locked due to multiple failed login attempts. Please try again later.", StatusCodes.Status423Locked);
        public static readonly Error InvalidGoogleToken =
            new("Auth.InvalidGoogleToken", "The provided Google ID token is invalid or has expired.", StatusCodes.Status401Unauthorized);
        public static readonly Error ExternalLoginFailed =
            new("Auth.ExternalLoginFailed", "Failed to associate external login (Google) with the user.", StatusCodes.Status422UnprocessableEntity);
        public static readonly Error UserCreationFailed =
            new("Auth.UserCreationFailed", "Unable to create the user account due to a processing error.", StatusCodes.Status422UnprocessableEntity);


    }

}
