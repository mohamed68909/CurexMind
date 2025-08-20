namespace ClincManagement.API.Contracts.Authentications.Requests
{
    public record SignUpRequest
  (
     string FullName,
        string Email,
        string UserName,
        string Password,
        string ConfirmPassword,
        string PhoneNumber


        );
}
