# Valley Triad — Prompts de arte (fundos e cartas)

Prompts para gerar a arte das molduras, das cenas de fundo e das 52 cartas.
Os prompts estão **em inglês** (geradores respondem melhor). Cada prompt de carta deve ser
usado como `[BASE] + [CENA da categoria] + linha específica da carta`.

## Especificações técnicas

**Resoluções-alvo (entregue NESSE tamanho — ver "Tamanho & blur" abaixo):**

| O quê | Resolução final | Proporção | Onde salvar |
|---|---|---|---|
| **Arte da carta** (conteúdo da janela) | **304×292 px** | ~1.04:1 (quase quadrada) | `assets/art/<cardId>.png` |
| **Cena de categoria** (fundo genérico) | **304×292 px** | ~1.04:1 | (mesma janela; use como base das cartas) |
| **Moldura por raridade** | **368×512 px** | 23:32 ≈ 0.72 (retrato) | `assets/frames/<raridade>.png` |

- **Como gerar**: peça ao gerador em **512×512** (arte/cena) ou **512×704** (moldura) e
  **reduza** para a resolução-alvo com `tools/fit_art.py`. Para reforçar o hi-bit, gere,
  reduza a ~**152×146** (nearest) e volte a 304×292 — isso "trava" o grid de pixel fino.
- **Override por carta**: salvar `assets/art/<cardId>.png` faz o renderer usar essa imagem
  na janela **automaticamente** (senão, usa a cena procedural). Nada de código.
- **Sem texto, sem moldura, sem marca d'água** dentro da arte — moldura/nome/valores/selo
  são compostos pelo mod em runtime.

### Tamanho & blur (por que 5 MB → KB, e por que não borra)
- O card é montado num RenderTarget de **368×512** e a janela recebe a arte **1:1** em
  **304×292**. Depois o card inteiro é só **reduzido** na mesa — **o jogo nunca amplia** sua
  arte, então não há blur perceptível.
- Entregue **já na resolução-alvo** (não maior). Uma imagem 512²+ full-color pesa MBs; a
  mesma reduzida a 304×292 + PNG otimizado dá **dezenas de KB**. Rode `tools/fit_art.py`.
- **Molduras por IA**: a IA não deixa a janela transparente (sai opaca e taparia a arte).
  **Recomendado**: gerar por IA só as **artes das cartas** e manter as molduras procedurais
  (KB, janela correta). Se insistir em molduras por IA, use `fit_art.py --punch` (fura a
  janela nas coords fixas — pode cortar borda decorativa).
- **Nexus**: arte gerada exige a tag **AI-Generated Content**; o `description.txt` atual diz
  "no AI art" (ajustar). Alternativa que preserva o discurso: usar a IA como concept e
  repassar em pixel manualmente.

## [BASE] — prefixo de estilo (use em TODOS os prompts)

```
16-bit hi-bit pixel art illustration, SNES JRPG style, cozy farm-valley aesthetic
inspired by Stardew Valley, visible pixel grid, rich warm palette, painterly pixel
shading, clean pixel clusters, subject centered with a soft spotlight, gentle rim
light, slightly darkened vignette edges, no text, no watermark, no frame, no border,
no anti-aliasing blur, not photorealistic, square composition
```

Negative prompt sugerido:
```
text, watermark, signature, frame, border, blur, photo, 3d render, smooth gradients,
vector art, anime lineart, oversaturated neon
```

## Molduras por raridade (4 imagens, 368×512, janela transparente)

**NÃO desenhe as moedas de valor** — o mod desenha disco + número nas 4 bordas da janela em runtime (garante alinhamento). A moldura tem: madeira, inlay/cantos da raridade, faixa do nome vazia e as gemas de raridade.

| Arquivo | Prompt (após [BASE]) |
|---|---|
| `frames/common.png` | rustic wooden card frame, warm oak planks, simple carved corners, thin moss-green inlay line, empty wooden name plaque near the bottom, one small green gem below the plaque, large empty transparent window in the upper two thirds |
| `frames/uncommon.png` | polished wooden card frame with sapphire-blue inlay line and blue corner studs, wooden name plaque, two blue gems below the plaque, large empty transparent window |
| `frames/rare.png` | dark hardwood card frame with copper-orange inlay, engraved corner flourishes, name plaque with copper trim, three orange gems below the plaque, large empty transparent window |
| `frames/legendary.png` | ornate dark wooden card frame with glowing golden inlay and filigree corners, subtle sparkle highlights, gilded name plaque, four gold gems below the plaque, large empty transparent window |

## Cenas por categoria (fundos genéricos, usados quando não houver arte específica)

**Resolução: 304×292 px** (quase quadrada, ~1.04:1). Gerar 512×512 → reduzir com `fit_art.py`.

| Categoria | Prompt de cena (após [BASE]) |
|---|---|
| Crop / Forage (`field`) | lush farm field at golden hour, receding rows of leafy green crops with distance haze, tilled soil furrows converging to the horizon, warm sky band with a distant treeline |
| Animal (`pasture`) | sunny green pasture with rolling hills, rustic wooden fence line, scattered grass tufts, soft blue sky |
| Mineral / Monster (`mine`) | dim underground cavern, rough rock walls, scattered glowing crystals in purple teal and blue, warm torchlight glow from one side |
| Fish (`sea`) | open ocean at sunset, warm sun low over the horizon with a shimmering reflection path on the water, gentle wave crests, foam line at the bottom |
| Villager (`saloon`) | cozy tavern interior, warm wooden plank wall, shelf with colorful bottles, polished bar counter, soft lantern glow |
| Special (`night`) | starry night sky over a dark valley silhouette, bright moon, drifting fireflies, deep blue-purple gradient |

