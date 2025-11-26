# Kanban Board Application

Una aplicación completa de tablero Kanban con autenticación multiusuario, desarrollada con .NET 8 (backend) y Angular 17 (frontend).

## Características

- ✅ **Autenticación JWT** con login/registro
- ✅ **Sistema multiusuario** con roles (Admin/User)
- ✅ **Gestión completa de tareas** (CRUD)
- ✅ **Estados de tarea** (Por Hacer, En Progreso, Completadas)
- ✅ **Filtros y búsqueda avanzada**
- ✅ **Interfaz moderna** con tema oscuro
- ✅ **Drag & Drop** para mover tareas
- ✅ **Historial de auditoría** básico
- ✅ **Notificaciones in-app**
- ✅ **Diseño responsive**
- ✅ **Dockerizado** para fácil despliegue

## Tecnologías

- **Backend**: .NET 8, C#, Entity Framework Core, PostgreSQL
- **Frontend**: Angular 17, TypeScript, Angular Material
- **Base de datos**: PostgreSQL (configurable)
- **Autenticación**: JWT Bearer Tokens
- **Contenedorización**: Docker & Docker Compose

## Inicio Rápido

### Prerrequisitos

- **Docker Desktop** (versión más reciente)
- Node.js 18+ (para desarrollo local)
- .NET 8 SDK (para desarrollo local)

### ⚠️ Usuarios de Windows

Si tienes problemas con Docker en Windows, consulta **[DOCKER-WINDOWS.md](DOCKER-WINDOWS.md)** para instrucciones específicas y solución de problemas comunes.

### 🚀 Inicio Rápido con Docker (Recomendado)

#### Windows
```cmd
# 1. Verifica que Docker esté corriendo
start-docker.bat

# O manualmente:
# Copia configuración
copy .env.example .env

# Ejecuta servicios
docker-compose up --build
```

#### Linux/Mac
```bash
# 1. Copia configuración
cp .env.example .env

# 2. Ejecuta servicios
docker-compose up --build
```

### 🔧 Verificación de Docker

Antes de ejecutar, asegúrate de que Docker esté funcionando:

```cmd
# Verifica Docker
docker --version

# Verifica Docker Compose
docker-compose --version

# Verifica que Docker Desktop esté corriendo
docker ps
```

### 📝 Configuración Inicial

1. **Edita `.env`** con tus valores:
   ```env
   JWT_KEY=tu-clave-jwt-super-secreta
   DB_PASSWORD=TuPasswordSeguro123!
   ```

2. **Accede a la aplicación**:
    - **Frontend**: http://localhost
    - **Backend API**: http://localhost:5000
    - **Base de datos PostgreSQL**: localhost:5432

### 👤 Usuarios de Prueba

La aplicación incluye usuarios de prueba:
- **Admin**: `admin@test.com` / `admin123`
- **User**: `user@test.com` / `admin123`

**Nota**: Regístrate con tu propio usuario para una experiencia completa.

### Desarrollo Local

1. **Backend**
   ```bash
   cd Kanban.API
   dotnet run
   ```

2. **Frontend**
   ```bash
   cd frontend
   npm install
   npm start
   ```

3. **Base de datos**
   - Usa SQL Server local o Docker
   - Actualiza `appsettings.json` con tu connection string

## Variables de Entorno

| Variable | Descripción | Default |
|----------|-------------|---------|
| `JWT_KEY` | Clave secreta para JWT | `your-super-secret-jwt-key` |
| `JWT_ISSUER` | Emisor del token JWT | `https://yourdomain.com` |
| `DB_PASSWORD` | Contraseña de SQL Server | `YourStrong!Passw0rd` |
| `DATABASE_CONNECTION` | Connection string completo | SQL Server local |
| `API_URL` | URL del backend API | `http://localhost:8080/api` |

## Estructura del Proyecto

```
├── Kanban.API/              # Backend .NET 8
│   ├── Controllers/         # Controladores REST API
│   ├── Properties/          # Configuración launch
│   └── Dockerfile           # Docker para backend
├── Kanban.Application/      # Lógica de negocio
│   ├── Services/           # Servicios de aplicación
│   └── DTOs/               # Objetos de transferencia
├── Kanban.Core/            # Entidades y interfaces
│   ├── Entities/           # Modelos de datos
│   └── DTOs/               # Contratos de datos
├── Kanban.Infrastructure/  # Capa de infraestructura
│   └── Persistence/        # Entity Framework
├── frontend/               # Frontend Angular
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/  # Componentes UI
│   │   │   ├── services/    # Servicios Angular
│   │   │   └── guards/      # Guards de ruta
│   │   └── environments/    # Configuración
│   ├── Dockerfile          # Docker para frontend
│   └── nginx.conf          # Config Nginx
├── docker-compose.yml      # Orquestación Docker
└── .env.example           # Variables de entorno
```

## API Endpoints

### Autenticación
- `POST /api/auth/login` - Login
- `POST /api/auth/register` - Registro
- `POST /api/auth/refresh` - Refresh token

### Tareas
- `GET /api/tasks` - Listar tareas (con filtros)
- `GET /api/tasks/{id}` - Obtener tarea por ID
- `POST /api/tasks` - Crear tarea
- `PUT /api/tasks/{id}` - Actualizar tarea
- `DELETE /api/tasks/{id}` - Eliminar tarea
- `PUT /api/tasks/{id}/move` - Mover tarea
- `GET /api/tasks/{id}/history` - Historial de tarea

## Despliegue en Producción

1. **Configura variables de entorno** en tu servidor
2. **Actualiza connection strings** para base de datos de producción
3. **Configura HTTPS** y certificados SSL
4. **Ejecuta** `docker-compose up -d`
5. **Configura** un reverse proxy (nginx) si es necesario

## Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.