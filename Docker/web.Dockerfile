ARG DOTNET_VER=9.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VER} AS build
WORKDIR /app
COPY OpenPassVault.sln .
COPY OpenPassVault.Web/ OpenPassVault.Web/
COPY OpenPassVault.Shared/ OpenPassVault.Shared/

WORKDIR /app/OpenPassVault.Web

RUN dotnet restore
RUN dotnet publish -c Release -o out --no-restore

FROM nginx:1.29.1
WORKDIR /app
EXPOSE 80
COPY Docker/nginx/default.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/OpenPassVault.Web/out/wwwroot /usr/share/nginx/html
