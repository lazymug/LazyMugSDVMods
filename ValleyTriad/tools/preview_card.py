#!/usr/bin/env python3
"""Preview: simula o que o CardRenderer desenha (arte + moldura + moedas/dígitos/nome).

Serve para validar a geometria (GEOMETRY em gen_frames.py / CardRenderer.cs) sem
precisar abrir o jogo. Gera uma folha de contato em /tmp.

Uso: python3 preview_card.py [cardId ...]
"""
import os, sys, json
from PIL import Image, ImageDraw

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import gen_frames as G

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
S = G.S
ART = (G.WW * S, G.WH * S)

# 5x7 pixel font (same glyphs the mod uses for digits/letters)
FONT = {
    '0': ["01110","10001","10011","10101","11001","10001","01110"],
    '1': ["00100","01100","00100","00100","00100","00100","01110"],
    '2': ["01110","10001","00001","00110","01000","10000","11111"],
    '3': ["11111","00010","00100","00010","00001","10001","01110"],
    '4': ["00010","00110","01010","10010","11111","00010","00010"],
    '5': ["11111","10000","11110","00001","00001","10001","01110"],
    '6': ["00110","01000","10000","11110","10001","10001","01110"],
    '7': ["11111","00001","00010","00100","01000","01000","01000"],
    '8': ["01110","10001","10001","01110","10001","10001","01110"],
    '9': ["01110","10001","10001","01111","00001","00010","01100"],
    'A': ["01110","10001","10001","11111","10001","10001","10001"],
    'B': ["11110","10001","10001","11110","10001","10001","11110"],
    'C': ["01110","10001","10000","10000","10000","10001","01110"],
    'D': ["11100","10010","10001","10001","10001","10010","11100"],
    'E': ["11111","10000","10000","11100","10000","10000","11111"],
    'F': ["11111","10000","10000","11100","10000","10000","10000"],
    'G': ["01110","10001","10000","10111","10001","10001","01110"],
    'H': ["10001","10001","10001","11111","10001","10001","10001"],
    'I': ["01110","00100","00100","00100","00100","00100","01110"],
    'J': ["00111","00010","00010","00010","10010","10010","01100"],
    'K': ["10001","10010","10100","11000","10100","10010","10001"],
    'L': ["10000","10000","10000","10000","10000","10000","11111"],
    'M': ["10001","11011","10101","10101","10001","10001","10001"],
    'N': ["10001","10001","11001","10101","10011","10001","10001"],
    'O': ["01110","10001","10001","10001","10001","10001","01110"],
    'P': ["11110","10001","10001","11110","10000","10000","10000"],
    'Q': ["01110","10001","10001","10001","10101","10010","01101"],
    'R': ["11110","10001","10001","11110","10100","10010","10001"],
    'S': ["01111","10000","10000","01110","00001","00001","11110"],
    'T': ["11111","00100","00100","00100","00100","00100","00100"],
    'U': ["10001","10001","10001","10001","10001","10001","01110"],
    'V': ["10001","10001","10001","10001","10001","01010","00100"],
    'W': ["10001","10001","10001","10101","10101","11011","10001"],
    'X': ["10001","10001","01010","00100","01010","10001","10001"],
    'Y': ["10001","10001","01010","00100","00100","00100","00100"],
    'Z': ["11111","00001","00010","00100","01000","10000","11111"],
    ' ': ["00000"] * 7,
}
GW = 6


def text_w(t, blk):
    return (len(t) * GW - 1) * blk


def draw_text(d, t, cx, cy, blk, fill, outline):
    w = text_w(t, blk); x0 = cx - w // 2; y0 = cy - (7 * blk) // 2
    for pass_ in (0, 1):
        for gi, ch in enumerate(t):
            g = FONT.get(ch)
            if not g:
                continue
            for ry, row in enumerate(g):
                for rx, bit in enumerate(row):
                    if bit != '1':
                        continue
                    x = x0 + gi * GW * blk + rx * blk; y = y0 + ry * blk
                    if pass_ == 0:
                        for ox, oy in ((-blk, 0), (blk, 0), (0, -blk), (0, blk)):
                            d.rectangle([x + ox, y + oy, x + ox + blk - 1, y + oy + blk - 1], fill=outline)
                    else:
                        d.rectangle([x, y, x + blk - 1, y + blk - 1], fill=fill)


def render(card_id, tier, name, edges):
    frame = G.make(tier)
    card = Image.new("RGBA", (G.LW * S, G.LH * S), (0, 0, 0, 0))
    art_path = os.path.join(ROOT, "assets", "art", card_id + ".png")
    if os.path.exists(art_path):
        art = Image.open(art_path).convert("RGBA").resize(ART, Image.LANCZOS)
        card.alpha_composite(art, (G.WX * S, G.WY * S))
    card.alpha_composite(frame, (0, 0))

    d = ImageDraw.Draw(card)
    for (cx, cy), v in zip(G.COINS, edges):
        G.disc(d, cx, cy, G.COIN_R, (44, 28, 14, 255))
        G.disc(d, cx, cy, G.COIN_R - 0.8, (198, 150, 78, 255))
        G.disc(d, cx, cy, G.COIN_R - 2.0, (240, 211, 150, 255))
        draw_text(d, "A" if v == 10 else str(v), cx * S, cy * S, 4, (44, 28, 14, 255), (245, 224, 170, 255))

    px, py, pw, ph = G.PLAQUE
    blk = max(1, min(3, ((pw - 4) * S) // max(1, text_w(name, 1))))
    draw_text(d, name, (px + pw // 2) * S, (py + ph // 2) * S, blk, (245, 233, 205, 255), (44, 28, 14, 255))

    bg = Image.new("RGBA", card.size, (58, 86, 58, 255))
    bg.alpha_composite(card)
    return bg.convert("RGB")


def main():
    cards = json.load(open(os.path.join(ROOT, "assets", "cards.json")))
    cards = cards if isinstance(cards, list) else next((v for v in cards.values() if isinstance(v, list)), [])
    by_id = {(c.get("Id") or c.get("id")): c for c in cards}
    ids = sys.argv[1:] or ["pierre", "cauliflower", "prismaticshard", "abigail"]
    tiles = []
    for cid in ids:
        c = by_id[cid]
        tiles.append(render(cid, str(c.get("Tier")).lower(), cid.upper(),
                            [c.get("N"), c.get("S"), c.get("W"), c.get("E")]))
    w, h = tiles[0].size; pad = 12
    sheet = Image.new("RGB", (len(tiles) * w + (len(tiles) + 1) * pad, h + 2 * pad), (35, 35, 40))
    for k, im in enumerate(tiles):
        sheet.paste(im, (pad + k * (w + pad), pad))
    out = "/private/tmp/claude-501/-Volumes-SSD-Work-Workspace-LazyMugSDVMods/54b80de6-b82a-479e-a72c-690c19363333/scratchpad/preview_cards.png"
    sheet.save(out)
    print("saved", out, sheet.size)


if __name__ == "__main__":
    main()
