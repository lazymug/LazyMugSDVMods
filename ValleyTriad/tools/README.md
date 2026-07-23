# Valley Triad — tools

Reproducible generators for the design assets. **No game assets are committed** here.

- `build_cards.py` — regenerates `../cards.xlsx` (roster, tiers, validation). Pure data;
  object SpriteIndex/price were validated against the game's `Content/Data/Objects.xnb`.
  Run: `python3 build_cards.py` (needs `openpyxl`).
- `render_cards.py` — composes example card previews into `../card-previews/` using sprites
  cropped from the player's own game textures (springobjects, portraits). Those textures
  and the resulting PNGs contain © ConcernedApe art and are **git-ignored** — run locally
  only. Requires Pillow and the extracted `springobjects.png` / `abigail.png` (unpacked
  from the installed game via the `xnb` npm package).
