FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/API/RhSensoERP.API.csproj", "src/API/"]
COPY ["src/Shared/Core/RhSensoERP.Shared.Core.csproj", "src/Shared/Core/"]
COPY ["src/Shared/Application/RhSensoERP.Shared.Application.csproj", "src/Shared/Application/"]
COPY ["src/Shared/Infrastructure/RhSensoERP.Shared.Infrastructure.csproj", "src/Shared/Infrastructure/"]
COPY ["src/Identity/RhSensoERP.Identity.csproj", "src/Identity/"]
RUN dotnet restore "src/API/RhSensoERP.API.csproj"
COPY . .
WORKDIR "/src/src/API"
RUN dotnet build "RhSensoERP.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RhSensoERP.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RhSensoERP.API.dll"]
