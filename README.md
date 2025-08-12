# portfoli

[![Continuous integration](https://github.com/fabiano/portfoli/actions/workflows/continuous-integration.yml/badge.svg)](https://github.com/fabiano/portfoli/actions/workflows/continuous-integration.yml)

## Working on the project

- Install .NET SDK 9: `curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --version 9.0.304`
- Trust certificates: `dotnet dev-certs https --trust`
- Restore dependencies: `dotnet restore`
- Run migrations: `sh run-migrations.sh`
- Start the project: `dotnet watch run`
