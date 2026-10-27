# JT Desktop Agent — Platform Porting Plan

## Current verified scope

The native application in this repository is a Windows WPF/.NET application. Windows speech APIs, Microsoft Defender integration, and other Windows-specific calls cannot be treated as portable implementations. A successful Windows build does not prove macOS or Linux support.

## Recommended architecture

Keep the product identity **JT Desktop Agent** and the user-facing assistant name **Jeff AI**.

Separate the app into these layers:

1. **Shared agent core** — task plans, permission requests, action receipts, audit-event schema, cancellation and emergency-stop state.
2. **Platform adapters** — OS-specific process launch, accessibility, window management, speech, file operations and security-status integrations.
3. **Approval policy** — deny by default for consequential actions; display the target and effect before execution; never treat voice recognition as identity verification.
4. **UI shell** — consistent JT-branded task view, permission prompts, action history and stop control.
5. **Provider adapter** — optional Jeff AI connection through a documented interface; do not assume a backend is configured.

## Platform milestones

### Windows
- Keep the current WPF implementation as the reference platform.
- Add automated tests for permission decisions, path validation, cancellation and audit records.
- Verify that all file/process actions show a clear target and are recorded.

### macOS
- Implement a separate native or cross-platform UI and macOS adapter.
- Request Accessibility, Automation, microphone and file permissions only when needed.
- Document signing and notarization; never claim they are configured until verified.

### Linux
- Build a distinct adapter for supported desktop environments and display systems.
- Detect unsupported capabilities and explain the limitation instead of silently failing.
- Test packaging on each claimed architecture and document required system libraries.

## Security acceptance criteria

- Explicit approval for consequential actions.
- Least-privilege scopes and no hidden background control.
- Emergency stop cancels queued actions and blocks new ones.
- Auditable outcomes include success, failure, denial and cancellation.
- File actions validate canonical paths and handle symlinks deliberately.
- Secrets stay out of renderer/UI logs and repository history.
- Product documentation distinguishes heuristic checks from antivirus protection.

## Release gate

Do not advertise macOS or Linux as supported until a real platform implementation, automated checks, a native-platform build and a manual smoke test all pass. Do not publish unsigned packages as signed. Keep release artifacts in the product repository, not the company website repository.
