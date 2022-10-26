# Build
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /build
COPY . .
RUN dotnet restore "./src/DataHub.WebApi/DataHub.WebApi.csproj"
RUN dotnet publish "./src/DataHub.WebApi/DataHub.WebApi.csproj" -c Release -o /app --no-restore

# Serve
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
ENV ASPNETCORE_URLS=http://+:5000
WORKDIR /app
COPY --from=build /app ./

EXPOSE 5000

ENTRYPOINT ["dotnet", "DataHub.WebApi.dll"]