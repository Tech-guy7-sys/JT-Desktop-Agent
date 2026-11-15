# JT Desktop Agent / Agent Jeff — Release Notes

## 0.2.0 — Command Deck and productivity actions (draft, 15 November 2026)

**Status:** Pull request / release candidate. Not published. Windows build and CI verification are required before tagging a release.

### Added
- Searchable command palette opened with Ctrl+K.
- Quick actions for local file review, manual voice input, Security Center, and the floating Jeff companion.
- Transcript copy and user-selected file export.
- Recent activity history stored locally with action labels only.
- Confirmation before clearing local activity history.

### Safety and privacy
- Microphone capture remains user-started and visibly active; there is no background recording or wake-word capture in this build.
- File review remains a local metadata/hash review workflow. It is not represented as a full antivirus engine or automatic remediation.
- Activity history excludes transcripts, file contents, selected file paths and credentials.
- Saving a transcript requires the user to choose a destination.
- Desktop/device control remains permission-gated and exclusive to JT Desktop Agent.

### Verification
- XAML and Windows WPF compilation must pass in the repository's Windows CI.
- No native Windows installer or published release is claimed by this draft.
