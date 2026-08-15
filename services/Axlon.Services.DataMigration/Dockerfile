FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore services/Axlon.Services.DataMigration/Axlon.Services.DataMigration.csproj --configfile nuget.config
RUN dotnet publish services/Axlon.Services.DataMigration/Axlon.Services.DataMigration.csproj -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Axlon.Services.DataMigration.dll"]
