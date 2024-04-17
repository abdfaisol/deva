# Use the .NET 8 SDK image as a base
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the container
WORKDIR /app

# Copy the project file(s) and restore dependencies
COPY ["deva/deva.csproj", "deva/"]
RUN dotnet restore "./deva/deva.csproj"

# Copy the remaining source code
COPY . .

# Build the application
RUN dotnet build -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Final stage / image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 4001
ENTRYPOINT ["dotnet", "deva.dll"]
