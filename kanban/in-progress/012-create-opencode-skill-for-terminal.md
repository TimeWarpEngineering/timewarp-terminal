# Create opencode skill for terminal

## Description

Create an OpenCode skill for the TimeWarp.Terminal library. The skill should provide detailed instructions and best practices for using TimeWarp.Terminal (a console/TUI library for building terminal applications), similar to existing skills like playwright, runfiles, and kanban.

## Checklist

- [ ] Create directory structure at `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-flow/Cramer-2025-08-01-cron/opencode/skills/terminal/`
- [ ] Examine existing skills for structure and format consistency
- [ ] Extract key information from TimeWarp.Terminal README.md
- [ ] Write SKILL.md with front matter (name, description)
- [ ] Document core abstractions (IConsole, ITerminal, Terminal API)
- [ ] Document widgets (Panel, Table, Rule)
- [ ] Document ANSI colors and styling
- [ ] Document hyperlinks (OSC 8 support)
- [ ] Document testing patterns (TestTerminal, mocking)
- [ ] Add best practices for AI agents
- [ ] Include 10+ code examples
- [ ] Create quick reference table
- [ ] Validate markdown format
- [ ] Verify no Terminal.Gui references

## Notes

### Implementation Plan: TimeWarp.Terminal Skill for OpenCode

#### Skill Location
- Directory: `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-flow/Cramer-2025-08-01-cron/opencode/skills/terminal/`
- File: `SKILL.md`

#### Main Sections to Create

1. **Front Matter**
   - name: terminal
   - description: TimeWarp.Terminal library usage

2. **Introduction**
   - What is TimeWarp.Terminal
   - When to use it
   - Installation

3. **Core Abstractions**
   - IConsole vs ITerminal
   - Static Terminal API
   - Implementation classes

4. **Widgets**
   - Panel: Bordered panels with headers
   - Table: Multi-column tables
   - Rule: Horizontal rules

5. **ANSI Colors & Styling**
   - Color methods
   - Style chaining
   - ConsoleColor support

6. **Hyperlinks**
   - OSC 8 support
   - Terminal compatibility

7. **Testing**
   - TestTerminal usage
   - Mocking patterns

8. **Best Practices for AI Agents**
   - When to use each abstraction
   - Testing patterns
   - Common patterns

9. **Quick Reference**
   - Cheat sheet format

#### Implementation Steps

1. Create directory structure
2. Extract key info from README.md
3. Write SKILL.md with all sections
4. Include 10+ code examples
5. Create quick reference table
6. Validate markdown and format
7. Compare to existing skills for consistency

#### Success Criteria

- File at correct path with SKILL.md filename
- Front matter with name and description
- All major library features documented
- 10+ practical code examples
- Testing patterns explained
- Best practices for AI agents
- Quick reference table
- Matches existing skill format
- No Terminal.Gui references

---

**Original Notes:**
Target location: `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-flow/Cramer-2025-08-01-cron/opencode/skills/terminal.md`

The skill should cover:
- How to use the TimeWarp.Terminal library effectively
- Core abstractions and when to use each
- Widget usage (Panel, Table, Rule)
- ANSI colors and styling
- Testing patterns with TestTerminal
- Best practices for terminal-based development workflows

This skill will be loaded via the `skill` tool to provide context-specific guidance when working with TimeWarp.Terminal library operations.