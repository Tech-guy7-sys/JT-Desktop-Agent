# JT Desktop Agent — JT Desktop Agent

Standalone repository for the JT-branded Windows desktop agent.

## Build on Windows
Requirements: Windows 10/11 and the .NET 8 SDK.

```powershell
dotnet restore
dotnet run
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Current native application
- Windows speech recognition and speech synthesis with user-controlled start/stop.
- Editable transcript and spoken responses.
- Desktop companion window.
- Security Center that queries Microsoft Defender status, requests a scan after confirmation, and offers a fixed allowlist of read-only diagnostics.
- Program launch requests use a file picker and show the executable path before launch.
- Initial local file triage using SHA-256 and transparent heuristics.

## Security boundaries
JT Desktop Agent is **not a replacement for Microsoft Defender or another trusted antivirus**. The initial file triage is heuristic and can miss threats or report false positives. It does not provide full real-time protection, guaranteed malware detection, or automatic quarantine. Keep Windows security protection enabled.

Speech recognition does not verify speaker identity; voice input alone must not authorize sensitive actions. Some operations can be blocked by Windows policy or permissions.

## JT ecosystem integration
- Product: JT Desktop Agent
- Brand: JT
- AI-facing name: Jeff AI
- Desktop/device control belongs to JT Desktop Agent only and must use explicit approvals and least-privilege scopes.
- JT AI Core integration is a planned provider/API contract; this source does not claim an unconfigured backend is connected.

This repository holds the native WPF source. Build and test on Windows before treating a package as a verified release. Do not claim a package is signed unless signing is configured and verified.
