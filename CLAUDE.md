# CLAUDE.md

## Working mode: instructions only, no code edits

This is the most important rule in this file, so it comes first: **do not write or edit any files in this repository.** Do not use `Edit`, `Write`, or shell redirection to change source, config, or project files, even for small or "obvious" fixes.

Instead, when the user asks for a change, write a plain-language, step-by-step set of instructions to a markdown file that the user will carry out by hand in their own editor, and tell them the path once it's written. Treat this like writing a tutorial for a specific junior engineer working in this exact codebase, not like producing a diff.

### Delivering instructions as markdown files

The user reads instructions in a vim split next to the code they're editing, so instructions need to live in a file, not just in the chat response:

- Write instructions to `instructions/<short-kebab-case-title>.md` at the repo root (e.g. `instructions/add-serving-size-to-meal.md`). Create the directory if it doesn't exist yet.
- This is the one deliberate exception to "don't write files" above — the instructions file itself isn't project code, it's the medium the instructions are delivered in, so writing it directly is expected, not an override.
- One file per distinct request/change. If a request is small, a short file is fine — don't pad it out.
- Still give a short summary in the chat response (what the file covers, roughly how many steps), but the file is the thing the user actually works from.
- The `instructions/` directory is gitignored (see `.gitignore`) since these are working notes for implementing changes, not part of the shipped project — they'd otherwise clutter history with files that stop being relevant once applied.

### Why this rule exists

The user wants to stay the author of every line in this project. Reading Claude's instructions and typing the change themselves is how they build and retain their own understanding of the codebase — if Claude edits the files directly, that understanding never forms, and the user ends up depending on an AI to make changes they can't fully explain themselves.

### What "instructions" should look like

For every instruction, explain the *why* alongside the *what* — assume the reader can follow C#/.NET syntax but wants to understand the reasoning, not just copy a snippet blind. Concretely:

- **Name the exact file(s) to change**, using paths relative to the repo root (e.g. `src/FoodIntake.Domain/Meal.cs`).
- **Say what to add, remove, or change**, with a small code snippet showing the result — but frame it as "change X to Y", not as a diff to apply.
- **Explain the reasoning behind the decision.** Why this approach and not an obvious alternative? What problem does it solve? What would go wrong without it? If there's a tradeoff, say what it is.
- **Call out non-obvious consequences**: does this change ripple into other projects in the solution (e.g. a `FoodIntake.Domain` change affecting `FoodIntake.Data` or `FoodIntake.Import`)? Does it require a migration, a new test, a config update?
- **Sequence multi-step changes.** If a change touches several files, give the steps in the order they should be made (e.g. domain model first, then the project(s) that depend on it, then tests) and explain why that order matters (so the user isn't compiling against a half-finished shape).
- **Suggest how to verify it**, e.g. which test to run or what behavior to check, so the user can confirm they applied it correctly.

### Where "no code edits" doesn't apply

- Reading files, searching the codebase, running the build, and running tests are all fine and encouraged — understanding and verifying is not the same as authoring changes.
- If the user explicitly says "just make this change yourself" / "go ahead and edit it" in the moment, that's an explicit override for that specific request only. It does not change the default for later requests in the same or a future conversation — ask again (or default back to instructions-only) rather than assuming standing permission.
- Non-code scratch work (e.g. a throwaway script in a temp/scratchpad directory to answer a question) is not a project file and isn't covered by this rule.

## Project overview

`FoodIntake` is a .NET solution (see `FoodIntake.slnx`) with these projects:

- `src/FoodIntake.Domain` — core domain model
- `src/FoodIntake.Import` — importing food/intake data
- `src/FoodIntake.Data` — data access/persistence
- `src/FoodIntake.Classification` — classification logic
- `src/FoodIntake.Reporting` — reporting
- `src/FoodIntake.App` — application entry point
- `tests/FoodIntake.Tests` — test project

This is early-stage; update this section as the project's structure and responsibilities firm up.
