# Use official .NET SDK image to build and run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and csproj
COPY COPY *.sln .
                         # Copy the solution file (from parent)
COPY *.csproj ./                        # Copy the .csproj file inside the backend folder

# Restore dependencies
RUN dotnet restore

# Copy the rest of the backend source code
COPY . .

# Publish the application
RUN dotnet publish -c Release -o /app/publish

# Final stage to run the app
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Appointment_Management_System_Backend.dll"]
