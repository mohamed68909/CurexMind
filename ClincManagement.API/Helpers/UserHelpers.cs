namespace ClincManagement.API.Helpers
{
    public class UserHelpers : IUserHelpers
    {
        public string GetUserName(string? email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                return string.Empty;
            return email.Split('@')[0];
        }
    }
}
