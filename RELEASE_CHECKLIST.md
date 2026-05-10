# Release Checklist

Use this when preparing a GitHub release.

This repository is the broader `STS2 Boss Playable` project. The current release package is for the first entry, `Prism`, whose in-game mod id is `PrismMod`.

## Before Packaging

- Run `dotnet build src/PrismMod.csproj`.
- Run `dotnet publish src/PrismMod.csproj`.
- Launch the game once and confirm Prism appears in the mod list.
- Confirm dependency versions:

```text
BaseLib 3.1.2
STS2-RitsuLib 0.2.28
```

## Current Prism Release Folder

Package the published game mod folder:

```text
Slay the Spire 2/mods/PrismMod
```

Expected current Prism release contents:

```text
PrismMod/
  PrismMod.dll
  PrismMod.pck
  PrismMod.json
  INSTALLATION.txt
  install_stable_sts2_dependencies.bat
  tools/
    install_stable_sts2_dependencies.bat
    install_stable_sts2_dependencies.ps1
```

## GitHub Source Repository

Do not commit release binaries to source:

```text
*.dll
*.pck
*.zip
src/.godot/
src/.build_mods/
```

Upload the packaged `PrismMod.zip` to GitHub Releases instead.
