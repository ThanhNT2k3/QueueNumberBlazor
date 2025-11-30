# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/QMS.Web/QMS.Web.csproj", "src/QMS.Web/"]
COPY ["src/QMS.Application/QMS.Application.csproj", "src/QMS.Application/"]
COPY ["src/QMS.Domain/QMS.Domain.csproj", "src/QMS.Domain/"]
COPY ["src/QMS.Infrastructure/QMS.Infrastructure.csproj", "src/QMS.Infrastructure/"]

RUN dotnet restore "src/QMS.Web/QMS.Web.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/src/QMS.Web"
RUN dotnet build "QMS.Web.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "QMS.Web.csproj" -c Release -o /app/publish

# Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose port 8080 (default for Render)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Create a directory for SQLite database
RUN mkdir -p /app/data
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/qms.db"

ENTRYPOINT ["dotnet", "QMS.Web.dll"]
