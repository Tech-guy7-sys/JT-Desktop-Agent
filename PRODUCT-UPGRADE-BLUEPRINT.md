# JT Desktop Agent — Product Upgrade Blueprint

This document is the implementation contract for the next JT Desktop Agent milestones. It distinguishes existing features from planned work; a specification is not proof that a feature has shipped.

## Product experience

JT Desktop Agent is the user's local desktop companion. The assistant identity shown to users is **Jeff AI**. The JT Desktop Agent is the only JT product allowed to control the desktop, supported devices, windows, keyboard/mouse automation, file operations, or approved terminal workflows.

### First-run experience
1. Launch a first-run setup wizard after installation. Explain each permission before requesting it.
2. Ask separately for microphone access, startup/background operation, notifications, file access, accessibility/UI automation, and optional connectors. Do not bundle consent.
3. Microphone and wake-word listening are OFF until explicitly enabled. Show a persistent tray/menu-bar listening indicator while active, with Pause and Quit controls.
4. Ask the user to choose a wake phrase. Do not assume that "Jeff" or another name is unique or reliably detected. Explain false activations and local/cloud processing choices.
5. After setup, show a short welcome from Jeff and a guided tour. Do not request administrator privileges unless a specific approved feature requires them.

### Wake phrase and companion overlay
- Background agent starts only after the user opts into launch-at-login/background operation.
- A local wake-word engine should detect the configured phrase; do not stream continuous microphone audio to a cloud model for wake detection.
- When detected, show a compact companion near the right edge, vertically around the middle, inset toward the screen centre.
- Support drag repositioning and voice/text commands such as "move to the top left". Persist position per display and recover safely when monitors change.
- Hide the companion after ten minutes without interaction; keep the tray/background process available only if the user enabled it.
- The companion must expose microphone state and a one-action pause/exit control. Never conceal active capture.
- Wake-word detection is not speaker authentication. Sensitive actions require normal UI approval or a stronger configured authentication factor.
- Windows secure desktop/UAC prompts and lock screen are outside ordinary overlay control.

### File handoff: "Jeff, please work on this file"
- Provide Explorer context-menu action "Ask Jeff to work on this file", plus drag/drop and file picker.
- Show exact file path, type, size, and requested task before reading or modifying it.
- Start in read-only analysis mode. Create edits as a proposed copy/diff; ask before overwriting, deleting, moving, uploading, or executing.
- Keep an action record: file selected, scope granted, analysis summary, proposed changes, approvals, result, and timestamps.
- Use per-task scoped permissions, not blanket access to the whole disk.

## Product sections in the application

1. **Home / Companion** — wake phrase, status, quick actions, recent tasks.
2. **Capabilities** — searchable capability catalogue with diagrams, examples, requirements, and permission labels.
3. **Activity / Workflow** — chronological task timeline showing plan, analysis, tools, files touched, approvals, diffs, and outcomes. Include stop/cancel.
4. **Permissions** — granular scopes, one-time vs persistent grants, expiry, revocation, and audit history.
5. **Voice & Dictation** — speech-to-text, push-to-talk, wake phrase, dictation target selection, punctuation commands, read-back, and stop. Dictation must insert text only into a user-selected field or app; never silently type into an arbitrary focused field.
6. **Files** — file handoff, supported formats, previews, diff/restore, and file history.
7. **JT Apps** — launch and hand off work to installed JT apps through documented app APIs and explicit permission grants.
8. **Connectors** — OAuth/MCP connectors, connected accounts, scopes, revoke, and per-connector action approvals.
9. **Settings** — startup, overlay position, wake phrase, timeout, privacy, model/provider, notifications, diagnostics.

## Voice and dictation interoperability

Offer an independent dictation layer with push-to-talk and optional wake phrase. Provide an explicit "Dictate into selected field" action and a clear target preview. Do not claim to embed or reproduce proprietary Wispr Flow or Siri implementations. Integrate supported operating-system dictation and third-party services only through documented, licensed interfaces and user-authorized connectors. Voice commands must not authorize sensitive system actions by themselves.

