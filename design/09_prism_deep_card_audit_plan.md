# 09. Prism Deep Card Audit And Expansion Plan

This is a working balance audit for Prism after the first broad card rework.

Source basis:
- Current Prism source under `src/PrismModCode/Cards/`.
- Decompiled STS2 card pools under `C:\TEMP\sts2_decompile\current_sts2`.
- Snapshot date: 2026-05-13.

## Main Diagnosis

Prism has not been evaluated deeply enough card-by-card. Recent testing exposed this clearly: `Borrowed Fangs` was still a 1-cost 14 damage card that also generated a playable off-class Attack, which was far above normal uncommon power budget.

The current problem is not one isolated overtuned card. It is the interaction of four structural issues:

1. Prism has a smaller card pool than base characters, so each mistake appears more often.
2. Many cards combine a normal-rate effect with off-class generation, discounting, or autoplay.
3. The card pool has too few rare attacks and too few uncommon powers.
4. Some axes overlap: multiple cards generate playable off-class cards, multiple cards are 2-cost hand payoffs, and multiple cards are small conditional block/attack cards.

## Pool Comparison

Base character pools are roughly 87-88 cards:

| Character | Total | Powers | Uncommon Powers | Rare Powers | Rare Attacks |
|---|---:|---:|---:|---:|---:|
| Ironclad | 87 | 19 | 8 | 11 | 7 |
| Silent | 88 | 18 | 8 | 10 | 5 |
| Defect | 88 | 19 | 10 | 9 | 8 |
| Necrobinder | 88 | 17 | 9 | 8 | 8 |
| Regent | 88 | 18 | 8 | 10 | 9 |
| Prism | 55 | 8 | 1 | 7 | 1 |

Prism's power count is low overall, but its power rarity is wrong: almost all powers are rare. The character lacks the middle layer of build-direction powers that base characters use to make archetypes draftable before rare rewards.

Prism also has only one rare attack. This is both a balance/variety issue and a shop-generation issue, because normal merchants create fixed Attack/Attack/Skill/Skill/Power colored card slots and can roll rare attacks.

## Current Card Audit

### Starter And Basic

- `Reinforce`: fine. Standard Strike rate: 1 energy, 6 damage, upgrades to 9.
- `Guard`: slightly weak upgrade. Base 5 block is standard, but upgrade to 7 is below the usual 8 block Defend benchmark. Candidate: upgrade +3 instead of +2.
- `Radiant Gamble`: risky but acceptable as the special starter if the generated pool stays Ironclad/Silent basic non-Strike/non-Defend. Watch for strong basic cards becoming too common with Prismatic Shard discount and Exhaust.
- `Prism Whirlwind`: strong starter identity. 2 energy 5x3 single target plus next-turn Weak is high, but it is the main starter identity and no longer generates cards. Keep watching.

### Common Attacks

- `Big Shard`: 1 energy 8 damage, 13 if a 2-cost card is in hand. Probably high but acceptable because the condition asks for hand shape. Watch early act consistency.
- `Prismatic Brand`: 1 energy 7 damage, plus Weak if Attack Intent exists. Fine.
- `Prism Beam`: must stay common because its identity is collection scaling. 3 energy 19 all-enemy damage, doubled by other copies. Major long-term balance risk; do not make rare.
- `Rescue Strike`: 1 energy 7 damage, 12 if a card was generated this turn. Similar to Big Shard. Acceptable but overlaps with Big Shard's role.
- `Scattered Strike`: 1 energy 9 damage, draws if 2-cost card in hand. High for common when active, but condition is visible and deckbuilding-oriented.
- `Shard Finder`: 1 energy 6 damage, applies Vulnerable if another-character card is in hand. Fine but can become very efficient with starter generated card.
- `Shard Rush`: currently dangerous. 1 energy 8 damage, generates a playable off-class Attack, and makes it cost 0 this turn. This is a common that can create large tempo spikes. Candidate: move to uncommon, remove cost-to-0, or reduce damage to 5-6.

