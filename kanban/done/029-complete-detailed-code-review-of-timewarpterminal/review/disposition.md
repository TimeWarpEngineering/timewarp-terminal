# Disposition — task 029

**Date:** 2026-09-04
**Outcome:** pending-children (not `clean` / `accepted-exceptions` until the children land)
**Rounds:** 1
**Final open count:** 21 open, **0 unfiled** — all opens filed as `--parent 029` children
**Pinned SHA reviewed:** `1a6a29b66c38ba24b6306520de554b22def7bc74` (origin-home `master` at implement start; no product delta on this branch)

## Summary

Elevated six-area round 1 of TimeWarp.Terminal 1.0.1 (whole-repo, not a PR delta). 022 blockers/majors/minors were not re-opened; remaining defects are 022 gaps (TestConsole never got the TestTerminal Dispose/Read mirror; snupkg is produced but never pushed), new panel layout bugs, residual OSC 8 channels (C1 ST + URL-as-display), CI path filters that skip `tests/` and `samples/`, and FormatProvider parallel isolation. Independent product fixes stay on this id via children — no sibling “apply 029 findings” task. Parent stays **in-progress** until those children land.

## Children (open findings)

| Child | Merged IDs | Batch | Origin-home |
|-------|------------|--------|-------------|
| **029-001** | M1, M2, M3, M4, M14, M18 | TestConsole/TestTerminal parity + FormatProvider | published `5455b0b` |
| **029-002** | M5, M6 | OSC 8 C1 ST + URL display text | published `1e06939` |
| **029-003** | M7, M8, M15, M16, M19 | Panel layout / wrap / builders | published `bbe2d87` |
| **029-004** | M9, M10, M20, M21 | CI path filters, snupkg push, pack Include, banner | published `28e5005` |
| **029-005** | M11, M12, M13, M17 | Title getter, Beep docs, CancelKeyPress, stderr color | published `5f2ab30` |

## Exception log (if accepted-exceptions)

None. No `wontfix` this round.

## Escalations

- None. Child-filed exit bar is the task 029 kitchen rule (`task.md` Evaluate step 5), not a human stalemate.

## Paths

- `review/review-framework.md`
- `review/round-1/core-abstractions.md`
- `review/round-1/static-facade.md`
- `review/round-1/test-doubles.md`
- `review/round-1/widgets.md`
- `review/round-1/tests-infra.md`
- `review/round-1/security.md`
- `review/round-1/merged.md`
- `review/disposition.md` (this file)
