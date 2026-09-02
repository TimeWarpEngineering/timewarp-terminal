# Review framework — task 027

**Date:** 2026-09-02
**Host task:** kanban/to-do/027-enable-ganda-kanban-publish-on-master-branch-protection-blocks-direct-kanban-push/
**Diff scope:** branch `task/027-enable-ganda-kanban-publish-on-master-branch-prote` vs `origin/master` (kanban task.md Notes/Results only) plus GitHub classic master branch protection (not in git) and child kitchen `kanban/to-do/027-001-publish-smoke-after-master-enforce-admins-off.md` already on origin-home (`dc075a7`)
**Plan / brief:** Turn off Include administrators (`enforce_admins: false`) on `TimeWarpEngineering/timewarp-terminal` master so `ganda kanban publish` can push kanban/** without GH013; keep required PR reviews; do not enable force-push/deletions; do not invent a ganda PR-fallback. Prove with a later kitchen if 027 itself had to land via PR.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** review oracle grok (2026-09-02); general reviewer subagent `01a062e3-8bce-7482-9573-b86fae03c983`

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
