# Base image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7000

# SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy .csproj before restore to enhanc caching and not to download dependancies with each update to the app
# Copy solution file and project files
COPY . .

# Restore using the solution file
RUN dotnet restore Appointment_Management_System_Backend.sln

# Copy everything in the backend folder from this layer I can all the time update my image with the updates of the project 

# Publish only the API project
WORKDIR /src/src/Appointment_System.Presentation
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Appointment_System.Presentation.dll"]
