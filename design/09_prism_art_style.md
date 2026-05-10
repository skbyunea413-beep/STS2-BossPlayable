# Prism Card Art Style Guide

This guide defines the visual direction for Prism card portraits. The goal is to keep newly generated or commissioned art consistent with the Slay the Spire 2-inspired, hand-painted vector look already used by the mod.

The character identity is not generic prism magic. Prism art should feel like it belongs to the red triangular Prism creature shown in the existing portraits: a rough red triangular core, a black central hollow eye with a small yellow glow, floating blue cone shards or drill horns, thick black shadow shapes, and bold magenta/yellow action streaks.

## Core Style

- Hand-painted vector illustration, not photorealism.
- Clean layered shapes with chunky, angular silhouettes.
- Carefully painted gradient fills on each shape, but with visible hard cel-shaded planes.
- Readable card-game composition at small portrait size.
- Simple fantasy iconography: the player should understand the card's role quickly.
- High-contrast dark backgrounds or hot red/orange backgrounds with bold action shapes.
- Thick black shadow cuts, heavy dark outlines, and strong pink/magenta rim light.
- No text, card frames, UI, watermarks, logos, or modern objects.

## Character Motifs

- Red triangular rocky prism body, like layered carved stone or folded muscle.
- Black central socket or hollow with a small glowing yellow eye.
- Floating blue cone shards, blue drill horns, or faceted blue crystals.
- Yellow rings, yellow impact bursts, and magenta/purple speed slashes.
- The Prism creature itself should appear in most card art, either as the main subject or as an unmistakable fragment.
- Hands or bodies can appear, but they should be Prism-like or belong to the card subject. Avoid generic armored hands.
- Backgrounds use simple graphic shapes, speed lines, faint circles, or broad shadow silhouettes.
- Motion is shown through sharp angular streaks, not soft particles or noisy brush strokes.

## Palette

- Prism body: deep red, crimson, dark maroon, orange-red highlights.
- Eye and energy: saturated yellow, gold, occasional white sparkle.
- Shards and drills: cobalt blue, cyan-blue, dark navy shadow.
- Background: deep violet, purple, magenta, crimson, red-orange.
- Shadows and outlines: near-black purple or black.
- Rim light: pink/magenta or yellow.

## Avoid

- Photorealistic rendering.
- Anime character illustration.
- 3D render style.
- Heavy painterly brush noise.
- Overly detailed backgrounds.
- Modern sci-fi machinery.
- Flat icon art without hand-drawn gradients.
- Generic crystal magic with no Prism creature identity.
- Generic human hands holding cards unless Prism is clearly present.
- Thin elegant glass shards as the main identity; Prism's shapes are chunky and creature-like.
- Text inside the image.

## Card Concept Motifs

Direct attack:
- The red triangular Prism lunging or spinning with blue drill shards.
- Yellow impact bursts, jagged magenta speed slashes, stone fragments breaking apart.

Other-character card generation:
- The Prism creature pulling or biting a borrowed card from a split-color rift.
- Blue shards orbiting while the yellow eye focuses on one playable card.
- Borrowed cards should be abstract rectangles with no readable text or class logos.

Exhaust synergy:
- Cards cracking and burning into red/blue shards around Prism's central eye.
- Prism swallowing or shattering temporary cards, leaving yellow sparks.

Attack Intent:
- Prism holding a charged pose, blue drill shards suspended midair, yellow rings tightening before release.

Cost or downside:
- Heavy pale stones or black rocks falling around the red Prism body.
- Prism straining, crying, or being weighed down while still glowing.

Fallback plan:
- A blunt direct yellow-white beam or drill-charge from Prism's eye and blue shards.

## Prompt Template

Use this as the default prompt skeleton for card portraits:

```text
Use case: stylized-concept
Asset type: Slay the Spire 2-style card portrait
Primary request: [CARD SUBJECT AND ACTION]
Scene/backdrop: simple graphic dark violet, magenta, or red-orange fantasy backdrop with angular speed shapes
Subject: the Prism creature: rough red triangular body, black central hollow eye with a small glowing yellow pupil, floating blue cone shards or drill horns; [CARD SUBJECT AND ACTION]
Style/medium: Slay the Spire 2-inspired hand-painted vector card illustration, chunky layered shapes, hard cel-shaded gradient planes, thick black shadow cuts, crisp silhouette
Composition/framing: close readable portrait composition, dynamic diagonal action, strong silhouette, no card frame
Lighting/mood: bold yellow energy, magenta rim light, blue shard highlights, dramatic arcade-like impact
Color palette: crimson red Prism body, cobalt blue shards, yellow eye and impact light, deep violet shadows, magenta rim light, red-orange accents
Materials/textures: rough carved red stone, faceted blue crystal drills, black hollow eye socket, graphic energy streaks
Constraints: no text, no UI, no frame, no watermark, no logo, readable at small size
Avoid: photorealism, anime, 3D render, heavy brush texture, noisy background, modern sci-fi objects, generic glass prism, generic human hand, elegant thin crystal shards
```

## File Naming

- Small portrait: `src/PrismMod/images/card_portraits/<card_name>.png`
- Big portrait: `src/PrismMod/images/card_portraits/big/<card_name>.png`
- Use lowercase snake case filenames.
- Keep image subjects tied to card mechanics, not only abstract mood.
- When possible, include the Prism creature or its unmistakable red triangle, black eye, and blue shard language.

## First Draft Targets

Good early candidates for generated art:

- `field_procurement`: the red Prism creature focusing its yellow eye on one abstract borrowed card pulled from a split-color rift.
- `prism_beam`: Prism firing a blunt yellow-white beam from its central eye while blue drill shards align around it.
- `peak_of_folly`: Prism on a red rocky peak, blue shards orbiting, while pale stones tumble into the discard-like shadow below.
- `exhaust_payoff`: Prism shattering temporary cards into red and blue fragments that flow into its black eye.
- `attack_intent_payoff`: Prism frozen in a charged pose, blue drills held back by yellow rings before the next strike.
