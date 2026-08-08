# Bring repo audit-clean on TimeWarp.Nuru.DevCli 3.0.0-beta.72

## Description

Org wave (timewarp-nuru 458-010 remediation + DevCli 3.0.0-beta.72 adoption —
they are the same wave: the audit's `nuru` check went red org-wide when
beta.72 shipped, by design). Passing `ganda repo audit` now means adopting the
full release toolkit: `dev release`, promotion gates, attestation verifier,
trusted-publishing probe, derived package sets.

## Checklist

- [x] `ganda repo audit --fix` (bumps TimeWarp.Nuru/DevCli to latest, fixes kebab/structure where fixable)
- [x] Verify Directory.Packages.props pins TimeWarp.Nuru.DevCli (and TimeWarp.Nuru where referenced) at 3.0.0-beta.72
- [x] Build — NURU050 names any missing DI registration (e.g. `IPackableProjectService`); add per the DevCli readme migration notes (CS0101 local-CiMode note also applies)
- [x] `dev self-install` (AOT binary is a snapshot; new commands like `release` are absent until reinstalled)
- [x] `ganda repo audit` → PASSES ALL CHECKS (if a check is structurally unfixable here, record it explicitly with a reason instead of forcing)
- [x] Smoke: `dev --help` shows `release`; `dev check-version` derives the packable set (publishers only)
- [x] Commit everything (audit fixes, props, dev.cs, kanban) — local commits fine; ride the repo's normal merge flow

## Notes

Created 2026-08-08 from the nuru 458 program session. timewarp-nuru is the
reference (audit-clean at beta.72, first release shipped through the full
machinery).

### Implementation notes (2026-08-08)

**Before audit:** 18 pass / 2 fail — `nuru` (beta.71) + `kebab-path-names` (5 paths).

**After:** `ganda repo audit` → 20 pass / 0 fail.

Hand fixes beyond `--fix`:
- `TimeWarp.Nuru.DevCli` pin was still beta.71 after `--fix` (only Nuru was bumped) → set to `3.0.0-beta.72`
- `TimeWarp.Amuru` → `1.0.0` (NU1605: Nuru beta.72 requires Amuru ≥1.0.0)
- Added `TimeWarp.Amuru.Tools` `1.0.0-beta.2` (IRepoCleanService moved out of core Amuru at 1.0.0)
- DI: removed `GitTagCheckService` (gone from DevCli package), added `IPackableProjectService`/`PackableProjectService`
- NoWarn for IDE0022/IDE0046/IDE0066/IDE0078 on DevCli content files from NuGet cache
- `ganda repo audit --fix --checks kebab-path-names` renamed `LICENSE`→`license`, `README.md`→`readme.md`, and three workspace underscore filenames

## Results

Repo is audit-clean on TimeWarp.Nuru / DevCli **3.0.0-beta.72**. Dev CLI builds, self-installs, exposes `release`, and `check-version` runs against the packable set.

### How to validate

**Smoke**
```bash
cd /home/steve/worktrees/github.com/TimeWarpEngineering/timewarp-terminal/dev
grep -E 'TimeWarp\.(Nuru|Amuru)' Directory.Packages.props
# Expect: Nuru + DevCli 3.0.0-beta.72; Amuru 1.0.0; Amuru.Tools 1.0.0-beta.2

ganda repo audit
# Expect: Repository passes all audit checks.

./bin/dev --help
# Expect: commands include release, check-version, clean, self-install

./bin/dev check-version
# Expect: reports packable package(s); may say version already released (1.0.0) — not a failure of the wave
```

**Automated gate**
```bash
ganda repo audit   # exit 0
```

**Depends on / Not in scope**
- Local commits only; no push
- Full solution test suite / NuGet publish not required for this task

## Session

- Implementation: grok (2026-08-08) — audit --fix + hand pins/DI/Amuru.Tools + kebab fix + self-install
