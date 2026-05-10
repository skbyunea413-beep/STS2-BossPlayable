# Prism Card Art Style Guide

This guide defines the visual direction for Prism card portraits. The goal is to keep newly generated or commissioned art consistent with the Slay the Spire 2-inspired, hand-painted vector look already used by the mod.

## Core Style

- Hand-painted vector illustration, not photorealism.
- Clean layered shapes with crisp silhouettes.
- Carefully painted gradient fills on each shape.
- Readable card-game composition at small portrait size.
- Simple fantasy iconography: the player should understand the card's role quickly.
- Dark muted backgrounds with bright refracted highlights.
- No text, card frames, UI, watermarks, logos, or modern objects.

## Prism Motifs

- Glass, crystal, prism shards, refracted light, broken mirrors.
- Cyan, white, magenta, gold, and small lime highlights.
- Black metal, pale stone, deep violet shadows, charcoal navy backgrounds.
- Light splitting through cards, weapons, hands, or runes.
- Motion shown through sharp light trails rather than noisy brush strokes.

## Avoid

- Photorealistic rendering.
- Anime character illustration.
- 3D render style.
- Heavy painterly brush noise.
- Overly detailed backgrounds.
- Modern sci-fi machinery.
- Flat icon art without hand-drawn gradients.
- Text inside the image.

## Card Concept Motifs

Direct attack:
- Prism blades, piercing beams, glass shrapnel, refracted impact bursts.

Other-character card generation:
- Strange cards emerging from a prism, split-color card silhouettes, broken mirror choices, borrowed runes.

Exhaust synergy:
- Cards dissolving into light, crystal dust, burning glass edges, fading fragments.

Attack Intent:
- Held aim, light gathered before release, delayed strike posture, a suspended beam about to fire.

Cost or downside:
- Heavy black stones, cracked crystal weights, brilliant light trapped under rubble.

Fallback plan:
- A direct white prism beam cutting across the portrait, simple and forceful.

## Prompt Template

Use this as the default prompt skeleton for card portraits:

```text
Use case: stylized-concept
Asset type: Slay the Spire 2-style card portrait
Primary request: [CARD SUBJECT AND ACTION]
Scene/backdrop: dark muted fantasy background, simple and uncluttered
Subject: [MAIN SUBJECT]
Style/medium: hand-painted vector card illustration, clean layered shapes, carefully hand-drawn gradient fills, crisp silhouette
Composition/framing: centered readable composition, strong silhouette, portrait crop, no card frame
Lighting/mood: dramatic prism light, refracted cyan white magenta gold highlights, soft rim light
Color palette: deep violet shadows, charcoal navy, cyan, white, magenta, gold, small lime accents
Materials/textures: glass, crystal, black metal, pale stone, glowing runes
Constraints: no text, no UI, no frame, no watermark, no logo, readable at small size
Avoid: photorealism, anime, 3D render, heavy brush texture, noisy background, modern sci-fi objects
```

## File Naming

- Small portrait: `src/PrismMod/images/card_portraits/<card_name>.png`
- Big portrait: `src/PrismMod/images/card_portraits/big/<card_name>.png`
- Use lowercase snake case filenames.
- Keep image subjects tied to card mechanics, not only abstract mood.

## First Draft Targets

Good early candidates for generated art:

- `field_procurement`: a prism pulling one playable borrowed card into the hand.
- `prism_beam`: a direct beam splitting through several enemies or silhouettes.
- `peak_of_folly`: brilliant cards rising from a crystal peak while black stones fall into shadow.
- `exhaust_payoff`: a card dissolving into crystal light and feeding a central prism.
- `attack_intent_payoff`: a held prism lance gathering light for the next strike.
