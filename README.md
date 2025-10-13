# OpenPassVault
![.NET test (xunit)](https://github.com/dani7354/open-pass-vault/actions/workflows/10-dotnet-test.yml/badge.svg)
![Docker Image Build and Push](https://github.com/dani7354/open-pass-vault/actions/workflows/30-build-docker-images.yml/badge.svg)

## Setup dev environment
TODO...
### Web API Certificates and TLS
Generate development certificates by running:

`$ dotnet dev-certs https -ep Docker/certs -p <export password>`

This certificate is used both on the host machine and inside the Docker container, as the directory is mounted. See `compose.yaml`.

### Web API .env
`OpenPassVault.API/.env` file for the web api. All environment variables are required. Most of them can be copied to the Docker version, which is located in `Docker/api.env`, but make sure MYSQL_SERVER is updated to "db" (default name of database container in `compose.yaml`).
```
MYSQL_SERVER=127.0.0.1
MYSQL_DATABASE=OpenPassVault
MYSQL_USER=
MYSQL_PASSWORD=

CORS_ALLOWED_ORIGINS=https://localhost
JWT_SIGNING_KEY=
JWT_AUDIENCE=
JWT_ISSUER=OpenPassVault
ASPNETCORE_URLS=https://+:8080
ASPNETCORE_Kestrel__Certificates__Default__Password=
ASPNETCORE_Kestrel__Certificates__Default__Path=Docker/certs/certificate.pfx
```

### MySQL .env
`Docker/db.env` for MySQL Docker container.
```
MYSQL_ROOT_PASSWORD=
MYSQL_DATABASE=OpenPassVault
MYSQL_USER=
MYSQL_PASSWORD=
TZ=Europe/Paris

```

### Nginx Certificates and TLS
The Nginx server serving the Blazor app (OpenPassVault.Web) needs certificates in order for TLS to function. Use the `dotnet dev-certs` command as described in a former section to generate a cert and run the following commands to convert the PKCS12 cert to PEM:

`$ openssl pkcs12 -in Docker/certs/certificate.pfx -out Docker/certs/certificate.pem -clcerts -nokeys`

`$ openssl pkcs12 -in Docker/certs/certificate.pfx -out Docker/certs/key.pem -nocerts -noenc`