### Common Skills

- `Angular Cover`: 1 energy 7 block and discounts a random hand card by 1. Strong common utility, probably acceptable but overlaps with Prismatic Cover and Chromatic Aberration.
- `Buried`: 1 energy 6 block, 12 if exhaust pile has a card. Fine. Upgrade +3/+3 may make it 18 when active, but the condition is not turn-1 free.
- `Common Rummage`: 1 energy, generates a random common other-character card. If it cannot be played, draw 1. Upgrade now reduces cost to 0 instead of doubling generation.
- `Faceted Guard`: 1 energy 7 block, 11 if a card exhausted this turn. Fine.
- `Field Procurement`: 1 energy, generates any playable other-character card. This is the baseline generator. Upgrade to 2 cards is likely too much at common.
- `Light Shard Shield`: 1 energy 9 block, 13 if 2-cost card in hand. High common block, but condition is hand-shape. Compare with Defend+ at 8.
- `Mirror Screen`: 1 energy 8 block, 12 with Attack Intent. Fine.
- `Prismatic Cover`: 1 energy 7 block, discounts all other-character cards in hand by 1 this turn. This can be strong but only works after generation. Keep as support, but it overlaps with Angular Cover/Chromatic Aberration.
- `Secret Procurement`: 1 energy choose 1 of 3 playable other-character cards plus 3 block. Too good for common because selection is much stronger than random. Candidate: uncommon, or cost 2, or remove block.
- `Sharp Afterimage`: 0 energy draw 1, Exhaust, Attack Intent 4 damage. Strong common cantrip but acceptable as an Attack Intent enabler. Watch with Lens Runaway.
- `Scar`: 1 energy, lose 3 HP, add 3 Shivs. Probably strong but self-damage is real; does not directly use Prism identity.

### Uncommon Attacks

- `Borrowed Fangs`: now 2 energy 7x2 and generate playable other-character Attack. This is still strong but no longer obviously broken. Candidate: keep, or reduce generated Attack discount interaction by not forcing additional discount.
- `Exhaust Flare`: 1 energy 5 all-enemy plus exhaust-pile count. Fine scaling uncommon.
- `Forced Flare`: currently declared rare. 2 energy 10 all-enemy now and 10 all-enemy Attack Intent later. This is a rare-level attack and is a reasonable temporary rare attack candidate.
- `Heavy Refraction`: 2 energy 14 all-enemy, can become 1 cost after playing a 2-cost card. Strong but coherent with 2-cost archetype.
- `Spark Of Intent`: 1 energy 9 damage plus 9 Attack Intent later. High for uncommon but delayed damage can be disrupted by target death.

### Uncommon Skills

- `Afterglow Compression`: 1 energy draw 1 and discount a random 2-cost hand card. Upgrade to 0 cost may be strong but acceptable for uncommon.
- `Borrowed Moment`: 0 energy generate a random other-character card and give Retain. Less immediate than before, but still risky because upgrade can generate 2 retained discounted cards.
- `Chromatic Aberration`: 1 energy 8 block and discount an other-character card. Fine.
- `Debris Reaction`: 1 energy 7 block, if a card exhausted this turn apply 1 Weak to all. Fine.
- `Delayed Amplification`: 1 energy increase all Attack Intent by 5 and draw 1. Strong support, likely okay because it requires Attack Intent already banked.
- `Mixed Signals`: generate random other-character card, then type-based payoff. Interesting and differentiated. Watch complexity; can be swingy.
- `Overcharged Lens`: 1 energy temporary power-like skill: first 2-cost card this turn draws 1. Fine, but maybe should be a power if the effect becomes persistent.
- `Peak Of Folly`: changed to 3 energy, 3 playable common off-class cards and 2 Rocks. Upgrade now raises cards to 4 instead of reducing Rocks.
- `Prism Rearrangement`: 0 energy exhaust an other-character card to generate another random other-character card, Exhaust. Fine as cycling, now less deterministic.
- `Prism Refraction`: 1 energy Retain, generate Prism Beam. Supports common beam package. Fine but must be watched with Prism Beam scaling.
- `Recycled Guard`: 1 energy 8 block, if exhaust pile has cards generate random other-character card. Strong but conditional. Candidate: uncommon is correct.
- `Refractive Distortion`: 1 energy apply 2 Weak, if generated this turn -2 Strength. Strong but conditional and single-target. Fine.
- `Rift Storage`: 1 energy retrieve random skill from exhaust, discount it. Fine.
- `Shard Salvage`: 1 energy Exhaust, retrieve random exhaust card, discount it. Strong and coherent.

