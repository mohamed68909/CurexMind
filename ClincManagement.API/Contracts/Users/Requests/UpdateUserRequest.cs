
public record UpdateUserRequest(
    string FullName,
    string Email,
    IList<string> Roles
);
