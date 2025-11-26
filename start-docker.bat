@echo off
echo Verificando Docker...
docker --version
if %errorlevel% neq 0 (
    echo ERROR: Docker no está instalado o no está en el PATH
    echo Por favor instala Docker Desktop desde: https://www.docker.com/products/docker-desktop
    pause
    exit /b 1
)

echo Verificando Docker Compose...
docker-compose --version
if %errorlevel% neq 0 (
    echo ERROR: Docker Compose no está disponible
    echo Asegúrate de que Docker Desktop esté corriendo
    pause
    exit /b 1
)

echo Docker está funcionando correctamente.
echo.

if not exist .env (
    echo Copiando archivo de configuración...
    copy .env.example .env
    echo.
    echo AVISO: Se creó .env con valores por defecto.
    echo Edita .env con tus configuraciones antes de continuar.
    echo Presiona cualquier tecla para continuar...
    pause
)

echo Iniciando servicios...
docker-compose up --build

pause