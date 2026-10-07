# Library Management System

## Project Overview

A web-based Library Management System developed using ASP.NET Core MVC to
manage students, books, and library transactions. The system provides
student and book management, book issue and return operations, and a
dashboard with library statistics and analytics.

## Technologies Used

- ASP.NET Core MVC (.NET 8)
- C#
- Entity Framework Core
- SQL Server
- Razor Views
- Bootstrap
- HTML5
- CSS3
- JavaScript

## Features

- Student registration and student record management
- Book management with book details, quantity, and availability status
- Book issue management with issue and due date tracking
- Book return management with return date and fine calculation
- Student and book search functionality
- Record listing and management for students, books, issued books, and returned books
- Admin dashboard with key library statistics
- Library analytics and graphical data visualization
- CRUD operations with client-side and server-side validation

## Database

**Database:** `LibraryManagement`

The application uses SQL Server for data storage and Entity Framework Core
for database operations.

## Architecture

The application follows the **ASP.NET Core MVC architecture**:

- **Model** – Represents application data and database entities
- **View** – Provides the user interface using Razor Views
- **Controller** – Handles application requests and business flow

## How to Run

1. Clone the repository.
2. Open the solution in Visual Studio.
3. Restore the required NuGet packages.
4. Make sure SQL Server is installed and running.
5. Create or restore the `LibraryManagement` database.
6. Update the SQL Server connection string in `appsettings.json`.
7. Build and run the project using Visual Studio.

## Learning Outcomes

- ASP.NET Core MVC development
- C# programming
- CRUD operations
- Entity Framework Core
- SQL Server database integration
- Razor Views
- Form validation
- Database relationships
- Dashboard and data visualization
