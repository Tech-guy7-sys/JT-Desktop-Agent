# JT Desktop Agent platform support

## Current implementation

The current native application uses .NET 8 WPF and Windows-specific speech, Defender, and desktop APIs. The supported native target is Windows. The CI workflow builds Windows x64 and x86 portable artifacts; the x86 build must pass CI before it is advertised as available.

## macOS and Linux

The existing WPF application cannot be compiled as a native macOS or Linux app by changing a build flag. Supporting those systems requires a cross-platform UI and platform-specific implementations for desktop control, accessibility, process launching, permissions, speech, and security diagnostics. Do not publish fake or empty installers for those platforms.

## Required future port

- Keep shared task planning and permission policy independent from OS-specific actions.
- Implement macOS and Linux control adapters behind explicit permission checks.
- Preserve user confirmation for sensitive actions, audit logs, and an emergency stop.
- Build on native runners, test the installed app, and publish platform packages only after the matching CI job passes.
- Keep the product display name **JT Desktop Agent** and the JT brand. The AI experience may be called **Jeff AI**; it is not a replacement name for the desktop agent.
