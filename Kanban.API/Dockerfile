# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["../Kanban.API/Kanban.API.csproj", "Kanban.API/"]
COPY ["../Kanban.Application/Kanban.Application.csproj", "Kanban.Application/"]
COPY ["../Kanban.Core/Kanban.Core.csproj", "Kanban.Core/"]
COPY ["../Kanban.Infrastructure/Kanban.Infrastructure.csproj", "Kanban.Infrastructure/"]

RUN dotnet restore "Kanban.API/Kanban.API.csproj"

# Copy everything else and build
COPY .. .
WORKDIR "/src/Kanban.API"
RUN dotnet build "Kanban.API.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "Kanban.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose port 80
EXPOSE 80

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the app
ENTRYPOINT ["dotnet", "Kanban.API.dll"]