## JT ecosystem capabilities

- JT AI Agent: research, planning, writing, reasoning, data analysis, projects, specialist agents.
- Jeff AI: general AI workspace and provider/model selection; no direct desktop control.
- JT Code: coding workspace, repository context, tests, review, debugging.
- JT Design: design canvases/assets, brand kits, export and review.
- JT Studio: audio production workflows, project organization, recording/editing integrations.
- JT Motion: video/motion design workflows, timeline/assets/render queue.
- JT Cloud: user-controlled sync, files, sharing, backup and version history.
- JT App Manager: install/update/uninstall, app inventory, signatures and rollback.
- JT Stream: live production workspace; streaming destinations only after explicit setup.
- JT AI Core: shared provider abstraction, model routing, metering and policy contracts.

JT Desktop Agent can hand tasks to these apps through versioned local APIs. Each app owns its data and domain operations. Cross-app actions require explicit app-to-app scopes and a user-visible handoff record. Desktop control remains exclusive to JT Desktop Agent.

## Workflow and permission architecture

Use a local policy broker. Every tool call declares: requesting app, action, resource, requested scope, risk level, expiry, approval requirement, and audit event. Default deny. Require confirmation for external network writes, publishing, payments, account changes, destructive file actions, installs, elevated commands, and device-control actions. Provide an emergency stop that cancels queued work and disables automation.

## Install, update and uninstall experience

- Branded, signed installers and uninstallers should be built per supported OS and architecture.
- First launch opens onboarding; permissions are requested in context, not silently.
- Uninstall feedback is optional and must never block removal. Ask a short reason, explain what diagnostic data would be sent, show the exact payload, and require separate consent to send it.
- Do not upload file paths, personal content, credentials, or raw activity logs with feedback.
- An internal maintainer-only Command Agent may cluster anonymized opt-in feedback and recommend fixes. It must not prevent an uninstall, change production code, or publish an update autonomously.
- A maintainer reviews proposed changes, tests them, approves a release candidate and release notes, and explicitly authorizes publication through the update manager.
- The Command Agent and maintainer update controls are private internal tools; never expose their names, endpoints, UI, or operational details in customer-facing app surfaces.

## Connectors

Build a connector framework, not an unbounded promise to support every service. Support standards and documented APIs where available: OAuth 2.0/OIDC, MCP, REST/OpenAPI, webhooks, and local app bridges. Every connector has a publisher, verified origin, requested scopes, data handling summary, rate limits, revoke control, and per-action approval. No credential scraping or bypassing provider restrictions. Connector availability varies by provider and account plan.

## Current implementation status (verified from repository source)

- Existing: Windows WPF shell; Windows speech recognition and synthesis; manual start/stop listening; companion window; Defender status/confirmed scan; limited read-only diagnostics; basic file triage.
- Not implemented by this document: always-on wake phrase, tray background service, first-run permission wizard, ten-minute overlay lifecycle, Explorer context menu, file-to-agent AI execution, dictation-to-arbitrary-field insertion, workflow timeline, full capability gallery, JT app bridges, generic connector catalogue, branded cross-platform installers/uninstallers, opt-in uninstall feedback pipeline, private maintainer Command Agent/update approval workflow.
- The current app targets .NET 8 WPF on Windows. macOS/Linux require native ports and separate permission/install flows; Windows artifacts are not cross-platform ports.

## Delivery gates

1. Privacy/permission architecture and tray lifecycle.
2. First-run onboarding, tray resident mode, wake phrase and companion positioning.
3. File handoff with read-only analysis, proposed diffs and approval gate.
4. Activity timeline, permissions centre, dictation and capability gallery.
5. Versioned JT app bridge and shared JT AI Core client.
6. Connector framework and integrations.
7. Per-platform installer/uninstaller, signed packages, optional feedback and maintainer-controlled release pipeline.
8. Cross-platform ports only after native implementation and platform testing.

Do not mark a milestone complete until code, tests, package, and platform-specific verification exist.
