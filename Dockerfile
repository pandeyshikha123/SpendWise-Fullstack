# 1. Use .NET 9 SDK preview image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build

WORKDIR /src

# Copy .csproj and restore dependencies
COPY SpendWiseAPI/*.csproj ./SpendWiseAPI/
WORKDIR /src/SpendWiseAPI
RUN dotnet restore

# Copy rest of the source code
WORKDIR /src
COPY . .

# Publish the app
WORKDIR /src/SpendWiseAPI
RUN dotnet publish -c Release -o /app/publish

# 2. Use ASP.NET 9 preview runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS final

WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Run the application
ENTRYPOINT ["dotnet", "SpendWiseAPI.dll"]
