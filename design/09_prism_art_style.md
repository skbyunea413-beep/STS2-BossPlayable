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
- More vector-like than painterly: large flat color regions, clean shape boundaries, controlled gradients, and minimal texture noise.
- Restrict each card to a small palette. Prefer 5 to 7 total colors plus black shadow, not full concept-art color variation.
- Use 2 or 3 value steps per material. Avoid many tiny highlights, hue shifts, and polished rendering passes.
- No text, card frames, UI, watermarks, logos, or modern objects.

## Character Motifs

- Red triangular rocky prism body, like layered carved stone or folded muscle.
- Black central socket or hollow with a small glowing yellow eye.
- No mouth. Prism's expression comes only from the eye, body tilt, tears, and surrounding motion.
- Floating blue cone shards, blue drill horns, or faceted blue crystals.
- Yellow rings, yellow impact bursts, and magenta/purple speed slashes.
- The Prism creature itself should appear in most card art, either as the main subject or as an unmistakable fragment.
- Hands or bodies can appear, but they should be Prism-like or belong to the card subject. Avoid generic armored hands.
- Backgrounds use simple graphic shapes, speed lines, faint circles, or broad shadow silhouettes.
- Motion is shown through sharp angular streaks, not soft particles or noisy brush strokes.
- The mood should be slightly grotesque but funny: one-eyed, weirdly expressive, charmingly ugly, awkward, and mischievous rather than elegant.
- Push the "kimo-kawaii" feeling when appropriate: gross-cute tears, goofy panic, odd proportions, and comic discomfort.

## Palette

Default limited palette:
- Shadow/outline: near-black purple.
- Prism body dark: dark maroon.
- Prism body light: flat crimson red.
- Shards/drills: cobalt blue.
- Eye/impact: saturated yellow.
- Background: deep violet or red-orange, pick one dominant background color.
- Optional accent: magenta rim light. Use sparingly.

Allowed support colors, choose up to 3 per image:
- Pale stone gray for rocks, teeth-like props, or neutral debris.
- Warm orange for impact bursts, body highlights, or hot backgrounds.
- Cyan-blue for small shard highlights or tear drops.

Do not use all possible Prism colors in one image. Choose one dominant background family and keep the rest as accents. A good card usually has the default Prism colors plus only 2 or 3 support colors.

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
- Elegant, beautiful, noble, or purely mystical Prism designs.
- Normal cute mascot proportions without the unsettling one-eyed weirdness.
- Mouths, teeth, lips, tongues, noses, or readable facial features other than the single eye.
- Square preview compositions that do not fit the card portrait crop.
- Over-rendered concept-art detail that hides the simple vector shape language.
- Rich multi-hue palettes, glossy rendering, many highlight colors, and excessive material detail.
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
Style/medium: Slay the Spire 2-inspired hand-painted vector card illustration, chunky layered shapes, hard cel-shaded gradient planes, clean vector-like color regions, thick black shadow cuts, crisp silhouette, minimal texture
Composition/framing: 4:3 landscape card portrait composition, close readable subject, dynamic diagonal action, strong silhouette, crop-safe inside a card art window, no card frame
Lighting/mood: bold yellow energy, magenta rim light, blue shard highlights, dramatic arcade-like impact
Color palette: limited Prism palette: near-black purple shadows, dark maroon, flat crimson red, cobalt blue, saturated yellow, one dominant deep violet or red-orange background; choose up to 3 support colors from pale stone gray, warm orange, cyan-blue, and magenta accent
Materials/textures: rough carved red stone, faceted blue crystal drills, black hollow eye socket, graphic energy streaks
Constraints: no text, no UI, no frame, no watermark, no logo, readable at small size
Avoid: photorealism, anime, 3D render, heavy brush texture, noisy background, modern sci-fi objects, generic glass prism, generic human hand, elegant thin crystal shards, mouth, teeth, lips, tongue, nose, square composition, over-rendered detail, rich palette, glossy rendering, many highlights
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
