# JT Desktop Agent — Cross-platform shell

This Avalonia/.NET 8 application is the macOS/Linux UI shell for JT Desktop Agent. It includes the JT-branded workspace, a local read-only file hash review, and a visible stop control.

## Current scope

- Native build targets: macOS x64/ARM64 and Linux x64/ARM64.
- File review reads the selected file locally and displays its SHA-256 hash and size.
- Microphone access is not requested.
- Desktop control, accessibility automation, process launching, window management, and background agent actions are deliberately disabled until their platform adapters and permission gates are implemented.

The stop control prevents further shell-side actions; it is not represented as a verified OS-wide kill switch because this shell currently has no privileged action queue.

## Build

```sh
dotnet restore cross-platform/JTDesktopAgent.CrossPlatform.csproj
dotnet build cross-platform/JTDesktopAgent.CrossPlatform.csproj -c Release
dotnet publish cross-platform/JTDesktopAgent.CrossPlatform.csproj -c Release -r osx-arm64 --self-contained true
```

CI builds native artifacts on macOS and Linux runners. Successful compilation and packaging do not constitute signing, notarization, installation testing, or full feature parity with the Windows WPF application.
