# Kanban Board Backend API

A robust REST API for a Kanban board application built with .NET, featuring JWT authentication, task management, and clean architecture principles.

## 🚀 Features

- **JWT Authentication**: Secure token-based authentication with refresh tokens
- **Role-Based Authorization**: Admin and User roles with appropriate permissions
- **Task Management**: Full CRUD operations for Kanban tasks
- **Drag & Drop Support**: Position-based task ordering for Kanban columns
- **Export Functionality**: Generate PDF and Excel reports of tasks
- **Clean Architecture**: Organized into Core, Application, Infrastructure, and API layers
- **PostgreSQL Database**: Relational data storage with Entity Framework Core
- **Docker Support**: Containerized deployment with multi-stage builds

## 🛠 Technology Stack

- **Framework**: .NET 10.0 Web API
- **Language**: C# 12.0
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL 15
- **Authentication**: JWT Bearer tokens
- **Documentation**: OpenAPI/Swagger
- **Containerization**: Docker with multi-stage builds
- **Password Hashing**: BCrypt
- **PDF Generation**: iText7
- **Excel Generation**: ClosedXML

## 📋 Prerequisites

Before running this project, ensure you have the following installed:

- **.NET SDK**: Version 10.0 or higher
- **PostgreSQL**: Version 15 or higher (or Docker)
- **Docker**: Optional, for containerized deployment
- **Docker Compose**: Optional, for full stack deployment

## 🚀 Installation

1. **Clone the repository** (if not already done):
   ```bash
   git clone <repository-url>
   cd Kanban.API
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Set up the database**:
   - Ensure PostgreSQL is running
   - Update connection string in `appsettings.json` if needed

4. **Run database migrations**:
   ```bash
   dotnet ef database update
   ```

## ⚙️ Configuration

### appsettings.json

The application uses `appsettings.json` for configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=KanbanDb;Username=postgres;Password=password"
  },
  "Jwt": {
    "Key": "your-secret-key-here-should-be-long-and-secure",
    "Issuer": "KanbanAPI",
    "Audience": "KanbanClient"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Environment Variables

For production deployment, use environment variables:

- `ConnectionStrings__DefaultConnection`: Database connection string
- `Jwt__Key`: JWT secret key (must be secure and long)
- `Jwt__Issuer`: JWT issuer
- `Jwt__Audience`: JWT audience
- `ASPNETCORE_ENVIRONMENT`: Environment (Development/Production)

## 🏃 Running the Application

### Development

To run the API in development mode:

```bash
dotnet run
```

The API will be available at `http://localhost:8080`

Swagger documentation: `http://localhost:8080/swagger`

### Docker

To run with Docker:

```bash
docker build -t kanban-api .
docker run -p 8080:8080 -e ConnectionStrings__DefaultConnection="..." kanban-api
```

### Docker Compose (Full Stack)

To run the entire application stack:

```bash
# From the root directory
docker-compose up
```

This will start:
- PostgreSQL database on port 5432
- Backend API on port 8080
- Frontend on port 80

## 📁 Project Structure

```
Kanban.API/                    # API Layer (Presentation)
├── Controllers/               # API Controllers
│   ├── AuthController.cs      # Authentication endpoints
│   └── TasksController.cs     # Task management endpoints
├── Program.cs                 # Application entry point
├── appsettings.json           # Configuration
└── Dockerfile                 # Docker configuration

Kanban.Application/            # Application Layer (Business Logic)
├── Services/
│   ├── AuthService.cs         # Authentication business logic
│   └── TaskService.cs         # Task management business logic
└── Kanban.Application.csproj

Kanban.Core/                   # Domain Layer (Entities & DTOs)
├── Entities/                  # Domain entities
│   ├── User.cs
│   ├── Role.cs
│   ├── Task.cs
│   ├── TaskStatus.cs
│   └── RefreshToken.cs
├── DTOs/                      # Data transfer objects
│   ├── LoginRequest.cs
│   ├── LoginResponse.cs
│   ├── TaskDto.cs
│   ├── CreateTaskRequest.cs
│   ├── UpdateTaskRequest.cs
│   └── MoveTaskRequest.cs
└── Kanban.Core.csproj

Kanban.Infrastructure/         # Infrastructure Layer (Data Access)
├── Persistence/
│   └── ApplicationDbContext.cs # EF Core database context
└── Kanban.Infrastructure.csproj
```

## 📚 API Documentation

### Authentication Endpoints

#### POST /api/auth/login
Authenticate user and return JWT tokens.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh-token-here",
  "expiresIn": 3600,
  "user": {
    "id": 1,
    "email": "user@example.com",
    "role": "User"
  }
}
```

#### POST /api/auth/refresh
Refresh JWT access token.

**Request Body:**
```json
{
  "refreshToken": "refresh-token-here"
}
```

### Task Management Endpoints

#### GET /api/tasks
Get all tasks for the authenticated user.

**Response:**
```json
[
  {
    "id": 1,
    "title": "Implement login feature",
    "description": "Create user authentication system",
    "status": "InProgress",
    "position": 0,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
]
```

#### POST /api/tasks
Create a new task.

**Request Body:**
```json
{
  "title": "New task title",
  "description": "Task description",
  "status": "ToDo"
}
```

#### PUT /api/tasks/{id}
Update an existing task.

**Request Body:**
```json
{
  "title": "Updated task title",
  "description": "Updated description",
  "status": "InProgress"
}
```

#### DELETE /api/tasks/{id}
Delete a task.

#### PUT /api/tasks/{id}/move
Move a task to a new position/status (drag & drop).

**Request Body:**
```json
{
  "newStatus": "Done",
  "newPosition": 1
}
```

### Export Endpoints

#### GET /api/tasks/export/pdf
Export tasks to PDF format.

**Query Parameters:**
- `userId` (optional): Filter by user ID

#### GET /api/tasks/export/excel
Export tasks to Excel format.

**Query Parameters:**
- `userId` (optional): Filter by user ID

## 🧪 Testing

### Running Tests

```bash
dotnet test
```

### API Testing

Use the included `.http` files for testing endpoints:

- `Kanban.API.http` - Contains sample requests for all endpoints

Or use Swagger UI at `/swagger` when running the application.

## 🤝 Contributing

We welcome contributions! Please follow these guidelines:

1. **Fork the repository**
2. **Create a feature branch**: `git checkout -b feature/your-feature-name`
3. **Make your changes** and ensure tests pass
4. **Commit your changes**: `git commit -m 'Add some feature'`
5. **Push to the branch**: `git push origin feature/your-feature-name`
6. **Open a Pull Request**

### Code Style

- Follow C# coding conventions
- Use meaningful variable and method names
- Write XML documentation for public APIs
- Include unit tests for new features

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

## 📞 Support

If you have any questions or need help, please open an issue in the repository.

## 🔗 Related Projects

- **Frontend Application**: [frontend](../frontend/README.md)
- **Architecture Documentation**: [architecture.md](../architecture.md)

## 👤 Default Admin User

For testing purposes, a default admin user is available:

- **Email**: doc_js_galindo@fesc.edu.co
- **Password**: 0123456789
- **Role**: Admin