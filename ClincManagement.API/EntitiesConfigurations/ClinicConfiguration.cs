namespace ClincManagement.API.EntitiesConfigurations
{
    public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
    {
        public void Configure(EntityTypeBuilder<Clinic> builder)
        {
            // 🔸 Properties
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.Location)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(c => c.Description)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(c => c.CreatedDate)
                   .IsRequired()
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(c => c.IsActive)
                   .IsRequired();

           
            builder.HasMany(c => c.Appointments)
                   .WithOne(a => a.Clinic)
                   .HasForeignKey(a => a.ClinicId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(c => c.Reviews)
                   .WithOne(r => r.Clinic)
                   .HasForeignKey(r => r.ClinicId)
                   .OnDelete(DeleteBehavior.NoAction);

          
            builder.HasIndex(c => c.Name)
                   .IsUnique()
                   .HasDatabaseName("IX_ClinicName");

            builder.HasIndex(c => c.IsActive)
                   .HasDatabaseName("IX_ClinicIsActive");


            builder.HasData(

 new Clinic
 {
     Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
     Name = "Main Clinic",
     Description = "This is the main clinic located in the city center.",
     Location = "123 Main St, Cityville",
     IsActive = true,
     CreatedDate = new DateTime(2025, 9, 15),
 },


     new Clinic
       {
           Id = Guid.Parse("33333333-3333-3333-3333-333333333002"),
           Name = "Downtown Clinic",
           Description = "Downtown branch providing general medicine and pediatrics.",
           Location = "456 Downtown Ave, Cityville",
           IsActive = true,
           CreatedDate = new DateTime(2025, 9, 15),
       },
       new Clinic
       {
           Id = Guid.Parse("33333333-3333-3333-3333-333333333003"),
           Name = "Uptown Clinic",
           Description = "Uptown clinic specializing in cardiology and neurology.",
           Location = "789 Uptown Rd, Cityville",
           IsActive = true,
           CreatedDate = new DateTime(2025, 9, 15),
       },
       new Clinic
       {
           Id = Guid.Parse("33333333-3333-3333-3333-333333333004"),
           Name = "Eastside Clinic",
           Description = "Eastside clinic for dermatology and orthopedics.",
           Location = "321 Eastside Blvd, Cityville",
           IsActive = true,
           CreatedDate = new DateTime(2025, 9, 15),
       },
       new Clinic
       {
           Id = Guid.Parse("33333333-3333-3333-3333-333333333005"),
           Name = "West End Clinic",
           Description = "West End clinic offering gynecology and ENT services.",
           Location = "654 West End St, Cityville",
           IsActive = true,
           CreatedDate = new DateTime(2025, 9, 15),
       },
       new Clinic
       {
           Id = Guid.Parse("33333333-3333-3333-3333-333333333006"),
           Name = "Riverside Clinic",
           Description = "Riverside clinic focusing on ophthalmology and psychiatry.",
           Location = "987 Riverside Dr, Cityville",
           IsActive = true,
           CreatedDate = new DateTime(2025, 9, 15),
       },
       new Clinic
       {
           Id = Guid.Parse("33333333-3333-3333-3333-333333333007"),
           Name = "Greenfield Clinic",
           Description = "Greenfield clinic offering comprehensive medical care.",
           Location = "159 Greenfield Ln, Cityville",
           IsActive = true,
           CreatedDate = new DateTime(2025, 9, 15),
       }
   );

        }
    }
}
