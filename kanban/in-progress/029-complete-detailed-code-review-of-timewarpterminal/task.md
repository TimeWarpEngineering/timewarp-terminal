# Complete detailed code review of TimeWarp.Terminal

## Description

Whole-repo implementation review of **TimeWarp.Terminal as it exists on origin-home `master`**, not a PR delta.

This is the successor pass to kanban **022** (1.0 release-readiness review of ~25 library files). 022’s blockers/majors/minors all landed; the package is now **1.0.1**. This task re-reviews the **current** tree (SHA pinned in `review/review-framework.md`) for new defects, regressions of 022 fixes, and gaps 022 did not cover (tests, samples, dev-cli, CI path filters, 1.0.1 delta, widget/ANSI/test-double contracts that froze at 1.0).

Procedure: `tw-implementation-review` with **elevated effort** (area specialists, not default effort-1). Artifacts live under this folder task's `review/` subfolder. Same task through disposition — do **not** create a sibling “apply review findings” task.

## Requirements

### Scope (in)

Review product truth in this repo:

| Area | Paths |
|------|--------|
| Core abstractions | `iconsole.cs`, `iterminal.cs`, `timewarp-console.cs`, `timewarp-terminal.cs` — platform gating, `new`/explicit-impl pairing (`source/timewarp-terminal/AGENTS.md`) |
| Static facade | `terminal-static.cs` — `Terminal.Instance`, FormatProvider, color, CancelKeyPress |
| Hyperlinks / ANSI | `ansi-*.cs`, `terminal-hyperlink-extensions.cs` — OSC 8, SupportsColor / SupportsHyperlinks / NO_COLOR |
| Test doubles | `test-terminal.cs`, `test-console.cs`, `test-terminal-context.cs` — AsyncLocal isolation, Read/KeyAvailable/QueueKey |
| Widgets | `widgets/` — table, panel, rule, unicode-width, ansi-string-utils, borders/truncate |
| Tests | `tests/` Jaribu runfiles vs `dev test` inclusion |
| Samples / tools / infra | `samples/`, `tools/dev-cli/`, MSBuild, `.github/workflows/workflow.yml` (path filters), packaging/AOT/snupkg |

Every finding **must** cite `path:line` evidence in the current tree. Zero issues in an area is a valid outcome. Do not invent findings.

### Scope (out)

- Re-opening **022** findings already marked done unless the defect is **still present** (prove it with current file:line).
- **023** (`TimeWarp.Terminal.Layout` / Flexbox) — package does not exist yet; do not review a hypothetical layout API.
- **027-001** publish-smoke leftover and **028** journal gitignore — board hygiene, not this review.
- Strategic redesign of IConsole vs ITerminal (022 already moved `ReadKey` to ITerminal).
- Docs-only polish unless a doc **contradicts** code or ships a broken sample.

### Reviewer roster (effort)

| File | Area |
|------|------|
| `core-abstractions.md` | IConsole/ITerminal/TimeWarpConsole/TimeWarpTerminal, Unix vs Windows gates, fluent `new` pairing |
| `static-facade.md` | Terminal static, FormatProvider, color degradation, CancelKeyPress, Instance vs AsyncLocal |
| `test-doubles.md` | TestTerminal/TestConsole/TestTerminalContext contracts vs real console |
| `widgets.md` | table/panel/rule, unicode-width, wrap/truncate, emoji/ANSI in borders |
| `tests-infra.md` | CI inclusion (workflow path filters omit `tests/` and `samples/` — verify), packaging, samples, 1.0.1 delta |
| `security.md` | OSC 8 / ANSI injection, exception swallowing, redirected stdin/stdout |

Severity: `bug` · `suggestion` · `nit`. Status starts `open`. Prefer strongest severity when merging duplicates.

### Kitchen / procedure

