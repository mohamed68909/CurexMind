namespace ClincManagement.API.Errors
{
    using ClincManagement.API.Abstractions;

    public static class PatientErrors
    {
        public static readonly Error NotFound =
            new Error("Patient.NotFound", "Patient not found in the system");

        public static readonly Error NoAppointments =
            new Error("Patient.NoAppointments", "This patient has no appointments");
    }
}
