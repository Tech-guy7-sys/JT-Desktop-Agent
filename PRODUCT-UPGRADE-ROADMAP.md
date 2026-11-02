# JT Desktop Agent Product Roadmap

This roadmap separates shipped behavior from planned work. A roadmap is not evidence that a feature is implemented.

## Companion and voice
- First-run wizard with separate, optional permissions for microphone, startup, notifications, file access, accessibility automation and connectors.
- Background tray mode only after explicit opt-in. Microphone and wake phrase remain off until enabled; show a persistent listening indicator and pause/quit controls.
- User-selected wake phrase using local detection where available. Wake detection is not identity verification.
- Compact companion opens near the middle-right edge, can be dragged or repositioned by command, and hides after ten minutes idle while the opted-in background service remains available.
- Never conceal active microphone capture. Sensitive actions require ordinary approval, not voice alone.

## File handoff
- Explorer action, drag/drop and file picker: “Ask Jeff to work on this file”.
- Show the selected path and requested task. Start read-only; present proposed edits/diffs and ask before overwriting, deleting, moving, uploading or executing.
- Keep a user-visible activity record of plans, files accessed, approvals and outcomes.

## Application sections
Home/Companion; Capabilities with diagrams; Activity/Workflow; Permissions; Voice & Dictation; Files; JT Apps; Connectors; Settings. Dictation must target a user-selected field and offer review before insertion.

## JT app handoffs
JT Desktop Agent is the only app with desktop/device-control authority. Jeff AI and JT AI Agent are AI workspaces. JT Code, JT Design, JT Studio, JT Motion, JT Cloud, JT App Manager and JT Stream own their specialist workflows. Cross-app actions use versioned APIs and explicit app-to-app scopes.

## Connectors
Build a permissioned connector framework around documented OAuth/OIDC, MCP, REST/OpenAPI, webhooks and local app bridges. Show publisher, scopes, data handling and revoke controls. Do not promise every chatbot connector until implemented and tested. Do not copy proprietary Wispr Flow or Siri implementations; use documented OS/service interfaces where available.

## Install, update and uninstall
Provide branded platform-specific installers and uninstallers. First launch opens onboarding. Uninstall feedback is optional, consent-based, and never blocks removal. Show exactly what feedback is sent; exclude file contents, credentials and raw activity logs. A private maintainer review process may recommend fixes, but no automatic production edits or releases. A maintainer must approve code, tests, release notes and publication.

## Current status
Existing Windows WPF app: speech recognition/synthesis with manual start/stop, companion window, Defender status and confirmed scan, limited diagnostics, basic file triage. Not yet implemented: wake phrase, background tray lifecycle, onboarding permissions, idle overlay timeout, Explorer handoff, AI file execution, dictation insertion, activity timeline, capability gallery, cross-app bridges, generic connector catalogue, branded uninstall feedback and private maintainer workflow.

Current project targets .NET 8 WPF on Windows. macOS/Linux require native ports and separate validation.
