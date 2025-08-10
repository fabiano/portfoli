# portfoli

[![Continuous integration](https://github.com/fabiano/portfoli/actions/workflows/continuous-integration.yml/badge.svg)](https://github.com/fabiano/portfoli/actions/workflows/continuous-integration.yml)

## Working on the project

- Install .NET SDK 9
  - `winget install --exact --id Microsoft.DotNet.SDK.9`
  - `./dotnet-install.sh --version 9.0.304`
- Install Code recommended extensions
  - `code --install-extension editorconfig.editorconfig`
  - `code --install-extension ms-dotnettools.csdevkit`
  - `code --install-extension humao.rest-client`
- Trust ASP.NET HTTPS certificate
  - https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-9.0&tabs=visual-studio%2Clinux-sles#trust-the-aspnet-core-https-development-certificate
- Restore the dependencies: `dotnet restore`
- Build the project: `dotnet build`
- (Optional) Start the `Portfoli.AppHost` project with `dotnet watch run`
- (Optional) Open https://localhost:16001