1. Re-pin `review/review-framework.md` to the SHA actually reviewed (`git rev-parse origin/master`).
2. Round 1: spawn area reviewers (read-only on product code; write only under `review/round-1/`).
3. Merge → `review/round-1/merged.md` with stable `M#` IDs and counts table.
4. Evaluate:
   - Independent product fixes → **child tasks** (`ganda kanban create … --parent 029`), one coherent batch per child.
   - Tiny nits that belong on this branch → fix here, then `round-2/` re-review of the fix delta.
   - `wontfix` only with rationale + decider on the live `merged.md`.
5. Write `review/disposition.md` (`clean` or `accepted-exceptions`) when open count is 0 **or** remaining opens are filed as children with IDs recorded in disposition (parent stays in-progress until those children land).
6. `## Results` **must** include rounds, roster, counts by severity/status, disposition, `review/` paths, and `### How to validate`.

**Forbidden:** process files next to `task.md`; a sibling “apply 029 findings” task; clobbering prior `round-N/`.

## Checklist

### Kitchen

- [x] Folder task created (`ganda kanban reserve` + `claim --repo timewarp-terminal`)
- [x] `review/review-framework.md` scaffolded with scope, roster, prior-art notes
- [x] Worker re-pins SHA at implement start (`origin/master` `1a6a29b66c38ba24b6306520de554b22def7bc74`)
- [x] Moved to in-progress (`ganda kanban move 029 in-progress`)

### Round 1

- [ ] Area reviewers write `review/round-1/<area>.md` (6 files) — host `review` oracle (elevated roster in `review/review-framework.md`)
- [ ] Merge → `review/round-1/merged.md` (counts + stable `M#`)

### Disposition / follow-through

- [ ] Child tasks for independent product fixes (`--parent 029`), or same-task nits committed here
- [ ] `review/disposition.md`
- [x] `## Results` + `### How to validate` (implement pass; review oracle must add disposition)
- [ ] Do not `kanban done` from the implementer; host lifecycle / human gate

## Notes

### Prior art (do not duplicate blindly)

- **022** (done, flat task, no children) — six-pass 1.0 review. All blockers/majors/minors checked. Themes: WritePanel overload CS0121; Rule negative width; Unix CursorVisible/Title/Beep gates; KeyAvailable InvalidOperationException; Terminal.Instance vs AsyncLocal; color/hyperlink gating; culture vs invariant; OSC 8 injection; TestTerminal Read/KeyAvailable/QueueKey; ReadKey moved to ITerminal; unicode-width; table ANSI ellipsis; release pipeline tests; PackageReadmeFile; AOT.
- 022 reviewed **library source only** at `1.0.0-beta.13`. This pass must also cover tests, samples, tools, CI, and post-1.0 (`1.0.1`) code.

### Snapshot at kitchen create (2026-09-04)

- Origin-home SHA: `bf38d51` (`publish kanban 028`)
- Package version: `1.0.1` (`source/Directory.Build.props`)
- Shipped package: **TimeWarp.Terminal** only. Layout package is **023**, not in tree.
- ~28 library `.cs` files + 33 tests + 5 samples + 5 tools
- Other open work: **023** (Layout, gated on Flexbox), **027-001**, **028**

### Snapshot at implement start (2026-09-04)

- Origin-home SHA: `1a6a29b66c38ba24b6306520de554b22def7bc74` (`publish kanban 029`)
- `bf38d51..1a6a29b` is this kitchen only (no product-code delta)
- Package version still `1.0.1`
- Implement did **not** spawn area reviewers; host `review` oracle owns `review/round-1/`
- Local unstaged `.gitignore` journal ignore is **028**, not this id — left unstaged

### Related skills

- `tw-implementation-review` — procedure, templates, severity, disposition
- `tw-agent-collaboration` — QA workspace `review/`, same-task disposition, Results shape
- `tw-csharp` — conventions
- Repo `source/timewarp-terminal/AGENTS.md` — IConsole/ITerminal pairing (false positives vs real bugs)

### Dispatch (cockpit — not this session)

```bash
ganda task work 029 --repo timewarp-terminal --host herdr
```

## Session