### Rare Skills

- `Archmage's Rune`: 4 energy Exhaust, autoplay random cards until cost total reaches 10, upgrade 3 energy/15. Extremely volatile and potentially too strong or too slow depending roll. Should remain rare/ancient-like. Candidate: reduce randomness pool or exclude Prism/autoplay engines.
- `Radiate` / Prism Discharge: X, Retain, autoplay X+1/3 cards from exhaust. Very strong rare and likely correct rarity. Watch X with exhaust pile quality.
- `GentAndFect`: file is under Rare but constructor is Uncommon. Effect is random off-class Skill + 2 Stars + 1 orb slot for 1 energy. Still mechanically weird for Prism if orb slots are mostly off-class support.

### Rare Powers

- `External Refraction`: 2 energy, once per turn when playing other-character card gain 8 block and draw 1. Strong engine but could be uncommon at lower numbers.
- `GhostNGoblins`: 2 energy, every other-character play alternates damage and Osty summon. This is rare, but Osty interaction may be off-theme/unsafe.
- `Kaleidoscope Heart`: 1 energy, first generated card each turn gives 5 block and draw 1. Strong engine, rare is plausible.
- `Lens Runaway`: 2 energy, 2-cost cards increase Attack Intent damage by 3. Rare is plausible for Attack Intent scaling.
- `Pulsate`: 2 energy, Innate + Retain, end of turn autoplay random card(s). Very high volatility. If kept, remove either Innate or Retain; prefer remove Innate.
- `Shard Furnace`: 2 energy, first exhaust each turn draw 1 and block 5. This is an uncommon engine unless numbers are raised.
- `Vital Spark`: 1 energy, whenever attacked gain 1 energy next turn. This is defensive/resource utility and not currently exciting as rare. Candidate: uncommon, or rework rare version to do more.

### Ancient / Volatile Cards

- `Hidden Card`: 2 energy, autoplay random 2+ cost cards until cost total reaches 4. Volatile and dangerous, but ancient. Needs the same autoplay guard rules as Rune.
- `Dopamine`: ancient power. Current `DopaminePower` still triggers on autoplayed cards. This is a serious recursion/high-roll risk and should be fixed before adding more cards.
- `Ancient Prism Whirlwind`: ancient attack. Fine as a high-power boss/ancient upgrade fantasy, but keep out of normal reward/shopping unless intended.

## High Priority Balance Fixes Before Adding Many Cards

1. Add an autoplay guard to `DopaminePower`.
   - It currently triggers on every card play by the player, including autoplay.
   - This can cascade with `Pulsate`, `Hidden Card`, `Archmage's Rune`, and `Radiate`.

2. Revisit common generators with upgrades.
   - `Field Procurement+` and `Common Rummage+` generating 2 playable discounted off-class cards may be too much for common.
   - Suggested: upgrades improve selection/quality or add block, not double generation count.

3. Move `Secret Procurement` to uncommon or nerf it.
   - Choose 1 of 3 playable off-class cards is a premium effect.

