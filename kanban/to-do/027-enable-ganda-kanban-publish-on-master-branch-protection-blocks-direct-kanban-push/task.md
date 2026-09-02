# Enable ganda kanban publish on master (branch protection blocks direct kanban push)

## Description

`ganda kanban publish` pushes **kanban/** only straight to origin-home. It has **no PR fallback**.
This repo’s classic **master** branch protection **includes administrators**, so that push is
refused (`GH013` / “Changes must be made through a pull request”). Kitchens cannot land as
to-do inbox without a product-style PR.

Same trap as timewarp-nuru **468** (fixed 2026-09-02: `DELETE .../protection/enforce_admins`).
Found in the 468 public-repo scan. No GitHub rulesets (`GET /repos/.../rulesets` → `[]`).

Ganda policy (tw-kanban / `KanbanPublishService.FormatRequirePullRequestMessage`): turn off
“Require a pull request before merging” **or** add a bypass this identity can use. Path
allowlists for `kanban/` are not available on classic protection.

## Evidence (2026-09-02, `gh api .../branches/master/protection`)

| Repo | `enforce_admins` | Require PR reviews | Required checks | `kanban publish` |
|---|---|---|---|---|
| **timewarp-terminal** | **true** (Include administrators) | yes, 0 approvals | none | **blocked** (this task) |
| timewarp-nuru | false (after 468) | yes, 0 approvals | `ci` | works |
| timewarp-architecture | false | yes, 0 approvals | none | works |
| timewarp-state | false | yes, 0 approvals | none | works |

## Requirements

GitHub **Settings → Branches → master** (operator; not a product-code change):

- Set **Do not include administrators** (`enforce_admins: false`), matching nuru 468 /
  architecture / state.
- Keep required PR reviews for non-admin traffic.
- Do **not** enable force-push or branch deletion.
- Do **not** invent a ganda PR-fallback (ganda 221).

Prove:

```bash
gh api repos/TimeWarpEngineering/timewarp-terminal/branches/master/protection --jq '.enforce_admins.enabled'
# expect: false

# from a claimed terminal kitchen with only kanban/** vs origin/master:
ganda kanban publish 027   # or a later id if 027 already landed via PR
# expect: Published task … to origin/master
```

If 027 itself cannot be published until the setting changes, land this kitchen via PR, change
protection, then the **next** kitchen uses publish. Record which path was used.

## Checklist

- [ ] Master protection no longer blocks admin/operator `kanban/**` push (`enforce_admins` false)
- [ ] Force-push and deletions still off
- [ ] `ganda kanban publish` succeeds on a kanban-only kitchen (paste CLI output in Notes)
- [ ] Later terminal to-dos use publish, not a kanban-only PR, as the default inbox path

## Session

- Created: ganda session 920940 (2026-09-02)
- Cockpit: grok `01a03d38-9611-7620-aae5-848e15dafa94` (timewarp-flow)
- Trigger: nuru 468 other-repo scan — timewarp-terminal was the remaining public repo with
  `enforce_admins: true` + require PR

## Notes

Sibling: timewarp-nuru **468** (done). Recipe: `gh api -X DELETE repos/TimeWarpEngineering/timewarp-terminal/branches/master/protection/enforce_admins`, then prove publish.