- Created: Grok cockpit `01a06a77-1631-7543-b181-07ddc524f9fe` (2026-09-04) — reserved/claimed 029, wrote inbound brief
- Ganda claim: cramer@TWE-001 session 3290412 (2026-09-04)
- Implementer: Grok `01a06a90-5bc6-7a91-8523-fa528da1fa16` (2026-09-04) — re-pin SHA, in-progress, Results; no reviewer spawn
- Ganda claim (implement pickup): cramer@TWE-001 session 3295467 (2026-09-04)

## Results

### What was implemented

Implement oracle on **029** (whole-repo TimeWarp.Terminal review). No product-code change: this id reviews origin-home `master` as it exists, and ExtraRules forbid freelance `tw-implementation-review` / reviewer spawn from implement.

- Moved kitchen `to-do` → `in-progress`.
- Re-pinned `review/review-framework.md` to `git rev-parse origin/master` = `1a6a29b66c38ba24b6306520de554b22def7bc74` (`1a6a29b publish kanban 029`). Kitchen-create pin `bf38d51` kept as history. Product files are unchanged vs that pin (`bf38d51..1a6a29b` is this kitchen only).
- Left `review/round-1/` uncreated for the host **review** oracle (elevated 6-area roster already recorded on the framework).
- Did not create child tasks (no merged findings yet). Did not `kanban done` or `gh pr create`. Did not commit local `.gitignore` journal ignore (task **028**).

### Files changed

- `kanban/in-progress/029-complete-detailed-code-review-of-timewarpterminal/task.md` (column move + this Results)
- `kanban/in-progress/029-complete-detailed-code-review-of-timewarpterminal/review/review-framework.md` (SHA re-pin)

### Key decisions

- Host walks implement → review. Round-1 artifacts belong to the review node so it does not clobber or dual-write `round-1/`.
- Review scope stays whole-repo origin-home `master` at the pinned SHA, not this branch’s kanban-only delta.
- Roster remains elevated: `core-abstractions`, `static-facade`, `test-doubles`, `widgets`, `tests-infra`, `security`.

### Test outcomes

- No library/tests/samples/tools edits; no `./bin/dev test` run this pass.
- `git fetch origin master` then `git rev-parse origin/master` → `1a6a29b66c38ba24b6306520de554b22def7bc74`.
- `git diff --stat bf38d514990febd2815294908b2a599c6f6e0bab..1a6a29b66c38ba24b6306520de554b22def7bc74` → two kitchen files only.

### Review disposition

- **Outcome:** pending host `review` oracle (not `clean` / `accepted-exceptions` yet)
- **Rounds:** 0 (round 1 not started)
- **Effort / roster:** elevated; 6 area files named in `review/review-framework.md`
- **Final counts:** n/a until `review/round-1/merged.md`
- **Wontfix / escalations:** none this pass
- **Paths:**
  - `review/review-framework.md` (pinned SHA + roster)
  - `review/round-1/` — not written by implement
  - `review/disposition.md` — not written by implement

### How to validate

**Smoke**

```bash
git fetch origin master
echo "origin/master=$(git rev-parse origin/master)"
grep -n 'Pinned SHA at implement start' kanban/in-progress/029-complete-detailed-code-review-of-timewarpterminal/review/review-framework.md
test ! -d kanban/in-progress/029-complete-detailed-code-review-of-timewarpterminal/review/round-1 && echo 'round-1 absent (review oracle owns it)'
ganda kanban show 029
```

**Expect**

- `origin/master` equals `1a6a29b66c38ba24b6306520de554b22def7bc74`.
- Framework line contains that full SHA; roster lists the six area names; host path is `kanban/in-progress/029-…`.
- `review/round-1/` does not exist until the host review oracle runs.
- `ganda kanban show 029` reports column `in-progress` and claim held.
- `git diff origin/master -- source tests samples tools .github` is empty (no product delta from implement).

**Not in scope this pass:** spawning the six area reviewers; `review/disposition.md`; child tasks; `./bin/dev test`; committing `.gitignore` (028).
