# 00. 캐릭터 컨셉

## 에미야 시로 (Prism Shirou)

**출처**: Fate/stay night  
**특기**: 강화(Reinforcement) + 투영(Tracing)  
**최종 Noble Phantasm**: Unlimited Blade Works

---

## 판타지 캐릭터 루프

```
약한 시작
→ 에너지로 기본 강화/방어 운영
→ 마력을 아껴 강력한 투영 카드 발동
→ Unlimited Blade Works로 투영 카드 풀 폭발
→ 덱이 Noble Phantasm들로 가득 차는 후반
```

---

## 핵심 메커니즘 3가지

### 1. 이중 자원 (에너지 + 마력)
- **에너지**: 매 턴 3회복. 대부분의 카드에 사용.
- **마력(★)**: 전투 시작 시 Starter 렐릭이 지급, 턴마다 회복 안 됨.  
  강력한 투영 카드(Noble Phantasm)에 추가로 소비.
- 설계 의도: 에너지는 턴당 자원, 마력은 전투 단위 예산.  
  어디서 마력을 쓸지 고르는 것 자체가 게임플레이.

### 2. 투영(Trace) 태그 [`Traced`]
- `CardTag.Traced` 커스텀 태그를 가진 카드 = 투영 카드.
- Noble Phantasm 카드들은 대부분 [Traced] + 시전 후 소멸(Exhaust).
- 여러 렐릭/파워가 "투영 카드 시전 시" 추가 효과 발동.
- 덱 빌딩의 축: 투영 카드를 얼마나, 어떤 종류로 모을 것인가.

### 3. Unlimited Blade Works
- 게임의 climax 메커니즘.
- 투영 카드 수에 따라 스케일.
- 버리기 더미의 투영 카드를 전부 손패로 / 전부 강화(Upgrade) 등
  → 구체 효과는 [04_cards_uncommon.md](04_cards_uncommon.md) 참조.

---

## 덱 빌딩 방향 (플레이어 선택지)

| 빌드 | 핵심 | 마력 운용 |
|------|------|-----------|
| **Trace 폭발형** | 투영 카드 다수 축적 + UBW 발동 | 마력을 중반까지 아꼈다가 일제 방출 |
| **강화 누적형** | Reinforcement 파워 스택 + 에너지 카드 | 마력을 조금씩 고효율 단일 타격에 사용 |
| **하이브리드** | 소수 정예 Noble Phantasm + 안정적 방어 | 마력 효율 극대화 |

---

## 구현 단계 요약

| Phase | 내용 |
|-------|------|
| Phase 1 | 캐릭터 등록 + 스타터 덱 + Magic Circuit 렐릭 → 플레이 가능 상태 |
| Phase 2 | Traced 태그 + Uncommon/UBW 구현 + 투영 연동 렐릭 |
| Phase 3 | Rare 카드 + 보스 렐릭 + 이미지 + pck 빌드 |
