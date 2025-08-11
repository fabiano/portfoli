# portfoli

[![Continuous integration](https://github.com/fabiano/portfoli/actions/workflows/continuous-integration.yml/badge.svg)](https://github.com/fabiano/portfoli/actions/workflows/continuous-integration.yml)

## Working on the project

- Install .NET SDK 9
  - `winget install --exact --id Microsoft.DotNet.SDK.9`
  - `curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --version 9.0.304`
- Install Aspire CLI: `dotnet tool install -g Aspire.Cli --prerelease`
- [Trust ASP.NET HTTPS certificate](https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-9.0&tabs=visual-studio%2Clinux-sles#trust-the-aspnet-core-https-development-certificate)
- Restore dependencies with `dotnet restore`
- Build with `dotnet build`
- (Optional) Start the project with `aspire run`
- (Optional) Open https://localhost:16001
