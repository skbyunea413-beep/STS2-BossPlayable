# STS2 Boss Playable

This repository is a **Boss Playable** project for **Slay the Spire 2**.

The long-term goal is to turn selected bosses, ancient beings, and special encounters into playable characters with their own card pools, relics, dialogue hooks, event interactions, and release packages.

The current playable entry is:

```text
Prism
```

Prism still uses the in-game mod id `PrismMod`. That name is part of the current Slay the Spire 2 mod package and should remain stable unless the project is deliberately migrated.

## Project Scope

```text
Project: STS2 Boss Playable
Current entry: Prism
Current game mod id: PrismMod
Current source location: src/
```

The repository is intentionally larger than Prism alone. Prism is the first implementation and currently lives at the repository root for build stability. Future boss entries can be added as sibling modules once the project grows beyond a single playable character.

Possible future layout:

```text
characters/
  Prism/
  Architect/
  Orobas/
shared/
docs/
```

Current layout:

```text
.
|-- src/                         # Current PrismMod Godot/.NET project
|   |-- PrismModCode/             # C# gameplay code
|   |-- PrismMod/                 # Godot resources, localization, images
|   |-- PrismMod.csproj
|   `-- PrismMod.json
|-- tools/                       # Dependency helper scripts
|-- design/                      # Early design notes
|-- IMAGE/                       # Source/reference image material
|-- INSTALLATION.txt             # Player installation guide
|-- PROJECT_CONTEXT.md           # Handoff notes for contributors and AI sessions
|-- RELEASE_CHECKLIST.md
`-- install_stable_sts2_dependencies.bat
```

## Current Entry: Prism

- Playable character: `Prism Shirou`
- Mod id: `PrismMod`
- Game: Slay the Spire 2
- Language data: English and Korean
- Install guide: Korean, English, and Chinese in [INSTALLATION.txt](INSTALLATION.txt)

Prism currently uses a gambling/random-cast identity with custom boss and ancient-event interactions.

Current balance reference:

- [STS2 base character balance benchmarks](design/08_sts2_balance_benchmarks.md)

## Required Dependencies

The currently validated dependency versions are:

```text
BaseLib 3.1.2
STS2-RitsuLib 0.2.28
```

The dependency installer included with the mod installs those exact versions.

Manual download links are listed in [INSTALLATION.txt](INSTALLATION.txt).

## Build

Requirements:

- .NET 9 SDK
- Slay the Spire 2 installed
- Godot 4.5.1 mono executable for publishing `.pck`
- BaseLib and STS2-RitsuLib installed in the game `mods` folder

Build:

```powershell
dotnet build src/PrismMod.csproj
```

Publish:

```powershell
dotnet publish src/PrismMod.csproj
```

Build and publish copy the current Prism mod files into the detected Slay the Spire 2 `mods/PrismMod` folder.

## Installation

For players, use the packaged release zip from GitHub Releases.

For local development builds:

1. Copy or publish `PrismMod` into the game `mods` folder.
2. Run `install_stable_sts2_dependencies.bat`.
3. Start Slay the Spire 2 and enable `Prism Shirou`.

See [INSTALLATION.txt](INSTALLATION.txt) for detailed instructions in Korean, English, and Chinese.

## Release Packaging

Do not commit compiled `.dll` or `.pck` files to this source repository.

Recommended current Prism release asset:

```text
PrismMod.zip
`-- PrismMod/
    |-- PrismMod.dll
    |-- PrismMod.pck
    |-- PrismMod.json
    |-- INSTALLATION.txt
    |-- install_stable_sts2_dependencies.bat
    `-- tools/
```

## Future Sessions

When continuing this project in another session, start by reading [PROJECT_CONTEXT.md](PROJECT_CONTEXT.md).
