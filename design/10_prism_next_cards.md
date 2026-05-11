# 10. Prism Next Card Plan

This note records the next Prism card batch using compact card text.

## Korean Text Rules

- Prefer short text that fits the card box.
- Use `피해를 X 줍니다`.
- Use official pile names: `뽑을 카드 더미`, `버린 카드 더미`, `소멸된 카드 더미`.
- Avoid bare `가져옵니다` when the source could be unclear.
- For off-class generation, use `다른 직업 카드 더미에서 ... 가져옵니다`.
- Do not repeat relic rules on every card:
  - generated cards gain Exhaust,
  - generated other-character cards cost 1 less this turn.
- Use `공격의도: <효과>` only. Do not write the next-turn reminder on every card.

## Source Wording

Use these patterns:

- `다른 직업 카드 더미에서 지금 낼 수 있는 무작위 카드를 1장 가져옵니다.`
- `다른 직업 카드 더미에서 지금 낼 수 있는 무작위 공격 카드를 1장 가져옵니다.`
- `소멸된 카드 더미에서 무작위 카드를 1장 가져옵니다.`
- `뽑을 카드 더미에서 공격 카드를 1장 뽑습니다.`

`뽑을 카드 더미` should mean the player's draw pile.  
`다른 직업 카드 더미` should mean Prism's temporary off-class generation pool.

## Next 10 Cards

| Card | Rarity | Type | Cost | Compact Korean text | Upgrade |
|---|---|---:|---:|---|---|
| 굴절 타격 | Common | Attack | 1 | 피해를 7 줍니다. 이번 턴 카드를 생성했다면 피해를 5 추가로 줍니다. | 피해 +3 / 추가 피해 +2 |
| 잔광 방어 | Common | Skill | 1 | 방어도를 7 얻습니다. 카드가 소멸된 적 있다면 방어도를 4 추가로 얻습니다. | 방어도 +3 |
| 드릴 예열 | Common | Skill | 0 | 카드를 1장 뽑습니다. 공격의도: 피해를 4 줍니다. 소멸. | 피해 +3 |
| 비틀린 조달 | Common | Skill | 1 | 다른 직업 카드 더미에서 지금 낼 수 있는 무작위 카드를 1장 가져옵니다. 방어도를 3 얻습니다. | 방어도 +3 |
| 파편 던지기 | Common | Attack | 1 | 피해를 6 줍니다. 다른 직업 카드가 있다면 취약 1을 부여합니다. | 피해 +3 |
| 렌즈 정렬 | Uncommon | Skill | 1 | 프리즘 광선을 1장 가져옵니다. 보존. | 비용 -1 |
| 강행 돌파 | Uncommon | Attack | 2 | 모든 적에게 피해를 10 줍니다. 공격의도: 모든 적에게 피해를 10 줍니다. | 피해 +3 |
| 젠트와펙트 | Uncommon | Skill | 1 | 다른 직업 카드 더미에서 지금 낼 수 있는 무작위 스킬 카드를 1장 가져옵니다. 별을 2 얻습니다. 구체 슬롯을 1개 얻습니다. | 별 +1 |
| 파편 회수 | Uncommon | Skill | 1 | 소멸된 카드 더미에서 무작위 카드를 1장 가져옵니다. 그 카드의 비용이 이번 턴 1 감소합니다. 소멸. | 비용 -1 |
| 마계촌 | Rare | Power | 2 | 다른 직업 카드를 낼 때마다 효과가 번갈아 발동합니다. 무작위 적에게 피해를 4 줍니다. 소환 2. | 비용 -1 |

## Notes

- `비틀린 조달` is intentionally plain. It is a stable common, not a payoff.
- `렌즈 정렬` supports the Prism Beam fallback plan without adding more random autoplay.
- `젠트와펙트` is a setup card for cards that need Stars or Orb Slots.
- `마계촌` should ignore autoplayed cards to avoid loops.
- `마계촌` should remember the next alternating effect in a power state.
