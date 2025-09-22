using ClincManagement.API.Abstractions;

namespace ClincManagement.API.Errors
{
    public static class UserErrors
    {
        public static readonly Error DuplicatedEmail =
    new("Auth.DuplicatedEmail", "A user with this email already exists.", StatusCodes.Status409Conflict);

        public static readonly Error DuplicatePhoneNumber =
            new("Auth.DuplicatePhoneNumber", "This phone number is already registered", StatusCodes.Status409Conflict);

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

        public static readonly Error NotFound =
            new("User.NotFound", "The specified user was not found.", StatusCodes.Status404NotFound);

        public static readonly Error InvalidRole =
            new("User.InvalidRole", "User does not have the required role.", StatusCodes.Status403Forbidden);

        public static readonly Error InvalidToken =
            new("User.InvalidToken", "The provided token is invalid or expired.", StatusCodes.Status401Unauthorized);

        public static readonly Error RefreshTokenExpired =
            new("User.RefreshTokenExpired", "The refresh token has expired.", StatusCodes.Status401Unauthorized);

        public static readonly Error RefreshTokenAlreadyRevoked =
            new("User.RefreshTokenAlreadyRevoked", "The refresh token has already been revoked.", StatusCodes.Status400BadRequest);

        public static readonly Error UpdateFailed =
            new("User.UpdateFailed", "Failed to update user profile due to a processing error.", StatusCodes.Status422UnprocessableEntity);

        public static readonly Error PasswordTooWeak =
            new("User.PasswordTooWeak", "The provided password does not meet the required complexity rules.", StatusCodes.Status400BadRequest);

        public static readonly Error EmailNotConfirmed =
            new("User.EmailNotConfirmed", "You must confirm your email before logging in.", StatusCodes.Status403Forbidden);

        public static readonly Error AccessDenied =
            new("User.AccessDenied", "You do not have permission to access this resource.", StatusCodes.Status403Forbidden);

        public static readonly Error DeleteFailed =
            new("User.DeleteFailed", "Failed to delete the user account due to a processing error.", StatusCodes.Status422UnprocessableEntity);

        public static readonly Error PasswordChangeFailed =
            new("User.PasswordChangeFailed", "Failed to change the user password due to a processing error.", StatusCodes.Status422UnprocessableEntity);
    }
}