## Cartas (52)

**Resolução: 304×292 px** (mesma janela das cenas). Formato: `id` — assunto.
Use `[BASE] + [CENA da categoria] +` a linha abaixo.

### Crops — cena `field`
- `parsnip` — a plump cream-white parsnip root with leafy green top, freshly pulled, resting on dark tilled soil
- `cauliflower` — a large round cauliflower with a dense white head wrapped in wide green leaves
- `potato` — a hearty golden-brown potato, freshly dug, bits of soil, small leaf sprout
- `greenbean` — a cluster of crisp green beans hanging from a climbing trellis vine
- `blueberry` — a sprig of plump deep-blue blueberries with tiny leaves, summer light
- `hotpepper` — a glossy bright-red hot pepper with a curved tip, heat shimmer hint
- `melon` — a big striped green melon, one wedge cut showing juicy pink flesh
- `tomato` — a ripe round red tomato with a green star calyx, summer vine behind
- `pumpkin` — a huge round orange pumpkin with deep ribs and a curled stem, autumn leaves
- `cranberries` — a low bush heavy with clusters of shiny crimson cranberries
- `eggplant` — a glossy deep-purple eggplant with a green cap, autumn field
- `yam` — a rustic purple-skinned yam half-buried in dark soil, sturdy leaves
- `ancientfruit` — a mystical teal fruit with fern-like fronds, faint magical glow, ancient vibe
- `sweetgemberry` — a jewel-like translucent crimson berry sparkling like a gemstone on a rare plant

### Forrageáveis — cena `field`
- `wildhorseradish` — a knobby wild horseradish root with white flowers, spring forest floor
- `salmonberry` — bright orange-pink salmonberries on a thorny spring bush
- `spiceberry` — vivid red spiceberries nestled in summer jungle-green leaves
- `commonmushroom` — a classic brown-capped mushroom with cream gills on the autumn forest floor
- `blackberry` — glossy dark blackberries on a bramble, autumn light
- `winterroot` — a pale gnarled root emerging from snowy ground, frost crystals
- `snowyam` — a frost-dusted purple yam half-buried in snow, winter light

### Animais — cena `pasture`
- `chicken` — a plump white hen with a red comb, mid-cluck, feathers catching the sun
- `cow` — a gentle white-and-brown spotted cow with soft eyes, chewing grass
- `duck` — a cheerful white duck with an orange bill, small pond splash
- `rabbit` — a soft brown rabbit with long ears, alert pose, clover patch

### Minerais — cena `mine`
- `quartz` — a cluster of clear white quartz crystals catching torchlight
- `diamond` — a brilliant-cut diamond radiating prismatic sparkles on dark rock
- `prismaticshard` — a legendary iridescent shard cycling through rainbow hues, floating slightly, intense magical aura

### Aldeões — cena `saloon` (fan-art dos personagens de Stardew Valley)
- `abigail` — young woman with long violet hair and bangs, adventurous grin, dark indigo jacket, holding a game controller or sword
- `sebastian` — pale young man with black emo fringe, dark hoodie, aloof look, hint of motorcycle smoke
- `sam` — cheerful young man with spiky blond hair, denim jacket, skateboard or guitar at his side
- `penny` — gentle young woman with short copper-red hair, modest teal dress, holding an open book
- `leah` — artistic woman with a long auburn side-braid, green blouse, wooden sculpture tools
- `elliott` — elegant man with long flowing auburn hair, ruffled shirt and coat, quill in hand, poetic air
- `haley` — fashionable young woman with blonde curls and a blue sundress, holding a camera
- `maru` — bright young woman with glasses and short dark curly hair, workshop overalls, holding a gadget
- `emily` — cheerful woman with short blue hair, colorful handmade clothes, crystals and thread
- `shane` — scruffy man with dark messy hair, blue hoodie with a chicken peeking from it
- `gus` — jolly mustachioed barkeeper in a vest and apron, polishing a mug behind the bar
- `pierre` — shopkeeper with brown hair and glasses, green apron, produce crate in his arms
- `robin` — friendly carpenter woman with short auburn hair, tool belt, hammer over her shoulder
- `clint` — burly blacksmith with a leather apron and heavy gloves, forge glow on his face
- `willy` — weathered old fisherman with a cap and rain slicker, fishing rod, pipe smoke
- `wizard` — mysterious sorcerer with wild dark hair and a purple coat, glowing arcane runes, dramatic shadows (cena `night`)

### Monstros — cena `mine`
- `skeleton` — an animated skeleton warrior with glowing eye sockets raising a bone club
- `shadowbrute` — a hulking shadow creature with burning white eyes, wisps of dark smoke
- `serpent` — a sinuous flying serpent with bared fangs, coiling through cavern air
- `pepperrex` — a small ferocious red dinosaur breathing a puff of flame, prehistoric ferns

### Peixes — cena `sea`
- `sturgeon` — a long armored sturgeon with bony ridges gliding through deep blue water
- `crimsonfish` — a legendary blazing-red fish leaping from the waves at sunset, scales like embers

### Especiais — cena `night`
- `junimoking` — a regal little forest spirit, round and apple-green with a tiny leaf crown, gentle glow, magical forest shrine
- `mrqi` — an enigmatic man with blue skin and a white pompadour, sharp black suit, casino neon glow, holding a glowing dice
