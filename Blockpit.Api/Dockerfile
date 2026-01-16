FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Blockpit.Api/Blockpit.Api.csproj", "Blockpit.Api/"]
COPY ["Blockpit.Mediator/Blockpit.Mediator.csproj", "Blockpit.Mediator/"]
COPY ["Blockpit.Data/Blockpit.Data.csproj", "Blockpit.Data/"]
COPY ["Blockpit.Query/Blockpit.Query.csproj", "Blockpit.Data/"]
COPY ["Blockpit.Observability/Blockpit.Observability.csproj", "Blockpit.Observability/"]
COPY ["Blockpit.Listener/Blockpit.Listener.csproj", "Blockpit.Listener/"]
COPY ["Blockpit.Migrations/Blockpit.Migrations.csproj", "Blockpit.Migrations/"]
RUN dotnet restore "Blockpit.Api/Blockpit.Api.csproj"
COPY . .
WORKDIR "/src/Blockpit.Api"
RUN dotnet build "./Blockpit.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Blockpit.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blockpit.Api.dll"]
