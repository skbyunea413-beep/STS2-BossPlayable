# 10. Prism Next 10 Card Plan

Snapshot date: 2026-05-13.

This is the next planned Prism batch after the deep audit in
`design/09_prism_deep_card_audit_plan.md`.

The goal is not to add more generic "get a playable other-character card"
effects. Prism already has enough of those, and Prismatic Shard makes them
stronger than their text first suggests. This batch instead adds payoffs,
finishers, and mid-rarity build anchors.

## Batch Shape

Target additions:

- Rare Attack: 4
- Rare Skill: 1
- Uncommon Power: 4
- Uncommon Attack: 1

Avoid adding common cards in this batch unless one of the current common
generators is moved upward or removed. The common pool already has several
efficient attacks, conditional blocks, and card-generation tools.

## Planned Cards

### 1. Stop Loss / 손절매

- Rarity: Rare
- Type: Attack
- Cost: 2
- Target: Any enemy
- Role: Attack Intent finisher
- Draft purpose: Gives Attack Intent decks a rare attack that ends fights
  instead of only storing delayed damage.
- Text draft: Deal 16 damage. Trigger your Attack Intent effects against this
  enemy, then reduce those Attack Intent values by 50%.
- Upgrade: Damage 16 -> 20.

Balance note:

This should not fully double stored Attack Intent for free. Reducing stored
values after triggering makes it a conversion card, not a pure multiplier.

### 2. Limit Up / 상한가

- Rarity: Rare
- Type: Attack
- Cost: 3
- Target: Any enemy
- Role: Big single-target rare attack
- Draft purpose: Prism lacks a clean rare attack that rewards using generated
  cards without generating more cards itself.
- Text draft: Deal 24 damage. For each other-character card you played this
  combat, deal 3 additional damage.
- Upgrade: Additional damage 3 -> 4.

Balance note:

This rewards the off-class engine but does not add another off-class card to
hand. It is intentionally single-target so it does not replace Prism Beam or
Exhaust Flare.

### 3. Market Crash / 시장 붕괴

- Rarity: Rare
- Type: Attack
- Cost: 2
- Target: All enemies
- Role: Exhaust-pile finisher
- Draft purpose: Gives exhaust decks a rare attack payoff that is different
  from the linear uncommon `Exhaust Flare`.
- Text draft: Deal 8 damage to all enemies. Repeat once if your exhaust pile
  has 8 or more cards.
- Upgrade: Damage 8 -> 10.

Balance note:

This is a threshold payoff, not direct scaling. That keeps it from simply
being a better `Exhaust Flare` at every stage.

### 4. Leverage / 레버리지

- Rarity: Rare
- Type: Attack
- Cost: 2
- Target: Any enemy
- Role: 2-cost-or-more support attack
- Draft purpose: Indirectly supports Prism Beam without naming it. If the deck
  has Prism Beam, this can find it; if not, it still supports any expensive
  card package.
- Text draft: Deal 14 damage. Put a random card that costs 2 or more from your
  draw pile into your hand. If you cannot, draw 1 card.
- Upgrade: Damage 14 -> 18.

Balance note:

Direct Prism Beam support becomes dead text when Prism Beam is not drafted.
This version is a high-cost-card tutor/fallback draw instead.

### 5. Buyback / 환매

- Rarity: Rare
- Type: Skill
- Cost: 2
- Target: Self
- Role: Controlled exhaust-pile selection
- Draft purpose: Offers a less random alternative to `Radiate`.
- Text draft: Choose a card from your exhaust pile and put it into your hand.
  It costs 0 this turn. Exhaust.
- Upgrade: Cost 2 -> 1.

Balance note:

This is strong because it is selection plus discount. Exhaust on the card
itself is required. It should not also draw, block, or generate off-class cards.

### 6. Shard Furnace

- Rarity: Uncommon
- Type: Power
- Cost: 1
- Target: Self
- Role: Exhaust support anchor
- Draft purpose: Move/rework current `ShardFurnace` out of rare-power crowding
  and make it a normal archetype support card.
- Text draft: The first time a card is exhausted each turn, gain 4 block.
- Upgrade: Block 4 -> 6.

Balance note:

This is intentionally simpler and weaker than the current rare-like draw plus
block engine. If it also draws, it becomes rare or needs cost 2.

