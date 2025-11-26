# System Architecture Design

## Overview
This document outlines the architecture for a Kanban board web application built with Angular frontend and .NET backend, following the requirements for Desarrollo Avanzado de Aplicaciones en Red.

## System Components

### Frontend (Angular)
- **Framework**: Angular 17+ with TypeScript
- **UI Library**: Angular Material for consistent design
- **Drag & Drop**: Angular CDK for Kanban functionality
- **State Management**: RxJS for reactive programming
- **HTTP Client**: Angular HttpClient with interceptors

### Backend (.NET)
- **Framework**: .NET 8 Web API
- **Language**: C#
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL
- **Authentication**: JWT with refresh tokens
- **Authorization**: Role-based access control

### Database
- **Type**: PostgreSQL
- **Migration Tool**: EF Core Migrations
- **Tables**: Users, Roles, Tasks

### Infrastructure
- **Containerization**: Docker with multi-stage builds
- **Orchestration**: Docker Compose for local development
- **Deployment**: CapRover on VPS

## Architecture Layers

### Backend Layers
1. **Presentation Layer** (Controllers)
   - API endpoints
   - Request/Response handling
   - Validation

2. **Application Layer** (Services)
   - Business logic
   - Orchestration
   - Validation rules

3. **Domain Layer** (Entities, DTOs)
   - Business entities
   - Data transfer objects
   - Domain models

4. **Infrastructure Layer** (Repositories)
   - Data access
   - Database operations
   - External services

### Frontend Layers
1. **Presentation Layer** (Components)
   - UI components
   - User interactions
   - Data binding

2. **Application Layer** (Services)
   - API communication
   - State management
   - Business logic

3. **Infrastructure Layer** (Interceptors, Guards)
   - HTTP interceptors
   - Route guards
   - Authentication

## Database Schema

### Users Table
```sql
CREATE TABLE Users (
    Id SERIAL PRIMARY KEY,
    Email VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    RoleId INT NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
```

### Roles Table
```sql
CREATE TABLE Roles (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(50) UNIQUE NOT NULL
);
```

### Tasks Table
```sql
CREATE TABLE Tasks (
    Id SERIAL PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    Status VARCHAR(20) NOT NULL CHECK (Status IN ('ToDo', 'InProgress', 'Done')),
    Position INT NOT NULL DEFAULT 0,
    UserId INT NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
```

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh JWT token

### Tasks (CRUD)
- `GET /api/tasks` - Get all tasks for authenticated user
- `POST /api/tasks` - Create new task
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task
- `PUT /api/tasks/{id}/move` - Move task (drag & drop)

### Exports
- `GET /api/tasks/export/pdf` - Export tasks to PDF
- `GET /api/tasks/export/excel` - Export tasks to Excel

## Frontend Structure

### Modules
- **AuthModule**: Login functionality
- **DashboardModule**: Main dashboard
- **KanbanModule**: Kanban board with drag & drop
- **SharedModule**: Common components and services

### Components
- LoginComponent
- DashboardComponent
- KanbanBoardComponent
- TaskCardComponent
- TaskFormComponent
- UserProfileComponent

### Services
- AuthService
- TaskService
- JwtService

### Guards
- AuthGuard

### Interceptors
- JwtInterceptor
- ErrorInterceptor

## Security
- JWT tokens with expiration
- Refresh token mechanism
- Role-based authorization (Admin, User)
- Route protection
- Password hashing with BCrypt

## Docker Configuration
- Multi-stage Dockerfile for Angular (build and nginx serve)
- Multi-stage Dockerfile for .NET (build and runtime)
- docker-compose.yml with PostgreSQL, backend, frontend services

## Deployment
- CapRover for container deployment
- Environment variables for configuration
- Database migrations on startup
- HTTPS configuration

## Mandatory User
- Email: doc_js_galindo@fesc.edu.co
- Password: 0123456789
- Role: Admin

This architecture ensures scalability, maintainability, and adherence to the course requirements.