# Orange HR Management System

A modern, full-stack HR management system built with .NET 8 and Blazor Server, featuring role-based access control, leave request workflows, and team management.

## 🚀 Live Demo

**Coming Soon** - Deploy to Railway in 5 minutes! See [QUICKSTART.md](./QUICKSTART.md)

## Features

- **Employee Management**: Comprehensive employee profiles with role-based access
- **Leave Request Workflow**: Two-step approval process (Manager → HR)
- **Team Management**: Organize employees into teams with manager hierarchy
- **Dashboard Analytics**: Real-time statistics and quick actions
- **Role-Based Access Control**: Three user roles (Admin/HR, Manager, Employee)
- **Modern UI**: Clean, responsive design with professional blue/white color scheme

## Technology Stack

### Backend
- .NET 8 (ASP.NET Core)
- C# 12
- Entity Framework Core
- SQL Server LocalDB
- ASP.NET Core Identity

### Frontend
- Blazor Server
- Bootstrap 5
- Custom CSS theme

### Architecture
- Clean Architecture
- Repository Pattern
- Service Layer
- Dependency Injection

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server LocalDB
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository
```bash
git clone https://github.com/yourusername/orange-hr-management.git
cd orange-hr-management
```

2. Restore dependencies
```bash
dotnet restore
```

3. Update database connection string (if needed)
Edit `src/HRManagement.BlazorUI/appsettings.json`

4. Apply database migrations
```bash
dotnet ef database update --project src/HRManagement.Infrastructure
```

5. Run the application
```bash
dotnet run --project src/HRManagement.BlazorUI
```

6. Open browser and navigate to `http://localhost:5007`

### Test Accounts

The system comes with pre-seeded test accounts:

**HR Admin**
- Email: sarah.chen@orange.com
- Password: Orange123!

**Manager**
- Email: michael.rodriguez@orange.com
- Password: Orange123!

**Employee**
- Email: emily.martinez@orange.com
- Password: Orange123!

## Project Structure

```
/
├── src/
│   ├── HRManagement.Domain/          # Core entities and interfaces
│   ├── HRManagement.Application/     # Business logic and services
│   ├── HRManagement.Infrastructure/  # Data access and EF Core
│   ├── HRManagement.BlazorUI/        # Blazor Server UI
│   └── HRManagement.Web/             # API controllers (legacy)
├── tests/
│   └── HRManagement.UnitTests/       # Unit tests
└── HRManagement.sln
```

## Key Features

### For Employees
- View personal profile and leave balance
- Submit leave requests
- Track request status and history

### For Managers
- Approve/reject team member leave requests
- View team overview and statistics
- Manage team member information

### For HR/Admin
- Full system access
- Final approval authority for leave requests
- Employee and team management
- System-wide analytics

## Database Schema

Key entities:
- **Employee**: User profiles with roles and leave balances
- **Team**: Team organization with manager assignments
- **LeaveRequest**: Leave request workflow with approval chain
- **LeaveBalance**: Annual leave balance tracking

## Development

### Build
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
```

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName --project src/HRManagement.Infrastructure

# Update database
dotnet ef database update --project src/HRManagement.Infrastructure
```

## Deployment

### Deploy to Railway

This project is configured for easy deployment to Railway.

1. **Push to GitHub**
```bash
git init
git add .
git commit -m "Initial commit"
git remote add origin https://github.com/yourusername/orange-hr-management.git
git push -u origin main
```

2. **Deploy on Railway**
   - Visit [railway.app](https://railway.app)
   - Click "Start a New Project"
   - Select "Deploy from GitHub repo"
   - Choose your repository
   - Railway will automatically detect the .NET project

3. **Add PostgreSQL Database**
   - In your Railway project, click "New"
   - Select "Database" → "Add PostgreSQL"
   - Railway will automatically set the `DATABASE_URL` environment variable

4. **Configure Environment Variables**
   Railway will automatically use the PostgreSQL connection string. No additional configuration needed!

5. **Access Your App**
   - Railway will provide a public URL (e.g., `https://your-app.up.railway.app`)
   - The app will automatically run migrations and seed data on first start

### Environment Variables

The following environment variables are automatically configured by Railway:
- `DATABASE_URL` - PostgreSQL connection string (auto-configured)
- `PORT` - Application port (auto-configured)

Optional variables you can set:
- `ASPNETCORE_ENVIRONMENT` - Set to `Production` (default)

## License

This project is licensed under the MIT License.

## Contact

For questions or support, please open an issue on GitHub.
