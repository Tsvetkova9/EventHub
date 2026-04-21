# EventHub

EventHub is a modern event management and ticket booking platform built with ASP.NET Core 8 MVC, Entity Framework Core, and Identity. It supports event discovery, ticket purchase, reviews, and an admin dashboard.

## Tech Stack
- ASP.NET Core 8 MVC
- Entity Framework Core 8
- SQL Server
- ASP.NET Identity
- Bootstrap 5
- xUnit, Moq (testing)

## Architecture
EventHub uses a clean N-Tier architecture:
- **EventHub.Web**: MVC UI, controllers, view models
- **EventHub.Core**: Entities, interfaces, business logic
- **EventHub.Infrastructure**: Data access, repositories, seeding
- **EventHub.Tests**: Unit tests (xUnit)

## How to Run
1. Clone the repo and open in Visual Studio or VS Code.
2. Update the connection string in `appsettings.json` if needed.
3. Run `dotnet ef database update` to create the database.
4. Run the app: `dotnet run --project EventHub.Web`
5. Log in with seed credentials:
   - **Admin:** admin@eventhub.com / Admin@123
   - **User:** user@eventhub.com / User@123

## Known Issues
- Image upload is simulated (URL only, no real file storage)
- Search is case-sensitive
- Some validation is missing on optional fields
- Admin area uses repeated layout code
- No email confirmation for registration
- Error pages are basic
- Some TODOs left in comments

---

> Project by Gergana Cvetkova, 2026. For coursework/demo use only.
