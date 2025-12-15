using ClincManagement.API.Abstractions;

namespace ClincManagement.API.Errors
{
    public static class DashboardErrors
    {
        
        public static readonly Error FetchFailed = new(
            "Dashboard.FetchFailed",
            "An unexpected error occurred while fetching the dashboard summary due to a database or server issue.",
            500
        );

       
        public static readonly Error UserDataInaccessible = new(
            "Dashboard.UserDataInaccessible",
            "The necessary user data could not be accessed.",
            403 
        );

    }
}