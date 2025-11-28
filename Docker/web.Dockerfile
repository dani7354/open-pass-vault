ARG DOTNET_VER=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VER} AS build
WORKDIR /app
COPY OpenPassVault.sln .
COPY OpenPassVault.Web/ OpenPassVault.Web/
COPY OpenPassVault.Shared/ OpenPassVault.Shared/

WORKDIR /app/OpenPassVault.Web

RUN dotnet restore
RUN dotnet publish -c Release -o out --no-restore

FROM nginx:1.29.3
WORKDIR /app
EXPOSE 443
EXPOSE 80

RUN mkdir /etc/nginx/tls # Mounted from host

COPY Docker/nginx/nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/OpenPassVault.Web/out/wwwroot /usr/share/nginx/html
