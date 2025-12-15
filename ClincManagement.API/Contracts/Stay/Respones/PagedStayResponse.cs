
namespace ClincManagement.API.Contracts.Stay.Responses
{
    public class PagedStayResponse
    {
        public int TotalCount { get; set; }
        public List<StayListResponse> Stays { get; set; } = new();
    }
}
