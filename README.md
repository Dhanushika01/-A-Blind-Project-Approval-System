# -A-Blind-Project-Approval-System

A C# ASP.NET Core backend system for blind project approval, with SQL Server database.

## Features

- User authentication (register/login)
- Project submission
- Blind reviews by reviewers
- Admin approval/rejection

## Setup

### Backend (C#)
1. Open this folder in VS Code: `-A-Blind-Project-Approval-System`
2. Restore packages: `dotnet restore backend-csharp/BlindProjectApproval.csproj`
3. Update `backend-csharp/appsettings.json` with your SQL Server connection string and JWT settings.
4. Create the database migration: `dotnet ef migrations add InitialCreate --project backend-csharp/BlindProjectApproval.csproj`
5. Apply the database: `dotnet ef database update --project backend-csharp/BlindProjectApproval.csproj`
6. Run the server: `dotnet run --project backend-csharp/BlindProjectApproval.csproj`

### VS Code
- Use `Terminal > Run Task... > Run Backend`
- Or use the Debug panel and launch `Launch Backend`

## API Endpoints

- POST /api/auth/register
- POST /api/auth/login
- GET /api/projects (reviewers/admins)
- GET /api/projects/my (own projects)
- POST /api/projects (submit project)
- PUT /api/projects/:id (update status, admin)
- POST /api/reviews (submit review)
- GET /api/reviews/project/:id (get reviews)