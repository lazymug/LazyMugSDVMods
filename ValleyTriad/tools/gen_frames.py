#!/usr/bin/env python3
"""Gera os templates de moldura por raridade (assets/frames/*.png).

Cada PNG é o "chassi" completo da carta — madeira, inlay da raridade, cantos
ornamentados, encaixes (sockets) das moedas, faixa do nome e gemas — com a
JANELA DE ARTE TRANSPARENTE. Em runtime o CardRenderer desenha a arte por baixo
e os discos das moedas + dígitos + nome + selo por cima.

As coordenadas replicam EXATAMENTE as constantes do CardRenderer.cs (GEOMETRY):
  carta lógica 92x128, escala S=4 (PNG 368x512)
  janela de arte: x14..77, y14..77 (transparente) — RECUADA, para que as moedas
    fiquem sobre a borda e o cenário nunca passe por trás delas
  moedas: N(46,7) S(46,84) O(7,46) L(85,46), raio 6
  faixa do nome: (6,93) 80x17 · gemas: y116

Rode `python3 gen_frames.py` após mudar a GEOMETRY do renderer.
"""
from PIL import Image, ImageDraw

OUT = "/Volumes/SSD_Work/Workspace/LazyMugSDVMods/ValleyTriad/assets/frames"
S = 4
LW, LH = 92, 128

# ---- geometry (keep in sync with CardRenderer.cs) ----
WX, WY, WW, WH = 14, 14, 64, 64          # art window (square) — grew as the coins shrank
COINS = [(46, 7), (46, 84), (7, 46), (85, 46)]   # N, S, W, E
COIN_R = 6                                # 4 device px smaller radius than before
PLAQUE = (6, 93, 80, 17)                  # x, y, w, h
GEM_Y = 116

DARK = (44, 28, 14, 255)
WOOD = (146, 99, 55, 255)
WOODHI = (186, 138, 84, 255)
WOODLO = (104, 68, 37, 255)
WOODDK = (78, 50, 27, 255)
RAR = {
    "common":    (111, 161, 74, 255),
    "uncommon":  (74, 143, 192, 255),
    "rare":      (206, 128, 48, 255),
    "legendary": (226, 188, 60, 255),
}
GEMS = {"common": 1, "uncommon": 2, "rare": 3, "legendary": 4}
GEM_EMPTY = (86, 60, 34, 255)


def fill(d, lx, ly, lw, lh, c):
    d.rectangle([lx * S, ly * S, (lx + lw) * S - 1, (ly + lh) * S - 1], fill=c)


def ring(d, lcx, lcy, lr, c, width=1):
    d.ellipse([(lcx - lr) * S, (lcy - lr) * S, (lcx + lr) * S, (lcy + lr) * S],
              outline=c, width=width * S)


def disc(d, lcx, lcy, lr, c):
    d.ellipse([(lcx - lr) * S, (lcy - lr) * S, (lcx + lr) * S, (lcy + lr) * S], fill=c)


def diamond(d, lcx, lcy, lr, c):
    d.polygon([(lcx * S, (lcy - lr) * S), ((lcx + lr) * S, lcy * S),
               (lcx * S, (lcy + lr) * S), ((lcx - lr) * S, lcy * S)], fill=c)


def lerp(a, b, t):
    return tuple(int(a[i] + (b[i] - a[i]) * t) for i in range(4))





def plaque(d, px, py, pw, ph, inlay):
    """Clean recessed name band: dark inset with a subtle bevel and a rarity trim."""
    fill(d, px, py, pw, ph, DARK)
    fill(d, px + 1, py + 1, pw - 2, ph - 2, WOODDK)
    fill(d, px + 1, py + 1, pw - 2, 1, lerp(WOODDK, DARK, .5))          # inner top shadow
    fill(d, px + 1, py + ph - 2, pw - 2, 1, lerp(WOODLO, WOODDK, .4))   # inner bottom light
    fill(d, px, py, pw, 1, lerp(inlay, DARK, .25))
    fill(d, px, py + ph - 1, pw, 1, lerp(inlay, DARK, .25))


