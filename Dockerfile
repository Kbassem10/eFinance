# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies (optimizing Docker layer caching)
COPY ["StudentRegistrationPortal.Api.csproj", "./"]
RUN dotnet restore "StudentRegistrationPortal.Api.csproj"

# Copy entire source tree and publish Release build
COPY . .
RUN dotnet publish "StudentRegistrationPortal.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StudentRegistrationPortal.Api.dll"]
