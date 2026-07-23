# LazyMug SDV Mods — Roadmap

Central list of future work: pending features on shipped mods, and concepts for new
mods not started yet. Per-mod deep dives live next to each mod
(`LMQoL/future-release.txt`, `GotoroEmpire/DESIGN.md`); this file is the index and the
place where new ideas are captured before they get their own folder.

Legend: ✅ done · 🚧 in progress · 📋 planned · 💡 idea (not committed)

---

## Shipped mods — pending work

### LM QoL (v1.3.0)
See `LMQoL/future-release.txt` for full notes.
- 📋 **Skip Fade Transitions** — reverted (soft-locked the game on warp). Needs a Harmony
  patch on `ScreenFade.UpdateFadeAlpha` that multiplies the per-tick delta instead of
  snapping `fadeToBlackAlpha` to the boundary. Requires in-game testing before shipping.
- 💡 **No Fence Decay** — default OFF (changes balance).
- 💡 **Trash Can Refund** — default OFF (changes balance).

### NPC Memory (v2.0.1)
- 💡 **Gossip chains** — one NPC repeats what another told them ("Gus mentioned you've been
  fishing a lot"), so activity spreads through the social graph.
- 💡 **Newsletter classifieds / letters to the editor** — occasional reader-submitted
  snippets pulled from tracked events.
- 💡 **Reaction memory** — NPCs remember gifts/quests, not just skill activities.

### Gotoro Empire (v0.0.1)
See `GotoroEmpire/DESIGN.md`. Vision is a visitable Região/Área.
- 📋 **Phase 0** — foundation with vanilla placeholder assets (no custom art). Buildable now.
- 📋 Custom pixel art is the main blocker for later phases.

### Weathered Letters (v1.0.1)
- 💡 **Letter replies** — let the player write back; unlocks follow-up letters or small rewards.
- 💡 **Cross-mod hooks** — letters that reference activity tracked by NPC Memory.

### Allellopathy (v1.1.0)
- 💡 **In-game companion guide** — a purchasable almanac showing good/bad neighbors per crop.
- 💡 **Visual feedback** — subtle overlay/tooltip when hovering a tile showing net effect.

### Elemental Force (v1.6.0)
- 💡 **Elemental resistances/weaknesses** on monsters.
- 💡 **Status effects** (burn, freeze, shock) tied to each element.

---

## New mod concepts (not started)

### 💡 Town Bulletin Board
A physical board in town with rotating requests, rumors, and lost-and-found — a lighter,
always-visible counterpart to the mail-based systems. Could surface NPC Memory events.

### 💡 Traveling Caravans
Periodic themed merchant caravans (spice trader, exotic seeds, curios) that park at the
bus stop for a day, each with a small haggling interaction.

### 💡 Seasonal Side-Events
Small non-festival events that fire on specific days/weather (a meteor shower fishing night,
a mushroom bloom in the forest) with limited-time gathering.

### 💡 Pet Tricks & Bonding
Teach the farm pet simple tricks over time; higher bond gives small passive perks
(finds forage, warns of incoming rain).

### 💡 Skill Mastery Perks
Optional post-level-10 perks per skill, chosen at a mastery shrine — keeps late game
progression interesting.

### 💡 Reputation with Factions
Track standing with groups (Adventurer's Guild, JojaMart, the Wizard) that gates small
perks and dialogue, reusing the NPC Memory tracking backbone.

---

## Minigames — separate standalone mods (revised)

Decision (revised 2026-07-23): ship the substantial minigames as **separate, standalone
mods**, not one umbrella. Rationale: Nexus counts *unique downloads per mod page*, so a
user who wants several minigames generates one counted download per page (more than a
single bundled page), and each mod ranks/searches independently — better for Donation
Points and discovery. Shared minigame-engine code is **copied internally** per mod, never a
required dependency (hard deps suppress downloads/DP). Only promote a minigame to its own
page if it's robust enough to sustain one; fold the slight ones into existing themed mods.

Priority (by download/DP potential): **Valley Triad** (flagship) › Monster Arena ›
Flower Dance Rhythm › Foraging Quiz / Fish Hunt.

- **Valley Triad** — own mod (flagship).
- **Monster Arena** — own mod.
- **Flower Dance Rhythm** — standalone-light, or a feature of a festivals mod.
- **Foraging Quiz** — fold into NPC Memory / a Linus mod (not its own page).
- **Legendary Fish Hunt** — only with a strong distinct mechanic, else cut.

### 📋 Valley Triad (tavern card game) — anchor feature; Triple Triad (FF8) style
> **Canonical, up-to-date rules live in `ValleyTriad/RULES.md`.** The notes below are a
> high-level snapshot; where they differ, RULES.md wins (e.g. acquisition is now
> play-based with no vendor, and the Elemental Force spirit design is deferred).

Direction chosen: adapt **FF8's Triple Triad**. Cards use **in-game sprites** (crops,
villagers, monsters, forageables, legendary creatures) — no custom art needed. 1v1 vs
NPCs at the Saloon; collectible.

**Intro event (tutorial):** on a **Friday when Abigail is at the Saloon**, entering triggers
an event where **Abigail introduces Valley Triad**, explains the rules, and hands the player
a **starter pack**. Abigail fits as the resident gamer. After this, the Friday-Saloon CTA
lets the player challenge villagers.

**Core ruleset (as FF8):**
- 3×3 board; each player holds 5 cards.
- Each card has 4 edge values N/E/S/W, 1–10 (display A = 10).
- Players alternate placing one card per turn.
- Capture: when placed, compare facing edges vs each adjacent enemy card; higher edge
  flips it to your color.
- Board fills (9 placed); most cards owned (board + leftover hand card) wins.

**Valley flavor:**
- Tiered pool from game sprites: crops/animals (low) → villagers (mid) → bosses/legendary
  (Wizard, Junimo King, Legendary Fish; high).
- Acquisition (v1) is **100% play-based** (no vendor/gacha, to stay distinct from the
  Pelican Packs mod): Abigail's starter pack, then winning cards from NPCs via the trade
  rule. Monster/fishing drops deferred.

**Advanced rules (all at launch):** Same, Plus, Combo.

**Trade rule:** "One" — winner takes one card from the loser (feeds the collection).

**Element rule — pluggable provider (seasons at launch, Elemental Force as an increment):**
Model "element" as an abstract provider so the source can be swapped/extended.
- **Launch — Seasons:** provider yields Spring/Summer/Fall/Winter. FF8 Elemental rule:
  tile with an element → matching card +1 to all edges, different/none −1. A card's season
  comes from its subject (Pumpkin = Fall, Strawberry = Spring, …).
- **Increment — Elemental Force (soft dep, reflection only; see `smapi-optional-deps`):**
  detected at runtime; absent = no-op. EF's 9 spirits: Ifrit (fire), Shiva (ice), Titan
  (earth), Leviathan (water), Ramuh (thunder), Phoenix (fire/revive), Carbuncle (light),
  Kirin (holy), Cactuar (wind). Integration layers:
  - **(a) Spirit tiles & cards** — tiles can spawn a spirit; themed cards carry a spirit
    (forge/fire → Ifrit, water/fish → Leviathan, rock/slime → Titan, Wizard → Kirin/
    Carbuncle). Same +1/−1 match rule as seasons.
  - **(b) Affinity triangle** (makes EF *mechanically* meaningful): on capture, if the
    attacker's spirit beats the defender's, attacker wins ties and gets a small edge bonus
    for that comparison. Counters: Water › Fire, Fire › Ice, Thunder › Water, Earth ›
    Thunder, Ice › Earth. Carbuncle/Kirin are neutral/support. Exact numbers = balancing.
  - **(c) Phoenix gimmick** (optional cherry): a captured Phoenix card has a 1-in-X chance
    to "revive" and flip back to its owner once.
  Ship **(a)+(b)**; **(c)** optional.

### 📋 Flower Dance Rhythm
Rhythm minigame during the Flower Dance festival; good performance grants a bonus
(friendship / a decoration). Uses an existing world hook (the festival).

### 📋 Monster Arena (wave defense)
Survive escalating waves at the Adventurer's Guild for per-round rewards; optional
run modifiers. Reuses the vanilla combat system.

### 💡 Foraging ID quiz (Linus) — lightest, pending go/no-go
Sort forage as safe vs. poisonous under time pressure; rewards foraging XP/friendship.
Thematic but slight — only if we want a very light Linus-flavored feature.

### 💡 Legendary Fish Hunt — replaces the (vanilla-redundant) fishing tournament
Multi-stage boss-fishing expedition with a new resource to manage (line tension / special
bait), ramping to an aquatic "boss". Needs a distinct mechanic or it's cut.

### Parked (not selected this round)
Slingshot shooting gallery · Cooking timing showdown · Tavern darts/pool · Mine Cart Rush.
