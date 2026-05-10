# Project Context

## Identity

Project name:

```text
STS2 Boss Playable
```

This is a Slay the Spire 2 project for making bosses, ancient beings, and special encounters playable.

Current entry:

```text
Prism
```

Prism is the first playable boss entry and currently occupies the root `src/` project for build stability.

## Repository Scope

This repository is bigger than a single Prism-only mod.

Current reality:

- `src/` contains the active Prism implementation.
- `PrismMod` is the current in-game mod id.
- Release packaging currently produces a `PrismMod` folder.

Long-term direction:

- Keep this repository as the shared Boss Playable project.
- Add future playable bosses as new entries when needed.
- Consider moving Prism into `characters/Prism/` only when there is a second entry and the build scripts are ready for a multi-module layout.

## Current Mod Entry

```text
Entry name: Prism
Mod id: PrismMod
Display name: Prism Shirou
Author: nagis
Game: Slay the Spire 2
```

The current Prism entry uses a gambling/random-cast card identity with boss and ancient-event interactions.

## Stable Dependencies

The currently validated versions are:

```text
BaseLib 3.1.2
STS2-RitsuLib 0.2.28
```

These versions are referenced by `src/PrismMod.csproj` and installed by `install_stable_sts2_dependencies.bat`.

Manual download links:

```text
https://github.com/Alchyr/BaseLib-StS2/releases/download/v3.1.2/BaseLib.3.1.2.zip
https://github.com/BAKAOLC/STS2-RitsuLib/releases/download/v0.2.28/STS2-RitsuLib.0.2.28.variant-pack.zip
```

## Build Commands

```powershell
dotnet build src/PrismMod.csproj
dotnet publish src/PrismMod.csproj
```

The project copies build outputs to the Slay the Spire 2 `mods/PrismMod` folder.

## Local Paths Used During Development

Current development workspace:

```text
C:\Users\nagis\Documents\STS2-PrismMod
```

Current local game mods folder:

```text
Y:\SteamLibrary\steamapps\common\Slay the Spire 2\mods
```

These paths are local-machine details. Future contributors may need to set their own game path.

## Important Files

```text
src/PrismMod.csproj
src/PrismMod.json
src/PrismModCode/MainFile.cs
src/PrismModCode/Character/PrismCharacter.cs
src/PrismModCode/Character/PrismCardPool.cs
src/PrismModCode/Cards/PrismRandomCardHelper.cs
src/PrismModCode/Relics/PrismaticShard.cs
src/PrismModCode/Patches/PrismAncientDialoguePatches.cs
src/PrismMod/localization/
INSTALLATION.txt
```

## Current Prism Design Notes

- Prism is a straightforward, aggressive playable boss character.
- Random card casting and gambling effects are a major mechanic.
- The card reward pool uses Ironclad as the base plus Prism cards.
- `PrismaticShard` adds Prism cards to rewards and can apply random stable enchantments.
- Some ancient/boss interactions are custom patched for Prism.

Implemented convenience commands:

```text
prism_architect
prism_orobas
```

## Known Development Habits

When changing code:

1. Read the relevant existing C# and localization files first.
2. Keep card text, localization keys, and card implementation synchronized.
3. Run:

```powershell
dotnet build src/PrismMod.csproj
dotnet publish src/PrismMod.csproj
```

4. If game behavior is wrong, inspect:

```text
C:\Users\nagis\AppData\Roaming\SlayTheSpire2\logs\godot.log
```

## GitHub Release Policy

Do not commit compiled outputs:

```text
*.dll
*.pck
*.zip
src/.godot/
src/.build_mods/
```

Use GitHub Releases for player-facing packaged builds.

## Handoff Prompt For Future AI Sessions

Use this at the start of a new session:

```text
This repository is STS2 Boss Playable, a Slay the Spire 2 project for playable boss-character mods.
The current entry is Prism, implemented in src/ as PrismMod.
Read README.md and PROJECT_CONTEXT.md first, then continue from the current source state.
Use BaseLib 3.1.2 and STS2-RitsuLib 0.2.28 unless I explicitly ask to update dependencies.
After code changes, run dotnet build src/PrismMod.csproj and dotnet publish src/PrismMod.csproj.
```
