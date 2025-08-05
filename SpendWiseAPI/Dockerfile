# Use .NET 9.0 Preview SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build

WORKDIR /app

# Copy project files
COPY SpendWiseAPI/*.csproj ./SpendWiseAPI/
WORKDIR /app/SpendWiseAPI
RUN dotnet restore

# Copy rest of the code
WORKDIR /app
COPY . .

# Build and publish
WORKDIR /app/SpendWiseAPI
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SpendWiseAPI.dll"]