4. Decide what `Shard Rush` is.
   - If common: remove the generated card's forced 0-cost.
   - If uncommon: keep the tempo rider and maybe lower damage.

5. Reduce rare-power crowding by moving 1-2 cards down or reworking.
   - `Shard Furnace` to uncommon is the cleanest.
   - `Vital Spark` to uncommon or rework as a more dramatic rare.
   - `External Refraction` can stay rare if numbers remain high, or become uncommon at lower values.

6. Fix rarity/file mismatches.
   - Runtime constructor is source of truth, but mismatched folders are confusing during balance passes.
   - `GentAndFect` and `ForcedFlare` are notable mismatches.

## Expansion Targets

Short-term target: grow Prism from 55 cards toward 65-68 cards.

Additions:
- Rare Attack +3 or +4.
- Rare Skill +1 or +2.
- Uncommon Power +4 or +5.
- Uncommon Attack +2.
- Common cards only if they fill a truly missing early role.

Do not add more generic other-character card generators until existing ones are differentiated.

## Proposed New Card Roles

### Rare Attacks

1. Attack Intent finisher.
   - Example role: immediately trigger stored Attack Intent against the target, then deal damage.
   - Should not generate cards.

2. Exhaust-pile finisher.
   - Example role: deal all-enemy damage scaling with exhaust pile, then maybe exhaust a card from hand.
   - Must not be a better `Exhaust Flare` at all stages; it should be rare because it changes endgame scaling.

3. Other-character payoff attack.
   - Example role: damage increases for each other-character card played this combat, or repeats if you played one this turn.
   - Rewards the generator engine without adding another generated card.

4. Prism Beam support rare attack.
   - Example role: deal damage and temporarily count as an extra Prism Beam for this combat, or duplicate a Prism Beam in hand with Exhaust.
   - Must preserve Prism Beam as the common collectible.

### Rare Skills

1. Controlled exhaust-pile selection.
   - Choose a card from exhaust to return and set to 0 this turn, then Exhaust.
   - Strong but player-driven, less random than Radiate.

2. Defensive turn-sculpting.
   - Gain block based on generated/exhausted cards and retain or discount one card.
   - Should not generate more off-class cards.

### Uncommon Powers

1. Exhaust support power.
   - The first time a card exhausts each turn, gain small block or draw next turn.
   - `Shard Furnace` can become this.

2. Other-character support power.
   - The first other-character card each turn gives small block or small damage.
   - Lower-power version of `External Refraction`.

3. Attack Intent support power.
   - The first Attack Intent created each turn gains +N or applies a small debuff.
   - Avoid compounding too fast with Lens Runaway.

4. Prism Beam support power.
   - When you play Prism Beam, gain a small defensive/resource bonus.
   - Do not duplicate Prism Beam for free every turn.

## Self-Check Rules For Future Cards

Before adding or buffing a card, answer these:

1. Does this card already pay for itself before the Prism-specific rider?
   - If yes, the rider must be small or conditional.

2. Does it generate a playable off-class card?
   - If yes, remember Prismatic Shard already adds Exhaust and a one-turn discount.

3. Does it autoplay random cards?
   - If yes, it must not trigger recursive engines unless explicitly intended.

4. Is the card common?
   - Common cards should teach one axis, not solve a turn by themselves.

5. Does it overlap with an existing card?
   - If it is another "gain block + generate card" or "damage + generate card", redesign it.

6. Would this break merchant generation if it is the only card of its rarity/type?
   - Prism needs enough rare attacks and enough power distribution for shops.

## Recommended Next Work Order

1. Fix `DopaminePower` autoplay recursion.
2. Nerf/re-rarity `Secret Procurement`, `Shard Rush`, and generator upgrades.
3. Move/rework `Shard Furnace` and possibly `Vital Spark` into uncommon power space.
4. Add three rare attacks with distinct roles.
5. Add two uncommon powers.
6. Re-run a shop-entry test and two early-act runs.
