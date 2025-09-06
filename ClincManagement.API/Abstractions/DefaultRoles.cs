namespace ClincManagement.API.Abstractions
{
    public static class DefaultRoles
    {
        public partial class Patient
        {
            public const string Name = nameof(Patient);
            public const string Id = "4D447E8A-B35A-4DAE-BCE3-4552BF828693";
            public const string ConcurrencyStamp = "E9FD0D85-6770-4A99-B3A2-69158B9EF3D7";
        }
        public partial class Admin
        {
            public const string Name = nameof(Admin);
            public const string Id = "8757DDE1-DA74-4A92-9EEB-46C4A35AC090";
            public const string ConcurrencyStamp = "F167EA47-FC22-4A47-81F9-1E21C11DB217";
        }
    }
}
