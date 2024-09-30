# About

This project provides an authentication backend for a Dragalia Lost API server to interact with for the purposes of signing in users.

It also provides functionality for users to create a web-based account, to which they can upload a JSON savefile. Servers can retrieve this along with the last upload date to offer save import functionality.

## How to run

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Access to a PostgreSQL database. An example Docker setup is described below, which requires [Docker Desktop](https://www.docker.com/products/docker-desktop/) on Windows.

### Guide

Create a file called `appsettings.json` with the following structure:

```json
{
  "Jwt": {
    "PrivateKey": "<your private key>",
    "Issuer": "http://localhost:5002/",
    "Kid": "key id"
  },
  "ExpirationTimes": {
    "Id": 3600,
    "Access": 3600,
    "Session": 3600
  },
  "Patreon": {
    "ClientId": "placehoder",
    "ClientSecret": "placehoder",
    "StateEncryptionKey": "placehoder",
    "StateHmacKey": "placehoder"
  }
}
```

The Patreon configuration is not used during the normal operations of the application.

You can generate a private key using `openssl genrsa` on a command-line or a number of free websites. Then Base64 encode the entire file, including the `BEGIN`/`END PRIVATE KEY` markers. Set this as the `$.Jwt.PrivateKey` property.

Next, you must provide the app with the details of a PostgreSQL database to connect to. You can start one in a Docker container using the following command:

```bash
docker run --rm --name baas-postgres -e POSTGRES_PASSWORD=mysecretpassword -p 5432:5432 -d postgres:15
```

> [!NOTE]  
> The above command will create a database with no persistent storage -- i.e. all data will be lost when the container shuts down. If you want to persist data, look into using Docker Compose with a Docker volume. See the [How to Use the Postgres Docker Official Image](https://www.docker.com/blog/how-to-use-the-postgres-docker-official-image/) guide for more information.

Once started, set the following environment variables according to your setup:

- `BAASDB_HOST` (required)
- `BAASDB_USERNAME` (required)
- `BAASDB_PASSWORD` (required)
- `BAASDB_NAME` (defaults to `baas`)
- `BAASDB_PORT` (defaults to `5432`)

If using the Docker command above, set the variables as follows:

- `BAASDB_HOST`: `localhost`
- `BAASDB_USERNAME`: `postgres`
- `BAASDB_PASSWORD`: `mysecretpassword`
- `BAASDB_NAME`: `postgres`

Then, start the app, either using the CLI:

```
dotnet run --project .\DragaliaBaasServer\DragaliaBaasServer.csproj
```

or using your favourite IDE such as JetBrains Rider, Visual Studio, or Visual Studio Code.
