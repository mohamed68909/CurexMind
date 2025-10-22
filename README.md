🏥 Clinic Management APIClinic Management API is a RESTful Web API built with ASP.NET Core and Entity Framework Core designed to manage medical clinics efficiently — including appointments, patients, doctors, invoices, payments, and services.🚀 Key Features🧑‍⚕️ Manage doctors, patients, and appointments.💰 Create and track invoices and payments.🧾 Manage different service types and pricing.🔐 Secure authentication using ASP.NET Identity and JWT Tokens.🧭 Built-in API documentation via Swagger UI.⚙️ Clean layered architecture (Controllers / Services / Data / Entities / DTOs).🔄 Pagination support for large data sets.🧩 Uses Mapster for fast object mapping between entities and DTOs.🏗️ Project StructureClincManagement.API/
├── Controllers/              # REST API endpoints
├── Services/
│   ├── Interface/            # Service interfaces
│   └── Implementation/       # Business logic implementations
├── Entities/                 # Database entities
├── EntitiesConfigurations/   # EF Core Fluent API configurations
├── Data/                     # ApplicationDbContext
├── Contracts/                # DTOs for requests/responses
├── Errors/                   # Error handling and result wrappers
├── Program.cs                # Application entry point
├── appsettings.json          # Configuration and connection strings
└── README.md                 # Project documentation
🧰 Technologies UsedCategoryTechnologyLanguageC# (.NET 7 / .NET 8)FrameworkASP.NET Core Web APIORMEntity Framework CoreDatabaseSQL ServerAuthenticationASP.NET Identity + JWTObject MappingMapsterDocumentationSwagger / SwashbuckleDependency InjectionBuilt-in DIDesign PatternsService Layer, DTOs, Result Wrapper⚙️ How to Run Locally1️⃣ Prerequisites.NET SDK 7.0 or laterSQL Server (local or remote)Visual Studio or Visual Studio CodeEF Core CLI tools installed2️⃣ Steps to Run# Clone the repository
git clone [https://github.com/mohamed68909/ClincManagement.git](https://github.com/mohamed68909/ClincManagement.git)

# Navigate to the API project
cd ClincManagement/ClincManagement.API

# Apply database migrations
dotnet ef database update

# Run the project
dotnet run
Then open your browser at:https://localhost:{port}/swagger
to explore and test all API endpoints.🧠 Design OverviewFluent API Configurations are defined in EntitiesConfigurations to set field lengths, keys, and relationships.Service Layer handles business logic and communicates with the database via ApplicationDbContext.DTOs (Data Transfer Objects) ensure data isolation between API models and database entities.Result is used for consistent success/error handling across all responses.Pagination is implemented for endpoints like appointments and invoices.Swagger UI is pre-configured for easy API documentation and testing.📦 Example EndpointsMethodEndpointDescriptionGET/api/appointments?page=1&pageSize=10Get paginated list of appointmentsPOST/api/appointmentsCreate a new appointmentPUT/api/appointments/{id}Update an appointmentDELETE/api/appointments/{id}Delete an appointmentGET/api/invoicesRetrieve all invoicesPOST/api/serviceTypesAdd a new service type🔒 AuthenticationThe API uses JWT (JSON Web Tokens) combined with ASP.NET Identity for secure authentication and authorization.Register or log in through /api/auth/register or /api/auth/login.Receive a JWT token in the response.Use the token in all subsequent requests by adding this header:Authorization: Bearer {your_token_here}🧩 Example Code Snippetpublic class AppointmentService : IAppointmentService
{
    private readonly ApplicationDbContext _context;

    public AppointmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedAppointmentResponse>> GetAllAppointmentsAsync(int page, int pageSize, CancellationToken cancel)
    {
        var query = _context.Appointments.AsNoTracking();
        var total = await query.CountAsync(cancel);

        var items = await query
            .OrderBy(a => a.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<AppointmentDto>() // Using Mapster
            .ToListAsync(cancel);

        return Result.Success(new PagedAppointmentResponse(total, items));
    }
}
🧾 Future Improvements🧠 Add a frontend (React or Angular).📱 Create a mobile version using .NET MAUI or Flutter.☁️ Deploy to Azure App Service or Render.🔍 Add structured logging using Serilog.🧪 Implement unit and integration tests (xUnit + Moq).💬 Add localization (multi-language support).📸 ScreenshotsA quick visual overview of the API's documentation via Swagger UI.Swagger UI - HomeThe main page lists all available controllers and authentication methods.!<img src="[https://raw.githubusercontent.com/mohamed68909/ClincManagement/main/assets/swagger-ui-home.png](https://raw.githubusercontent.com/mohamed68909/ClincManagement/main/assets/swagger-ui-home.png)" alt="Swagger UI Home Page" width="700"/>
Endpoint Example (Appointments)A detailed view of the Appointments controller, showing the data structure and operations.!<img src="[https://raw.githubusercontent.com/mohamed68909/ClincManagement/main/assets/swagger-ui-appointment-endpoint.png](https://raw.githubusercontent.com/mohamed68909/ClincManagement/main/assets/swagger-ui-appointment-endpoint.png)" alt="Appointments Endpoint Definition" width="700"/>
👨‍💻 AuthorMohamed Ashraf💼 Software Developer specializing in .NET and Web APIs🌐 GitHub Profile📧 Contact: [your.email@example.com]🪪 LicenseThis project is developed for educational and training purposes.Feel free to use or modify it for non-commercial projects.MIT License Copyright (c) 2025 Mohamed Ashraf !
