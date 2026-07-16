# Gotoro Empire — Design Document

**Vision chosen:** Región/Área — a new visitable area representing the Gotoro Empire,
reachable from the valley. This is the most ambitious of the four options and the
heaviest in map/art work.

## Current state (starting point)

The mod is a skeleton:
- **C# (`ModEntry.cs`)** — empty `Entry` (`// todo`). Declares a dependency on
  `DIGUS.MailFrameworkMod` (letters were intended).
- **`[CP] GotoroEmpire`** — adds a single character `James` (Winter 27, `HomeRegion: "Other"`,
  Male, Adult) with **no** sprite, portrait, schedule, dialogue, or map. It will not load
  correctly as-is (missing assets).
- **`[PFM] Gotoro Empire`** — empty producer pack (custom machines were intended).

## Lore anchor

The Gotoro Empire is the nation that fought the Ferngill Republic; **Kent** was a POW
there for two years. Aesthetic: an eastern/imperial power. Post-war fiction: a Gotoro
**trade delegation / cultural enclave** establishes a small port reachable by boat,
opening commerce and slowly thawing relations. This lets us add an area without
rewriting valley history, and gives natural hooks (Kent's reaction, imported goods).

## What a "new area" mod requires

1. **Maps** (Tiled `.tmx` or CP map assets) — the biggest lift.
   - Exterior: "Gotoro Trade Port" (docks, market square).
   - Interiors: a teahouse/inn, a bazaar shop, James's residence.
   - Tilesheets: reuse vanilla where possible; custom sheets for imperial architecture.
2. **Access / travel** — how the player reaches it.
   - Recommended: a **dock added to the Beach** with a boat, unlocked by an intro
     quest that starts with a **letter from James** (reuses MailFrameworkMod — our strength).
   - Alternative: extend Willy's boat (Ginger Island style) with a second destination.
3. **New location registration (SDV 1.6)** — CP `EditData` on `Data/Locations` +
   `Maps/<Name>` map asset + warp wiring (passability, warp tiles both directions).
4. **NPCs** — James + 2-3 Gotoro villagers. Each needs: `Data/Characters` entry,
   sprite + portrait (pixel art), schedule, dialogue, gift tastes.
5. **Shop / economy** — a Gotoro bazaar (`Data/Shops`) selling exotic imported goods
   (new items via CP `Data/Objects`). Plays to the mod's trade theme.
6. **PFM producers** — imperial artisan machines (e.g., a Tea Press / Gotoro Kiln)
   that turn valley crops into imperial goods. The `[PFM]` pack is already stubbed.
7. **Quests / events** — arrival quest, a few James heart events, optionally a
   "Gotoro Lantern Festival".
8. **Lore integration** — optional, sensitive dialogue hooks with Kent.

## Phased plan

- **Phase 0 — Foundation (no custom art; I can do this now)**
  - Intro letter (MailFrameworkMod) → unlock.
  - One placeholder interior map built from a **vanilla tilesheet**.
  - Warp in/out working from the Beach dock.
  - James present with basic dialogue, using a **vanilla sprite/portrait as placeholder**.
  - Goal: prove the full pipeline (location registration + warp + NPC + letter) end-to-end.

- **Phase 1 — Identity (needs art)**
  - Custom tilesheets + proper exterior port map.
  - James's real portrait + sprite.
  - Bazaar shop with a handful of imported items (`Data/Objects` + `Data/Shops`).

- **Phase 2 — Depth**
  - PFM machines + recipes.
  - More NPCs with schedules, gift tastes, heart events.

- **Phase 3 — Polish**
  - Festival, music, Kent lore hooks.

## Key risks / open decisions

- **Art is the bottleneck.** Maps, tilesheets, portraits, sprites all need pixel art.
  Phase 0 sidesteps this with vanilla placeholders to validate the systems first.
- **Map authoring** requires Tiled. Decide: hand-authored `.tmx` vs. CP `EditMap` patches
  onto a copied vanilla map.
- **Access method**: Beach dock (recommended) vs. Willy's boat extension.
- **Scope of NPC roster** for launch (suggest: James only in Phase 0/1).