### 7. Diversify / 분산 투자

- Rarity: Uncommon
- Type: Power
- Cost: 1
- Target: Self
- Role: Other-character defensive support
- Draft purpose: Gives off-class decks a modest payoff that does not generate
  more cards.
- Text draft: The first time you play an other-character card each turn, gain
  5 block.
- Upgrade: Block 5 -> 7.

Balance note:

This is the fair uncommon version of `External Refraction`. No draw attached.

### 8. Bearish Signal / 하락 신호

- Rarity: Uncommon
- Type: Power
- Cost: 1
- Target: Self
- Role: Attack Intent support
- Draft purpose: Makes Attack Intent decks draftable before rare powers appear.
- Text draft: The first time you create Attack Intent each turn, apply 1 Weak
  to the target.
- Upgrade: Also deal 3 damage to the target.

Balance note:

This improves survivability and target pressure without directly multiplying
Attack Intent numbers. It should not stack too explosively.

### 9. Safe Asset / 안전자산

- Rarity: Uncommon
- Type: Power
- Cost: 1
- Target: Self
- Role: 2-cost-or-more defensive support
- Draft purpose: Indirectly supports Prism Beam and the existing heavy-card
  package without requiring Prism Beam to appear.
- Text draft: Whenever you play a card that costs 2 or more, gain 5 block.
- Upgrade: Block 5 -> 7.

Balance note:

This overlaps slightly with `Lens Runaway`, but on a different axis: defense
instead of Attack Intent scaling.

### 10. Day Trade / 단타 매매

- Rarity: Uncommon
- Type: Attack
- Cost: 1
- Target: Any enemy
- Role: Other-character payoff attack
- Draft purpose: Replaces some pressure to add more "damage plus generate"
  cards.
- Text draft: Deal 8 damage. If you played an other-character card this turn,
  draw 1 card.
- Upgrade: Damage 8 -> 11.

Balance note:

This is a payoff for already having the engine online. It does not fetch a new
card, so it should not overlap with `Borrowed Fangs` or `Shard Rush`.

## Self-Check

### Overlap Check

- No card in this batch randomly generates an off-class card.
- Only `Perfect Refraction` adds a card to hand, and it pulls from exhaust
  instead of from the other-character pool.
- `Intent Breaker` and `Warning Color` both touch Attack Intent, but one is a
  rare finisher and the other is a small uncommon support power.
- `Spectrum Lance` and `Gravity Capacitor` support 2-cost-or-more cards instead
  of directly naming Prism Beam, so they are not dead when Prism Beam is absent.
- `Prism Guillotine` and `Borrowed Edge` both reward other-character cards, but
  one is a late-game damage finisher and the other is a small tempo attack.

### Power Budget Check

- Rare attacks get fight-ending roles, but none also generate cards.
- Uncommon powers are intentionally small, narrow, and build-defining.
- The rare skill is very powerful, so it has Exhaust and does not include block
  or draw.
- There are no autoplay cards in this batch, avoiding the Dopamine/Radiate/
  Pulsate recursion problem.

### Pool Health Check

After adding this batch, Prism would move from 55 cards to 65 cards:

- Rare attacks become meaningfully represented.
- Uncommon powers stop being nearly empty.
- Rare power crowding becomes less painful if `Shard Furnace` is moved down.
- Common card rewards remain stable instead of being flooded by more common
  generators.

### Implementation Risk Check

- Lowest risk: `Borrowed Edge`, `Shard Furnace`, `Borrowed Footwork`,
  `Beam Capacitor`.
- Medium risk: `Warning Color`, `Fracture Storm`, `Prism Guillotine`.
- Highest risk: `Intent Breaker`, `Spectrum Lance`, `Perfect Refraction`.

`Intent Breaker` needs careful interaction with existing Attack Intent power
storage. `Spectrum Lance` needs a combat-only Prism Beam count modifier instead
of making actual cards. `Perfect Refraction` needs a choose-from-exhaust UI.

## Recommended Implementation Order

1. Fix existing high-risk balance issues first:
   - Dopamine autoplay guard.
   - `Secret Procurement`.
   - `Shard Rush`.
   - common generator upgrades.
2. Implement the four low-risk cards.
3. Implement the medium-risk cards.
4. Implement the three high-risk cards after the supporting helper APIs are
   clear.
