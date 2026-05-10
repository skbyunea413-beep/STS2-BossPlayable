# 07. 파워

> **상태**: 🔲 미완 — 아이디어 목록 단계

---

## 설계 방향

- 카드 파워 외에 고유 파워 구현이 필요한 것들만 별도 정리.
- 카드 효과로 이미 포함된 파워(Reinforcement, UBW 등)는 구현 필수.
- 렐릭 효과에서 파생되는 파워도 포함.

---

## 버프 파워 (플레이어에게 적용)

| 파워명 | StackType | 효과 | 출처 | 상태 |
|--------|-----------|------|------|------|
| **Reinforcement** | Counter | 강도(Strength) 와 동일: 공격 피해 +Amount. | Reinforcement 카드, 렐릭 | 🔲 |
| **Unlimited Blade Works** | Single | 매 턴 시작 시 버리기 더미 투영 카드 1장 뽑기. 투영 카드 시전 시 전체 피해 3. | UBW 카드 | 🔲 |
| **Mana Surge** | Single | 3턴(또는 2턴)마다 ★1 획득. 턴 카운터 관리. | Mana Surge 카드 | 🔲 |
| **Structural Grasp** | Counter | 카드 뽑을 때마다 블록 +1. | 특정 렐릭 or 카드 | 🔲 |
| **Reinforcement Mastery** | Counter | 공격 카드 시전 시 Amount 강도 누적 (최대 한도 있음). | 카드 | 🔲 |

---

## 디버프 파워 (적에게 적용)

| 파워명 | StackType | 효과 | 출처 | 상태 |
|--------|-----------|------|------|------|
| **Broken Fantasy** | Counter | 받는 투영([Traced]) 공격 피해 +25%. | 특정 카드 | 🔲 |

---

## 구현 노트

```csharp
// Reinforcement 파워 — Strength와 동일한 ModifyDamageAdditive 패턴
public override decimal ModifyDamageAdditive(
    Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
{
    if (base.Owner != dealer) return 0m;
    if (!props.IsPoweredAttack()) return 0m;
    return base.Amount;
}

// UBW 파워 — AfterSideTurnStart (ctx 없음!) + AfterCardPlayed
public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
{
    if (side != base.Owner.Side) return;
    Flash();
    // 버리기 더미에서 투영 카드 1장 뽑기
    var traced = PileType.Discard.GetPile(base.Owner.Player)
        .Cards.FirstOrDefault(c => c.Tags.Contains(PrismCardTags.Traced));
    if (traced != null)
        await CardPileCmd.Add(traced, PileType.Hand);
}

// Mana Surge 파워 — AfterTurnEnd 턴 카운터
private int _turnCount = 0;
public override async Task AfterTurnEnd(PlayerChoiceContext ctx, CombatSide side)
{
    if (side != base.Owner.Side) return;
    _turnCount++;
    if (_turnCount >= base.Amount)  // Amount = 3턴 or 2턴
    {
        _turnCount = 0;
        Flash();
        await PlayerCmd.GainStars(1, base.Owner.Player);
    }
}
```

---

## 미결 사항

- [ ] Reinforcement를 기존 StrengthPower를 그냥 쓸지, 커스텀 파워로 만들지 결정
- [ ] UBW의 AfterSideTurnStart에서 뽑기가 안 되면(버리기 더미 비어있음) 뽑기 더미 뒤지는지 여부
- [ ] Broken Fantasy 적용 조건 — 투영 태그 체크를 ModifyDamage에서 cardSource.Tags로 가능한지 확인
