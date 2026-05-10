# 08. STS2 Base Character Balance Benchmarks

This note summarizes the local Slay the Spire 2 baseline used to rebalance Prism.

Source basis:
- Local decompiled game model files under `C:\Users\nagis\Documents\STS2DecompTemp`.
- Prism implementation in this repository under `src/PrismModCode`.
- Snapshot date: 2026-05-10.

The purpose is not to copy the base game. The purpose is to keep Prism close to the same power budget while preserving its identity: straightforward attack pressure, generated cards from other jobs, and Exhaust-pile payoffs.

## Base Character Baseline

| Character | HP | Gold | Starting deck | Starter relic | Main early identity |
|---|---:|---:|---|---|---|
| Ironclad | 80 | 99 | 5 Strike, 4 Defend, Bash | Burning Blood | High HP, combat healing, direct attacks, vulnerable |
| Silent | 70 | 99 | 5 Strike, 5 Defend, Neutralize, Survivor | Ring of the Snake | Larger deck, larger turn 1 hand, weak, block, skill density |
| Defect | 75 | 99 | 4 Strike, 4 Defend, Zap, Dualcast | Cracked Core | Orb setup, turn 1 lightning, scaling through focus/orbs |
| Regent | 75 | 99 | 4 Strike, 4 Defend, Falling Star, Venerate | Divine Right | Stars as extra resource, burst setup, mixed debuffs |
| Necrobinder | 66 | 99 | 4 Strike, 4 Defend, Bodyguard, Unleash | Bound Phylactery | Low HP offset by Osty, summon/ally economy |
| Prism, current | 77 | 77 | 6 Reinforce, 6 Guard, Prism Whirlwind, Radiant Gamble | Prismatic Shard, Dingy Rug, Prismatic Gem | Random cross-pool card use, reward enchantments |

Observations:
- Normal characters start with 10 cards, except Silent at 12.
- Starter decks usually contain 8 to 10 plain Strike/Defend equivalents plus 1 to 2 identity cards.
- Starter relics are strong but narrow. They do not usually rewrite every card reward or grant several independent starting bonuses.
- Base characters use 99 starting gold. Prism currently uses 77, but the reduced gold does not offset its reward and autoplay upside.

## Card Pool Shape

| Pool | Cards | Basic | Common | Uncommon | Rare | Ancient | Attack | Skill | Power |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Ironclad | 87 | 3 | 20 | 36 | 26 | 2 | 37 | 30 | 20 |
| Silent | 88 | 4 | 20 | 36 | 26 | 2 | 28 | 41 | 19 |
| Defect | 88 | 4 | 20 | 36 | 26 | 2 | 29 | 39 | 20 |
| Regent | 88 | 4 | 20 | 36 | 26 | 2 | 32 | 37 | 19 |
| Necrobinder | 88 | 4 | 20 | 36 | 26 | 2 | 35 | 35 | 18 |
| Prism, current | 19 | 4 | 7 | 2 | 3 | 3 | 3 | 13 | 3 |

Base game pools are extremely regular:
- Common count is 20.
- Uncommon count is 36.
- Rare count is 26.
- Ancient count is 2.
- Basic count is 3 or 4.

Prism's current pool is much smaller and much more volatile. A small pool is not automatically bad, but it makes every overpowered card appear more often. It also means reward injection through Prismatic Shard has an outsized effect.

## Cost Curve

| Pool | X cost | 0 | 1 | 2 | 3 | 4+ |
|---|---:|---:|---:|---:|---:|---:|
| Ironclad | 1 | 12 | 48 | 19 | 7 | 0 |
| Silent | 0 | 17 | 44 | 17 | 10 | 0 |
| Defect | 0 | 17 | 48 | 16 | 6 | 1 |
| Regent | 0 | 21 | 49 | 13 | 4 | 1 |
| Necrobinder | 0 | 11 | 54 | 14 | 7 | 2 |

The center of gravity is 1 energy. Most pools contain:
- About 44 to 54 one-cost cards.
- About 13 to 19 two-cost cards.
- About 4 to 10 three-cost cards.
- Very few four-plus-cost cards.

For Prism, this matters because "play a random card" is not worth 1 energy. If the random card can come from all five base pools and can be autoplayed for free, it samples from hundreds of cards whose normal costs and build requirements are being bypassed.

## Starter Card Benchmarks

Base Strike/Defend equivalents:
- Strike: 1 energy, 6 damage, upgrades to 9.
- Defend: 1 energy, 5 block, usually upgrades to 8.

Identity starter examples:
- Bash: 2 energy, 8 damage, 2 vulnerable. Upgrade: +2 damage, +1 vulnerable.
- Neutralize: 0 energy, 3 damage, 1 weak. Upgrade: +1 damage, +1 weak.
- Survivor: 1 energy, 8 block with discard behavior. Upgrade: +3 block.
- Zap: 1 energy, channel 1 Lightning. Upgrade: cost reduced to 0.
- Dualcast: 1 energy, evoke front orb twice. Upgrade: cost reduced to 0.
- Falling Star: 0 energy, 8 damage, 1 weak, 1 vulnerable. Upgrade: +4 damage.
- Venerate: 1 energy, gain 2 Stars. Upgrade: +1 Star.
- Bodyguard: 1 energy, summon 5. Upgrade: +2 summon.
- Unleash: 1 energy, Osty attack. Upgrade: +3 base calculation.

