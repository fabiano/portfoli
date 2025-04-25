# portfoli

[![Continuous integration](https://github.com/fabiano/portfoli/actions/workflows/continuous-integration.yml/badge.svg)](https://github.com/fabiano/portfoli/actions/workflows/continuous-integration.yml)

## Prerequisites

- [.NET SDK 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Visual Studio Code](https://code.visualstudio.com/)
- [EditorConfig for VS Code](https://marketplace.visualstudio.com/items?itemName=editorconfig.editorconfig)
- [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)

## Working on the project

- Install .NET SDK 9: `winget install --exact --id Microsoft.DotNet.SDK.9`
- Install EditorConfig extension: `code --install-extension editorconfig.editorconfig`
- Install C# Dev Kit extension: `code --install-extension ms-dotnettools.csdevkit`
- Restore the dependencies: `dotnet restore`
- Build the project: `dotnet build`
- (Optional) Start the project: `dotnet watch run`
- (Optional) Open http://localhost:5197
