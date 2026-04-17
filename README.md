# -A-Blind-Project-Approval-System

A C# ASP.NET Core backend system for blind project approval, with SQL Server database.

## Features

- User authentication (register/login)
- Project submission
- Blind reviews by reviewers
- Admin approval/rejection

## Setup

### Backend (C#)
1. Navigate to backend-csharp: `cd backend-csharp`
2. Restore packages: `dotnet restore`
3. Update `appsettings.json` with your SQL Server connection string and JWT settings.
4. Run migrations: `dotnet ef migrations add InitialCreate`
5. Apply the database: `dotnet ef database update`
6. Run the server: `dotnet run`

## API Endpoints

- POST /api/auth/register
- POST /api/auth/login
- GET /api/projects (reviewers/admins)
- GET /api/projects/my (own projects)
- POST /api/projects (submit project)
- PUT /api/projects/:id (update status, admin)
- POST /api/reviews (submit review)
- GET /api/reviews/project/:id (get reviews)