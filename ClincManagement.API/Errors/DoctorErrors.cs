using ClincManagement.API.Abstractions;

namespace ClincManagement.API.Errors
{
  
        public static class DoctorErrors
        {
            public static readonly Error NotFound = new(
                "Doctor.NotFound",
                "Doctor with the given ID was not found",
                404
            );
        }
    }


