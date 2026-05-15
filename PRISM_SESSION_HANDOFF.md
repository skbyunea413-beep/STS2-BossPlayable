# Prism Session Handoff

Use this document when a Codex/AI session has grown too long and a fresh
session needs to continue Prism work without rediscovering the project.

## Start Here

This repository is `STS2 Boss Playable`, a Slay the Spire 2 mod project.
The active playable entry is Prism, implemented directly under `src/` as
`PrismMod`.

At the start of a new session:

1. Read `PROJECT_CONTEXT.md`.
2. Read this file.
3. Check the worktree with:

```powershell
git status --short
```

4. Build before and after meaningful code changes:

```powershell
dotnet build src/PrismMod.csproj
dotnet publish src/PrismMod.csproj
```

Do not assume the working tree is clean. There may be large uncommitted changes
from previous Prism work.

## Local Reference Paths

Current workspace:

```text
C:\Users\nagis\Documents\STS2-PrismMod
```

Current decompile references:

```text
C:\TEMP\sts2_decompile\current_sts2
C:\TEMP\baselib_decompile
```

Use the local Prism code first. When behavior is unclear, compare against
decompiled vanilla STS2 cards, powers, commands, and UI nodes.

## Dependencies

Use the stable versions already referenced by the project unless explicitly
asked to update:

```text
BaseLib 3.1.2
STS2-RitsuLib 0.2.28
```

Important build commands:

```powershell
dotnet build src/PrismMod.csproj
dotnet publish src/PrismMod.csproj
```

The publish step copies outputs into the local Slay the Spire 2 mods folder.

## Mod Architecture

Important files:

```text
src/PrismModCode/MainFile.cs
src/PrismModCode/Character/PrismCharacter.cs
src/PrismModCode/Character/PrismCardPool.cs
src/PrismModCode/Cards/PrismCard.cs
src/PrismModCode/Cards/PrismRandomCardHelper.cs
src/PrismModCode/Relics/PrismaticShard.cs
src/PrismModCode/Powers/
src/PrismModCode/Patches/
src/PrismMod/localization/eng/cards.json
src/PrismMod/localization/kor/cards.json
```

`MainFile.Initialize()` registers Prism content through RitsuLib content-pack
scaffolding. New cards, powers, relics, epochs, and character content must be
registered there or they will not appear.

Prism cards inherit from:

```csharp
public abstract class PrismCard : CustomCardModel
```

Prism powers inherit from:

```csharp
public abstract class PrismPower : CustomPowerModel
```

## Card Writing Pattern

Typical Prism card structure:

```csharp
public sealed class ExampleCard : PrismCard
{
    public override string? CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/examplecard.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new BlockVar(5m, ValueProp.Move),
        new CardsVar(1),
    ];

    public ExampleCard() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        System.ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(ctx);
    }

    protected override void OnUpgrade() => base.DynamicVars.Damage.UpgradeValueBy(3m);
}
```

Common command patterns:

```csharp
await DamageCmd.Attack(amount).FromCard(this).Targeting(target).Execute(ctx);
await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
await CardPileCmd.Draw(ctx, base.DynamicVars.Cards.BaseValue, base.Owner);
await PowerCmd.Apply<MyPower>(ctx, target, amount, source, this);
await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
```

For generated other-character cards, prefer helpers in
`PrismRandomCardHelper` instead of rebuilding pools locally:

```csharp
await PrismRandomCardHelper.AddPlayableOtherCharacterCardToHand(ctx, base.Owner);
await PrismRandomCardHelper.ChoosePlayableOtherCharacterCardToHand(ctx, base.Owner, 3);
```

## Localization And Green Upgrade Numbers

Prism card text is in:

```text
src/PrismMod/localization/eng/cards.json
src/PrismMod/localization/kor/cards.json
```

Use `:diff()` for any numeric dynamic variable that should glow green in the
upgrade preview:

```text
Deal {Damage:diff()} damage.
Gain {Block:diff()} [gold]Block[/gold].
Draw {Cards:diff()} card.
```

This relies on vanilla STS2 upgrade-preview behavior:

- `OnUpgrade()` calls `DynamicVar.UpgradeValueBy(...)`.
- The dynamic var sets `WasJustUpgraded`.
- `GetDescriptionForUpgradePreview()` renders `{Var:diff()}` through
  `StsTextUtilities.HighlightChangeText(...)`.

Avoid directly mutating upgradeable values with `BaseValue += ...` unless there
is a very specific reason, because that may skip the `WasJustUpgraded` path.

Existing debug command:

```text
prism_upgrade_debug
```

It checks `Reinforce` upgrade-preview diagnostics.

