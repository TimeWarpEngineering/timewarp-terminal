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

- [x] Master protection no longer blocks admin/operator `kanban/**` push (`enforce_admins` false)
- [x] Force-push and deletions still off
- [x] `ganda kanban publish` succeeds on a kanban-only kitchen (paste CLI output in Notes)
- [x] Later terminal to-dos use publish, not a kanban-only PR, as the default inbox path

## Session

- Created: ganda session 920940 (2026-09-02)
- Cockpit: grok `01a03d38-9611-7620-aae5-848e15dafa94` (timewarp-flow)
- Trigger: nuru 468 other-repo scan — timewarp-terminal was the remaining public repo with
  `enforce_admins: true` + require PR
- Implementer: grok session (2026-09-02) — `enforce_admins` off + publish proof 027-001

## Notes

Sibling: timewarp-nuru **468** (done). Recipe: `gh api -X DELETE repos/TimeWarpEngineering/timewarp-terminal/branches/master/protection/enforce_admins`, then prove publish.

**Path used:** 027 kitchen landed via PR [#28](https://github.com/TimeWarpEngineering/timewarp-terminal/pull/28) while publish was still blocked. Operator then `DELETE`d `enforce_admins` (2026-09-02). Proof publish is child **027-001** (027 claim worktree kept for host review / open-pr).

`ganda kanban publish 027-001` (2026-09-02, after `enforce_admins` off):

```
Published task 027-001 to origin/master
Pushed:   dc075a7
Released claim
027-001 is in kanban/to-do/ on origin-home. Any agent can claim it.
Kitchen worktree removed.
Next edit: ganda kanban claim 027-001 (fresh tree from origin-home).
cwd is origin-home: /home/steve/worktrees/github.com/TimeWarpEngineering/timewarp-terminal/master
```

## Results

### What was implemented

Classic branch protection on `TimeWarpEngineering/timewarp-terminal` `master`: turned off
**Include administrators** (`gh api -X DELETE .../protection/enforce_admins`). Left required
PR reviews (0 approvals). Did not enable force-push or deletions. No product code. No ganda
PR-fallback (ganda 221).

### Files changed

- `kanban/to-do/027-…/task.md` (this file) — Notes / Results
- GitHub branch protection (not in git)
- Child kitchen `kanban/to-do/027-001-….md` already on origin-home via publish (`dc075a7`)

### Key decisions

- Match nuru 468 / architecture / state (`enforce_admins: false`) rather than dropping
  require-PR.
- 027 itself could not publish until the setting changed, so first landing was PR #28.
- Proof used later id **027-001** so this claim worktree stays for the host walk.

### Test outcomes

- After DELETE: `enforce_admins: false`, require-PR reviews still on (0 approvals),
  `allow_force_pushes: false`, `allow_deletions: false`. No rulesets (`[]`).
- `ganda kanban publish 027-001` succeeded (output in Notes).

### How to validate

**Smoke**

```bash
gh api repos/TimeWarpEngineering/timewarp-terminal/branches/master/protection --jq '{enforce_admins: .enforce_admins.enabled, approvals: .required_pull_request_reviews.required_approving_review_count, allow_force_pushes: .allow_force_pushes.enabled, allow_deletions: .allow_deletions.enabled}'
# expect: enforce_admins false, approvals 0, allow_force_pushes false, allow_deletions false

git fetch origin
git log -1 --oneline origin/master
# expect: includes `publish kanban 027-001` (dc075a7) or a later publish commit
```

**Expect**

- Admin/operator `ganda kanban publish <id>` from a kanban-only kitchen prints
  `Published task … to origin/master` (not GH013 / “Changes must be made through a pull request”).
- Required PR reviews remain for non-admin traffic.
- Force-push and branch deletion stay off.

**Not in scope:** inventing a ganda publish PR-fallback; dropping require-PR for everyone.
