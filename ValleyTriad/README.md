# Valley Triad

A Triple Triad (FF8) style collectible card game mod for Stardew Valley (SMAPI), using the
game's own sprites — cards are composed at runtime, so nothing copyrighted is shipped.

## Play
- **Abigail intro:** a Friday at the Saloon with Abigail present grants the starter deck.
- **Challenge:** Friday at the Saloon, empty hand, action button on a villager → a match.
- **Console:** `vt_deck` (collection/deck builder), `vt_open` (practice), `vt_grant` (starter
  pack for testing), `vt_test` (headless engine self-test).

## Project layout
- `Models/` — `Card`, `Deck`, `SaveData`, enums.
- `Game/Board.cs` — the Triple Triad engine (basic + Same/Plus/Combo + elemental), pure logic.
- `Game/Opponents.cs` — opponent deck + AI skill scaled by friendship; starter pack.
- `Data/CardDatabase.cs` — loads `assets/cards.json`.
- `Data/CollectionManager.cs` — per-save collection + deck persistence.
- `Rendering/CardRenderer.cs` + `PixelFont.cs` — runtime card composition, cached to
  `RenderTarget2D`; hero resolved from `ItemRegistry`/portraits/monster/animal sheets; card
  name from the localized `Item.DisplayName`/i18n; cache invalidated on locale change.
- `UI/TriadMenu.cs` — the playable match (stakes, sudden death, reward pick, capture flashes).
- `UI/DeckMenu.cs` — collection album + deck builder.
- `ModEntry.cs` — config (GMCM), world integration, commands.

## Content pipeline (design → data)
The card roster is authored in `cards.xlsx` (the single source of truth). Regenerate the
runtime data + i18n from it:

```bash
python3 tools/gen_cards_json.py   # cards.xlsx -> assets/cards.json + card.* i18n keys
```

Preview generators (`tools/render_pixel_card.py`, `render_field.py`, `build_cards.py`) write to
`card-previews/`, which is **git-ignored** because those PNGs contain © ConcernedApe sprites.
The shipped mod contains no game art — it composes from the player's own files.

## Build
```bash
dotnet build ValleyTriad/ValleyTriad.csproj -p:GamePath="<Stardew>/Contents/MacOS"
```

## Design docs
`RULES.md` (rules + decisions + implementation status) and `wireframes.html` (UX mockups).
