# JT Desktop Agent platform support

## Windows implementation

The primary Windows app remains the .NET 8 WPF application in the repository root. It uses Windows speech and security APIs. The Windows workflow builds and tests x64/x86 targets.

## macOS and Linux implementation

A separate Avalonia/.NET 8 desktop shell lives in `cross-platform/`. It is built on native GitHub Actions runners for:

- macOS x64 (`osx-x64`)
- macOS ARM64 (`osx-arm64`)
- Linux x64 (`linux-x64`)
- Linux ARM64 (`linux-arm64`)

The cross-platform shell currently provides the JT-branded workspace, local read-only file metadata/SHA-256 review, and a visible stop control. It does not request microphone access. Desktop automation, accessibility control, process launching, window management, speech, and security integrations remain disabled until the matching OS adapters and permission gates are implemented.

## Verification and release status

The macOS/Linux workflow must pass on its native runners before those build targets are described as CI-passing. A successful build is not proof of code signing, notarization, installation/runtime QA, or feature parity with Windows. Do not publish signed-build claims or full cross-platform feature claims until those checks are completed.

## Security requirements

- Keep task planning and approval policy separate from OS-specific actions.
- Require explicit approval for consequential actions.
- Preserve audit events and cancellation.
- Do not treat voice recognition as identity verification.
- Validate file paths and handle symlinks deliberately.
- Keep secrets out of UI logs and repository history.

Keep the product display name **JT Desktop Agent** and JT branding. The AI experience may be called **Jeff AI**; it does not replace the desktop agent's product name.