Implication for Prism:
- `Reinforce` at 1 energy / 6 damage is normal.
- `Guard` at 1 energy / 5 block is normal, though its +2 upgrade is weaker than most base Defends.
- `RadiantGamble` as a starting deck card is not normal. It converts 3 energy into free cross-pool autoplay with a cost-total threshold, which creates very high variance and can bypass intended setup costs.
- A Prism starter identity card should probably reinforce the attack-pressure plan or create one controlled off-class card, not autoplay a chain.

## Current Prism Balance Problems

1. Random card source is too broad.

`PrismRandomCardHelper.AllRandomCardOptions` currently draws from all character card pools, colorless cards, and the Prism pool. That gives Prism access to nearly the entire game card space in combat.

2. Autoplay skips normal costs and deck-building requirements.

Cards such as `MixedSignals`, `BorrowedFangs`, `PrismWhirlwind`, `RadiantGamble`, `HiddenCard`, `ArchmagesRune`, `Pulsate`, and `Radiate` can play generated or random cards without paying their printed energy costs. The generated cards also bypass normal draw, retain, and hand-size friction.

3. Autoplay can feed trigger engines.

`DopaminePower.AfterCardPlayed` does not check `cardPlay.IsAutoPlay`. If autoplayed cards count as played cards, Dopamine can generate more free cards from other free-card engines.

4. Starter relic value is too high and too global.

`PrismaticShard` originally:
- Adds Prism cards to eligible card reward pools.
- Adds a random enchantment to every card reward.
- Applies 1 to 3 stacks.

That was closer to a run-defining boss relic or special modifier than a normal starter relic. The current direction is to keep reward-pool injection, make generated cards gain Exhaust, and make generated off-class cards cost 1 less only on the turn they are created.

5. Rarity distribution is inconsistent.

Some cards live in rare-oriented folders but are declared as `CardRarity.Common`. That makes rewards noisier and makes powerful effects appear earlier and more often than intended.

6. Prism has multiple starter relics.

Base characters start with one starter relic. Prism starts with `PrismaticShard`, `DingyRug`, and `PrismaticGem`. This is a large baseline advantage even before card text is considered.

## Balance Rules For Prism

Use these rules before changing individual numbers:

0. Prism's main plan is direct pressure.

The deck should be able to win by attacking consistently. Off-class cards should broaden lines, patch weaknesses, or create burst windows; they should not be the only reason the character functions.

0.5. Generated cards are temporary fuel.

Generated cards should usually gain Exhaust. This keeps off-class generation from becoming permanent value, makes the discounted turn matter, and gives Prism a real Exhaust-pile subtheme.

1. Cross-pool access should be priced as premium utility.

Adding a random off-class card to hand is usually safer than autoplaying it. If it is autoplayed, it should be limited by rarity, cost, type, or count.

2. Generated off-class cards should normally not trigger Prism engines.

Autoplay should not recursively trigger Dopamine-like "whenever you play a card" effects unless the card explicitly says so.

3. Starter deck should teach the mechanic without high-roll chains.

A reasonable starter shape:
- 4 to 5 attacks.
- 4 to 5 blocks.
- 1 controlled identity card.
- 10 to 11 total cards.

4. Starter relic should do one thing.

Good starter relic directions:
- Add occasional Prism cards to rewards.
- Lightly improve generated off-class cards.
- Or provide a small combat-start cross-pool option.

It should not both change reward pools and enchant every reward.

5. Use the base pool rarity shape as the default target.

For a small early Prism pool, a stable target is:
- Basic: 3 to 4.
- Common: at least 8 to 12 before expanding.
- Uncommon: at least 8 to 12.
- Rare: 5 to 8.
- Ancient: 1 to 2.

As the pool grows, move toward the base game's 20 / 36 / 26 / 2 shape.

## Recommended First Pass

High-confidence changes:

1. Add `cardPlay.IsAutoPlay` guard to `DopaminePower`.
2. Remove `PrismCardPool` from generic random card generation, or only include Prism cards through explicit Prism-card effects.
3. Keep `PrismaticShard` reward-pool injection, but replace reward enchantments with "generated cards gain Exhaust; generated other-character cards cost 1 less this turn."
4. Avoid permanent or combat-long discounts until Prism's card generation volume is under control.
5. Replace starter `RadiantGamble` with a safer starter identity card.
6. Remove either `Innate` or `Retain` from `Pulsate`; remove `Innate` first.
7. Normalize declared rarities for `GentAndFect`, `GhostNGoblins`, and `PeakOfFolly`.

Suggested design direction:
- Prism should feel like a stubborn attacker who refracts other jobs' techniques into the current fight.
- The player should choose or receive off-class cards, then use Prismatic Shard's one-turn discount to convert them into tempo.
- Prism should not be strongest when the player stops making choices and lets autoplay resolve the fight.
