# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy .csproj and restore as distinct layers
COPY SpendWiseAPI/SpendWiseAPI.csproj SpendWiseAPI/
RUN dotnet restore SpendWiseAPI/SpendWiseAPI.csproj

# Copy the rest of the code
COPY . .

# Build and publish
RUN dotnet publish SpendWiseAPI/SpendWiseAPI.csproj -c Release -o /app/publish

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SpendWiseAPI.dll"]
