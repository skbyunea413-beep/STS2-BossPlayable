# Boss Playable: Prism

Prism is a playable boss-character mod for **Slay the Spire 2**.

This repository is the Prism entry in a broader "Boss Playable" mod project: the goal is to turn selected bosses and ancient beings into fully playable characters with their own card pools, relics, dialogue hooks, and event interactions.

## Current Status

- Playable character: **Prism Shirou**
- Mod id: `PrismMod`
- Game: Slay the Spire 2
- Language data: English and Korean
- Install guide: Korean, English, and Chinese in [INSTALLATION.txt](INSTALLATION.txt)

## Required Dependencies

Prism currently targets these stable library versions:

```text
BaseLib 3.1.2
STS2-RitsuLib 0.2.28
```

The dependency installer included with the mod installs those exact versions.

Manual download links are also listed in [INSTALLATION.txt](INSTALLATION.txt).

## Project Layout

```text
.
├── src/                         # Godot/.NET Slay the Spire 2 mod project
│   ├── PrismModCode/             # C# gameplay code
│   ├── PrismMod/                 # Godot resources, localization, images
│   ├── PrismMod.csproj
│   └── PrismMod.json
├── tools/                       # Dependency helper scripts
├── design/                      # Early design notes
├── IMAGE/                       # Source/reference image material
├── INSTALLATION.txt             # User installation guide
├── PROJECT_CONTEXT.md           # Handoff notes for future contributors or AI sessions
└── install_stable_sts2_dependencies.bat
```

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

Build and publish copy the mod files into the detected Slay the Spire 2 `mods/PrismMod` folder.

## Installation

For players, use the packaged release zip from GitHub Releases.

For local development builds:

1. Copy or publish `PrismMod` into the game `mods` folder.
2. Run `install_stable_sts2_dependencies.bat`.
3. Start Slay the Spire 2 and enable `Prism Shirou`.

See [INSTALLATION.txt](INSTALLATION.txt) for detailed instructions in Korean, English, and Chinese.

## Release Packaging

Do not commit compiled `.dll` or `.pck` files to this source repository.

Recommended release asset:

```text
PrismMod.zip
└── PrismMod/
    ├── PrismMod.dll
    ├── PrismMod.pck
    ├── PrismMod.json
    ├── INSTALLATION.txt
    ├── install_stable_sts2_dependencies.bat
    └── tools/
```

## Future Sessions

When continuing this project in another session, start by reading [PROJECT_CONTEXT.md](PROJECT_CONTEXT.md).
