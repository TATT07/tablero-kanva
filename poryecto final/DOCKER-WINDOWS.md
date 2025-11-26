# Guía Docker para Windows

## Problemas Comunes en Windows

### Error: "The system cannot find the file specified"
```
unable to get image: error during connect: Get "http://%2F%2F.%2Fpipe%2FdockerDesktopLinuxEngine/v1.51/images/...": open //./pipe/dockerDesktopLinuxEngine: The system cannot find the file specified.
```

**Solución**:
1. Abre Docker Desktop
2. Espera a que cargue completamente (puede tomar varios minutos)
3. Verifica que esté corriendo: busca el ícono de Docker en la barra de tareas
4. Si no funciona, reinicia Docker Desktop

### Error: "docker daemon is not running"
**Solución**:
1. Abre Docker Desktop como Administrador
2. Ve a Settings > General > Enable "Start Docker Desktop when you log in"
3. Reinicia tu computadora

## Pasos para Ejecutar

### 1. Verificar Docker
```cmd
# Ejecuta este archivo
start-docker.bat

# O verifica manualmente
docker --version
docker-compose --version
docker ps
```

### 2. Ejecutar la Aplicación
```cmd
# El archivo .env ya está configurado con valores por defecto
docker-compose up --build
```

### 3. Acceder
- **Frontend**: http://localhost
- **Backend**: http://localhost:5000
- **Base de datos**: localhost:5432

## Usuarios de Prueba
- **Admin**: `admin` / `admin123`
- **User**: `user` / `user123`

## Troubleshooting

### Puerto 4200 ocupado
```cmd
# Mata procesos en el puerto
netstat -ano | findstr :4200
taskkill /PID <PID> /F
```

### Limpiar Docker
```cmd
# Limpiar todo
docker system prune -a
docker volume prune
```

### Ver logs
```cmd
# Ver logs de servicios
docker-compose logs api
docker-compose logs frontend
docker-compose logs db
```

## Configuración Personalizada

Edita el archivo `.env` para cambiar:
- `JWT_KEY`: Clave secreta para tokens
- `DB_PASSWORD`: Contraseña de SQL Server
- `API_URL`: URL del backend (para desarrollo local)

## Soporte

Si tienes problemas con Docker en Windows:
1. Usa WSL2 en lugar de Hyper-V
2. Asegúrate de tener Windows 10/11 Pro o Enterprise
3. Verifica que virtualization esté habilitado en BIOS
4. Reinstala Docker Desktop si es necesario