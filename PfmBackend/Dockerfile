# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
WORKDIR "/src/Pfm.Api"
RUN dotnet restore "Pfm.Api.csproj"
RUN dotnet build "Pfm.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Pfm.Api.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["auto-categorization-rules.yml", "/app/"]
EXPOSE 8080
ENTRYPOINT ["dotnet", "Pfm.Api.dll"]