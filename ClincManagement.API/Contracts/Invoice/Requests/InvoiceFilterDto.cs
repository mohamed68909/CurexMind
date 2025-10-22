namespace ClincManagement.API.Contracts.Invoice.Requests
{
   
        public class InvoiceFilterDto
        {
            public string SearchQuery { get; set; }


            public DateTime? DateFrom { get; set; }


            public DateTime? DateTo { get; set; }


            public string PaymentStatus { get; set; }


            public string PaymentMethod { get; set; }

            public string DoctorId { get; set; }


            public string ServiceTypeId { get; set; }

            public int PageNumber { get; set; } = 1;


            public int PageSize { get; set; } = 20;


            public string SortBy { get; set; } = "InvoiceDate";

            public string SortDirection { get; set; } = "desc";
        }
    }