## Prism Design Rules

Prism identity:

- Direct attack pressure.
- Generated cards from other character pools.
- Generated cards usually gain Exhaust through `PrismaticShard`.
- Generated other-character cards cost 1 less for the turn they are created.
- Exhaust-pile payoffs.
- Attack Intent delayed damage/debuffs.
- High-cost card payoffs and Prism Beam scaling.

Important design guardrails:

- Do not add more generic "get a playable other-character card" cards unless
  there is a strong reason. Prism already has many.
- Generated-card effects are stronger than they look because of
  `PrismaticShard`.
- Common cards should teach one axis, not solve a whole turn by themselves.
- Autoplay effects are dangerous and should avoid recursive engines.
- Card text should stay short; explain shared mechanics through keywords and
  hover tips rather than repeating long rules on every card.
- Constructor `CardRarity` is the runtime truth. Some file folders may not
  match rarity during rework.

## Current Design Documents

Read these when balancing or adding cards:

```text
design/08_sts2_balance_benchmarks.md
design/09_prism_deep_card_audit_plan.md
design/10_prism_next_cards.md
```

`design/09_prism_deep_card_audit_plan.md` diagnoses current pool problems.
`design/10_prism_next_cards.md` describes the next-card expansion batch.

## High Priority Known Work

Before adding more cards, check these issues:

1. `DopaminePower` autoplay recursion risk.
   - It currently reacts to card plays and may trigger from autoplayed cards.
   - This is dangerous with `Pulsate`, `HiddenCard`, `ArchmagesRune`, and
     `Radiate`.

2. `SecretProcurement` balance.
   - Common, 1-cost, choose 1 of 3 playable other-character cards plus block is
     likely too strong.

3. Generator upgrades.
   - Be careful with upgrades that double generated playable off-class cards.

4. `ShardFurnace`, `VitalSpark`, and rare-power crowding.
   - Some rare powers may belong at uncommon or need reworks.

5. Autoplay targeting.
   - Use `PrismCard.GetAutoPlayTarget`, `CanAutoPlayNow`, and
     `AutoPlayWithValidTarget` when random autoplay could target enemies.

## VFX State

The Prism mega beam VFX is being reworked with both C# and Godot assets:

```text
src/PrismModCode/Vfx/PrismMegaBeamVfx.cs
src/PrismModCode/Vfx/NPrismMegaBeamVfx.cs
src/PrismMod/scenes/vfx/prism_mega_beam.gd
src/PrismMod/scenes/vfx/prism_mega_beam.tscn
src/PrismMod/scenes/vfx/prism_mega_beam_workbench.tscn
src/PrismMod/scenes/vfx/prism_screen_transition_overlay.gd
src/PrismMod/images/vfx/
src/PrismMod/audio/sfx/ultra_heavy_laser.ogg
```

`NPrismMegaBeamVfx` wraps the Godot scene and exposes timing constants:

```csharp
internal const float ChargeDuration = 0.34f;
internal const float BeamDuration = 1.17f;
internal const float DamageImpactDelay = 0.86f;
```

The Godot script handles the beam drawing, charge sprites, external loop,
electric charge, screen transition overlay, and editor preview controls.

When changing VFX, verify:

- Scene path remains `res://PrismMod/scenes/vfx/prism_mega_beam.tscn`.
- The C# wrapper and Godot script agree on timing.
- Added assets are in `src/PrismMod/images/vfx/` or `src/PrismMod/audio/`.
- Build/publish still includes the resources.

## Testing And Debugging

Useful commands:

```powershell
dotnet build src/PrismMod.csproj
dotnet publish src/PrismMod.csproj
```

Useful in-game console commands:

```text
prism_architect
prism_orobas
prism_upgrade_debug
```

Game log:

```text
C:\Users\nagis\AppData\Roaming\SlayTheSpire2\logs\godot.log
```

## Working Tree Warning

The working tree may include many generated or uncommitted assets:

- Card portrait PNGs and `.import` files.
- New card C# files and `.uid` files.
- VFX sprites, Godot scenes, and scripts.
- Localization edits.
- Audio files.

Never revert unrelated user changes. If a file is dirty, assume it may be part
of the previous Prism work unless it is clearly unrelated.

## Best Next Action For A Fresh Session

If the user asks to continue Prism work without a specific target:

1. Run `git status --short`.
2. Run `dotnet build src/PrismMod.csproj`.
3. If the build fails, fix compile errors first.
4. If the build passes, inspect and fix `DopaminePower` autoplay recursion.
5. Then review `SecretProcurement` and the generator upgrade balance.
6. Only after those are stable, continue the next-card batch from `design/10`.

