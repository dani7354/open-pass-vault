ARG DOTNET_VER=10.0

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VER} AS base
EXPOSE 8443
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

# Install dependencies for SkiaSharp, see https://stackoverflow.com/questions/74063162/deploy-skiasharp-on-a-container-running-net-6-alpine-linux
RUN apt update && \
    apt install -y libfontconfig1 libice6 libsm6 && \
    apt clean

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenPassVault.API.dll"]
