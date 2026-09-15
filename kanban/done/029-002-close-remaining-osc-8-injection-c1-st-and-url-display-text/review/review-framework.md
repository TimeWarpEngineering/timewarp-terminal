# Review framework — task 029-002

**Date:** 2026-09-15
**Host task:** kanban/in-progress/029-002-close-remaining-osc-8-injection-c1-st-and-url-display-text/
**Diff scope:** commit `adac20c` (`fix: close remaining OSC 8 C1 ST and URL-as-display injection`) vs `origin/feature/overnight-terminal`. Kanban-only commit `260c550` is out of product-code scope. Unrelated `.gitignore` / sibling-task files on the branch vs `origin/master` are out of scope.
**Plan / brief:** Land parent 029 round-1 merged findings M5 and M6 — percent-encode C1 (`U+0080`–`U+009F`, including ST `U+009C`) in `SanitizeUrl`; when `displayText` is omitted, reuse the sanitized URL as display text and on the `!SupportsHyperlinks` plain path; keep explicit caller `displayText` unmodified.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** Review oracle Grok `01a0a36f-c1c3-7ea1-ba95-27afff745e24` (2026-09-15); implementer Grok `01a0a368-5323-7ba0-8bd0-e9c824921ce7` (2026-09-15)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
