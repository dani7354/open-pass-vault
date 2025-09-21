ARG DOTNET_VER=9.0

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VER} AS base
EXPOSE 8080
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VER} AS build
WORKDIR /src
COPY OpenPassVault.sln .
COPY OpenPassVault.API/ OpenPassVault.API/
COPY OpenPassVault.Shared/ OpenPassVault.Shared/
WORKDIR /src/OpenPassVault.API/
RUN dotnet restore

WORKDIR /src/OpenPassVault.API
RUN dotnet build OpenPassVault.API.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish OpenPassVault.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenPassVault.API.dll"]