def make(tier):
    inlay = RAR[tier]
    img = Image.new("RGBA", (LW * S, LH * S), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # --- body: dark edge + a smooth top-lit gradient ---
    fill(d, 0, 0, LW, LH, DARK)
    for i in range(LH - 4):
        fill(d, 2, 2 + i, LW - 4, 1, lerp(WOODHI, WOODLO, i / (LH - 5)))
    # plank seams (visible on the side margins; the window/plaque cover the middle)
    for y in (26, 58, 100):
        fill(d, 3, y, LW - 6, 1, WOODDK)

    # --- rarity inlay: double line + inner hairline ---
    fill(d, 4, 4, LW - 8, 1, inlay); fill(d, 4, LH - 5, LW - 8, 1, inlay)
    fill(d, 4, 4, 1, LH - 8, inlay); fill(d, LW - 5, 4, 1, LH - 8, inlay)
    fill(d, 6, 6, LW - 12, 1, lerp(inlay, DARK, .45)); fill(d, 6, LH - 7, LW - 12, 1, lerp(inlay, DARK, .45))
    fill(d, 6, 6, 1, LH - 12, lerp(inlay, DARK, .45)); fill(d, LW - 7, 6, 1, LH - 12, lerp(inlay, DARK, .45))

    # --- ornate corners: filigree studs in the rarity colour ---
    for (cx, cy) in [(4, 4), (LW - 5, 4), (4, LH - 5), (LW - 5, LH - 5)]:
        fill(d, cx - 2, cy - 2, 5, 5, DARK)
        fill(d, cx - 1, cy - 1, 3, 3, inlay)
        fill(d, cx, cy, 1, 1, lerp(inlay, (255, 255, 255, 255), .55))
    # short L-shaped filigree tucked inside each corner
    for (cx, cy, sx, sy) in [(7, 7, 1, 1), (LW - 8, 7, -1, 1), (7, LH - 8, 1, -1), (LW - 8, LH - 8, -1, -1)]:
        for k in range(0, 4):
            fill(d, cx + sx * k, cy, 1, 1, lerp(inlay, WOOD, .3))
            fill(d, cx, cy + sy * k, 1, 1, lerp(inlay, WOOD, .3))
        fill(d, cx + sx * 3, cy + sy * 3, 1, 1, lerp(inlay, WOODHI, .2))

    # --- art window: bevelled socket (dark rim + inner highlight) ---
    fill(d, WX - 2, WY - 2, WW + 4, WH + 4, WOODDK)
    fill(d, WX - 2, WY - 2, WW + 4, 1, lerp(WOODHI, WOOD, .4))
    fill(d, WX - 1, WY - 1, WW + 2, WH + 2, DARK)

    # --- coin sockets on the wooden border (runtime draws disc + digit on top) ---
    for (cx, cy) in COINS:
        disc(d, cx, cy, COIN_R + 1.2, WOODDK)
        disc(d, cx, cy, COIN_R + 0.4, DARK)

    # --- name plaque: a little wooden sign board ---
    plaque(d, *PLAQUE, inlay)

    # --- rarity gems below the plaque ---
    ng = GEMS[tier]
    for i in range(4):
        gx = LW // 2 - 7 + i * 5
        col = inlay if i < ng else GEM_EMPTY
        diamond(d, gx, GEM_Y, 2.4, DARK)
        diamond(d, gx, GEM_Y, 1.8, col)
        if i < ng:
            fill(d, gx, GEM_Y - 1, 1, 1, lerp(col, (255, 255, 255, 255), .6))

    # --- punch the art window transparent (1px DARK rim stays around it) ---
    d.rectangle([WX * S, WY * S, (WX + WW) * S - 1, (WY + WH) * S - 1], fill=(0, 0, 0, 0))
    return img


if __name__ == "__main__":
    import os
    os.makedirs(OUT, exist_ok=True)
    for tier in RAR:
        make(tier).save(f"{OUT}/{tier}.png")
        print("ok", tier)
