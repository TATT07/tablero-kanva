# Kanban Board Frontend

A modern, responsive Kanban board application built with Angular, featuring drag-and-drop functionality, user authentication, and task management capabilities.

## 🚀 Features

- **User Authentication**: Secure login with JWT tokens and refresh mechanism
- **Kanban Board**: Visual task management with drag-and-drop between columns (To Do, In Progress, Done)
- **Task Management**: Create, read, update, and delete tasks
- **Responsive Design**: Mobile-friendly interface using Angular Material
- **Real-time Updates**: Reactive UI updates with RxJS
- **Export Functionality**: Export tasks to PDF and Excel formats

## 🛠 Technology Stack

- **Framework**: Angular 20.3.0
- **UI Library**: Angular Material 20.2.14
- **Drag & Drop**: Angular CDK 20.2.14
- **Language**: TypeScript 5.9.2
- **State Management**: RxJS 7.8.0
- **Build Tool**: Angular CLI 20.3.8
- **Styling**: SCSS

## 📋 Prerequisites

Before running this project, make sure you have the following installed:

- **Node.js**: Version 18.0.0 or higher
- **npm**: Version 9.0.0 or higher (comes with Node.js)
- **Angular CLI**: `npm install -g @angular/cli`

## 🚀 Installation

1. **Clone the repository** (if not already done):
   ```bash
   git clone <repository-url>
   cd frontend
   ```

2. **Install dependencies**:
   ```bash
   npm install
   ```

3. **Configure environment** (optional):
   - Copy `src/environments/environment.ts` to `environment.prod.ts` if needed
   - Update API endpoints if the backend is running on a different port

## ⚙️ Configuration

The application uses environment files for configuration:

- `src/environments/environment.ts` - Development configuration
- `src/environments/environment.prod.ts` - Production configuration

Default API endpoint: `http://localhost:8080/api`

## 🏃 Running the Application

### Development Server

To start the development server:

```bash
npm start
# or
ng serve
```

The application will be available at `http://localhost:4200/`

The app will automatically reload when you make changes to the source files.

### Production Build

To build the application for production:

```bash
npm run build
# or
ng build --configuration production
```

The build artifacts will be stored in the `dist/` directory.

### Docker

To run with Docker:

```bash
docker build -t kanban-frontend .
docker run -p 80:80 kanban-frontend
```

## 📁 Project Structure

```
frontend/
├── src/
│   ├── app/
│   │   ├── components/
│   │   │   ├── dashboard/          # Main dashboard component
│   │   │   ├── kanban-board/       # Kanban board with drag & drop
│   │   │   └── login/              # Authentication component
│   │   ├── features/               # Feature modules
│   │   ├── guards/                 # Route guards (auth guard)
│   │   ├── interceptors/           # HTTP interceptors (auth interceptor)
│   │   ├── models/                 # TypeScript interfaces
│   │   │   ├── auth.model.ts       # Authentication models
│   │   │   └── task.model.ts       # Task models
│   │   ├── services/               # Angular services
│   │   │   ├── auth.service.ts     # Authentication service
│   │   │   └── task.service.ts     # Task management service
│   │   ├── app.config.ts           # Application configuration
│   │   ├── app.routes.ts           # Route definitions
│   │   └── app.ts                  # Main application component
│   ├── environments/               # Environment configurations
│   ├── index.html                  # Main HTML file
│   └── styles.scss                 # Global styles
├── public/                         # Static assets
├── angular.json                    # Angular CLI configuration
├── package.json                    # Dependencies and scripts
└── tsconfig.json                   # TypeScript configuration
```

## 🧪 Testing

### Unit Tests

To run unit tests:

```bash
npm test
# or
ng test
```

Tests are executed using Karma and Jasmine.

### End-to-End Tests

To run end-to-end tests:

```bash
npm run e2e
# or
ng e2e
```

## 🤝 Contributing

We welcome contributions! Please follow these guidelines:

1. **Fork the repository**
2. **Create a feature branch**: `git checkout -b feature/your-feature-name`
3. **Make your changes** and ensure tests pass
4. **Commit your changes**: `git commit -m 'Add some feature'`
5. **Push to the branch**: `git push origin feature/your-feature-name`
6. **Open a Pull Request**

### Code Style

- Use Angular CLI's default Prettier configuration
- Follow Angular style guide
- Use meaningful commit messages
- Write tests for new features

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

## 📞 Support

If you have any questions or need help, please open an issue in the repository.

## 🔗 Related Projects

- **Backend API**: [Kanban.API](../Kanban.API/README.md)
- **Architecture Documentation**: [architecture.md](../architecture.md)
