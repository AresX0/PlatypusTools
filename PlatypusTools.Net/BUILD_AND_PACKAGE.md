# Build & Packaging

Requirements:
- Install .NET 10 SDK (https://dotnet.microsoft.com/en-us/download/dotnet/10.0) — ensure `dotnet --list-sdks` shows a 10.x entry
- For installer: WiX Toolset (https://wixtoolset.org) or MSIX packaging tools

Build:
- Ensure .NET 10 SDK is installed and the repo root contains `global.json` to pin SDK (optional)
- dotnet build .\PlatypusTools.sln

Run UI locally:
- dotnet run --project PlatypusTools.UI\PlatypusTools.UI.csproj

Publish single-file exe (win-x64):
- dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=false -o publish --self-contained true PlatypusTools.UI\PlatypusTools.UI.csproj

Create installer (recommended approach):
- Use WiX: create a WiX project that packages the published folder into an MSI.
- Or create MSIX using MSIX Packaging Tool (manual or CI).

Notes:
- We will port each PowerShell feature into the Core library under `Services` and then call from UI.
- Start with dry-run behaviors & unit tests to ensure safety for destructive operations.
