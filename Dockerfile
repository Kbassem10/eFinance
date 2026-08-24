# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files for layer caching
COPY ["StudentRegistrationPortal.sln", "./"]
COPY ["src/StudentRegistrationPortal.Domain/StudentRegistrationPortal.Domain.csproj", "src/StudentRegistrationPortal.Domain/"]
COPY ["src/StudentRegistrationPortal.Application/StudentRegistrationPortal.Application.csproj", "src/StudentRegistrationPortal.Application/"]
COPY ["src/StudentRegistrationPortal.Infrastructure/StudentRegistrationPortal.Infrastructure.csproj", "src/StudentRegistrationPortal.Infrastructure/"]
COPY ["src/StudentRegistrationPortal.Api/StudentRegistrationPortal.Api.csproj", "src/StudentRegistrationPortal.Api/"]

# Restore dependencies
RUN dotnet restore "StudentRegistrationPortal.sln"

# Copy full source tree and publish API in Release mode
COPY . .
RUN dotnet publish "src/StudentRegistrationPortal.Api/StudentRegistrationPortal.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StudentRegistrationPortal.Api.dll"]
