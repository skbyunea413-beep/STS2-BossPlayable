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

The current Prism entry uses direct attack pressure, generated cards from other character pools, and Exhaust-pile payoffs, with boss and ancient-event interactions.

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
design/08_sts2_balance_benchmarks.md
INSTALLATION.txt
```

## Current Prism Design Notes

- Prism is a straightforward, aggressive playable boss character.
- The core identity is persistent attack pressure backed by generated cards from other character pools.
- Generated cards should usually gain Exhaust so they create one-turn decisions and feed Exhaust-pile synergies instead of becoming permanent value loops.
- Random card use should generally create playable options in hand, not replace player decisions with autoplay chains.
- Keep card text concise: do not repeat Attack Intent timing or PrismaticShard generated-card rules on every card.
- Card descriptions use `[gold]...[/gold]` for important referenced terms such as Attack Intent, other-character cards, and Exhaust pile. Prefer short visible terms plus hover tips for long explanations.
- The card reward pool uses Ironclad as the base plus Prism cards.
- `PrismaticShard` adds Prism cards to rewards, gives generated cards Exhaust, and discounts generated other-character cards by 1 for the turn they are created.
- The meaning of "other-character card" is centralized in `PrismRandomCardHelper`. Cards that add other-character cards should use `AddPlayableOtherCharacterCardToHand` so attack/skill/common subfilters are applied inside the playable other-character pool rather than rebuilding that pool locally.
- The other-character helper is intentionally broad but excludes cards that are poor or unsafe when randomly generated: multiplayer-only cards, all Powers except `Stone Armor`, Necrobinder `OstyAttack` cards, Necrobinder `Sacrifice`, Defect Focus-only cards such as `Focused Strike`, `Hotfix`, and `Synchronize`, Defect Evoke cards except `Shatter`, `Grand Finale`, `Knife Trap`, and future-only/low-immediate-value cards such as `Hidden Cache`, `Convergence`, `Outmaneuver`, `Invoke`, `Scavenge`, and `Prolong`.
- Some ancient/boss interactions are custom patched for Prism.
- Use `design/08_sts2_balance_benchmarks.md` as the baseline when rebalancing Prism against the base STS2 characters.

## Current Prism Card Map

This map describes the implemented card pool as of the current source state. Use the C# files under `src/PrismModCode/Cards/` as the source of truth.

Core mechanical axes:

- Other-character card generation is intentionally split by role now:
  - `Field Procurement`: baseline random playable other-character card.
  - `Secret Procurement`: choose 1 of 3 playable other-character cards, plus small Block.
  - `Borrowed Moment`: 0-cost playable other-character card generation that gives the generated card Retain.
  - `Shard Rush`: attack tempo; deals damage, adds a playable other-character Attack, and makes it cost 0 this turn.
  - `Borrowed Fangs`: higher-damage multi-hit attack that adds a playable other-character Attack without the free-this-turn tempo rider.
  - `Mixed Signals`: adds any playable other-character card, then gives a different payoff based on the generated card type.
  - `Common Rummage`, `Peak of Folly`, and `GentAndFect`: narrower generator packages with rarity or resource riders.
  - `Buried` and `Prismatic Cover` were moved away from direct generation to reduce duplicate generator roles.
- Exhaust-pile and autoplay swings: `Hidden Card`, `Archmage's Rune`, `Prism Discharge`, `Shard Salvage`, `Rift Storage`, `Pulsate`.
- Ironclad/Silent basic-card generation: `Radiant Gamble` adds a basic card from the `Two Characters` keyword. The keyword hover explains that this means Ironclad and Silent. The implementation excludes Strike/Defend-tagged cards.
- 2-cost-or-more payoffs: `Big Shard`, `Scattered Strike`, `Light Shard Shield`, `Heavy Refraction`, `Overcharged Lens`, `Lens Runaway`.
- Attack Intent package: `Spark of Intent`, `Sharp Afterimage`, `Forced Flare`, `Infinite Coil`, plus payoffs/modifiers from `Mirror Screen`, `Prismatic Brand`, `Delayed Amplification`, `Lens Runaway`.
- Card-generation/exhaust trigger powers: `Kaleidoscope Heart`, `Shard Furnace`, `External Refraction`, `Borrowed Orbit`.
- Prism Beam package: `Prism Beam` deals high all-enemy damage and doubles for each other `Prism Beam` the player has.

Important current card notes:

- `Reinforce` and `Guard` are temporary basic cards and should be reduced toward decompiled base-character starter benchmarks.
- `Prism Whirlwind` has been reworked away from random autoplay and card generation. Current behavior: immediately attack one target for 5x3, then create Attack Intent that applies 1 Weak to that target next turn.
- `Buried` is now an Exhaust-pile defensive common: Block, plus more Block if the Exhaust pile has any card.
- `Prismatic Cover` is now a hand support defensive common: Block, then reduce other-character cards already in hand by 1 this turn.
- `AttackIntentPower` resolves as a single hit. Do not reuse Prism Whirlwind's `Repeat=3` for Attack Intent execution unless intentionally rebalancing it as multi-hit.
- `Radiant Gamble` was reworked into a 0-cost starter card that adds one Ironclad/Silent basic non-Strike/non-Defend card to hand. If Radiant Gamble is upgraded, the generated basic card is upgraded. Do not put Exhaust on the starter card itself; generated cards already receive Exhaust from `PrismaticShard`, and generated other-character cards already cost 1 less that turn.
- `Prism Beam` was removed from the starting deck because its 3-cost exponential all-enemy scaling belongs in rewards, not the opening deck.
- `Prism Discharge` is rare. It plays X+bonus cards from the Exhaust pile and is too swingy for common rewards.
- `Prism Beam` has exponential scaling through duplicate copies; treat it as a major balance risk.
- `Dopamine`, `Hidden Card`, and `Archmage's Rune` are the biggest random/autoplay volatility risks.
- Several card folders do not match runtime rarity. Trust `CardRarity` in constructors, not directory names.

Current starting deck:

```text
Reinforce x4
Guard x4
Radiant Gamble x1
Prism Whirlwind x1
```

Planned starting-deck direction:

```text
Keep testing Radiant Gamble's Ironclad/Silent basic-card pool and Prism Whirlwind's Attack Intent draw pacing before treating the starting deck as final.
```

Recent balance/UI notes:

- Conditional bonus cards should override `ShouldGlowGoldInternal` when their bonus is active. This is now wired for 2-cost hand payoffs, generated-card payoffs, Exhaust-turn payoffs, Attack Intent payoffs, and Exhaust-pile followups.

## Known Combat Issues

- Attack Intent is implemented by `AttackIntentPower`.
- All-enemy Attack Intent used to execute once per all-enemy power instance, causing the all-enemy damage command to fire repeatedly when multiple all-enemy intent instances existed.
- Current intended behavior: at energy reset, all all-enemy Attack Intent instances are aggregated into one all-enemy hit sequence, then all aggregated instances are removed.

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
