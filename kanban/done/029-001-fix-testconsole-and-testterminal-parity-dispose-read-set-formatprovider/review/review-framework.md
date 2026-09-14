# Review framework — task 029-001

**Date:** 2026-09-15
**Host task:** kanban/in-progress/029-001-fix-testconsole-and-testterminal-parity-dispose-read-set-formatprovider/
**Diff scope:** commit `a826926` (`fix: isolate TestConsole I/O and FormatProvider across parallel tests`) vs `origin/feature/overnight-terminal` (`ae263b5`). Unstaged `.gitignore` is out of scope (unrelated local ignore).
**Plan / brief:** Land parent 029 round-1 merged findings M1, M2, M3, M4, M14, M18 — TestConsole Dispose/Read parity, SetIn/SetOut/SetError I/O redirect on both doubles, FormatProvider AsyncLocal isolation + immutable snapshot chain, ReadKey constructor shift, Clear vs ClearOutput docs.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** Review oracle Grok `01a0a243-c399-7711-b5e7-0ea48c6a87f0` (2026-09-15); implementer Grok `01a0a236-27b6-7b13-a0f9-0b2f6578c348` (2026-09-15)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
