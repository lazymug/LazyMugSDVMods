#!/usr/bin/env python3
"""Gera os templates de moldura por raridade (assets/frames/*.png).

Cada PNG é o "chassi" completo da carta — madeira, inlay da raridade, moedas
vazias, faixa do nome e gemas — com a JANELA DE ARTE TRANSPARENTE. Em runtime o
CardRenderer desenha a cena/sprite por baixo e os dígitos/nome/selo por cima.

As coordenadas replicam EXATAMENTE as constantes do CardRenderer.cs:
  carta lógica 92x128, escala S=4 (PNG 368x512)
  janela de arte: x8..83, y12..84 (transparente)
  moedas: N(46,16) S(46,81) W(12,48) E(80,48), raios 11/10/8.2
  faixa do nome: (6,92) 79x14 · gemas: y112, x 38+5i
"""
from PIL import Image, ImageDraw

OUT = "/Volumes/SSD_Work/Workspace/LazyMugSDVMods/ValleyTriad/assets/frames"
S = 4
LW, LH = 92, 128

DARK = (53, 34, 18, 255)
WOOD = (139, 94, 52, 255)
WOODHI = (178, 131, 80, 255)
WOODLO = (101, 66, 36, 255)
RAR = {
    "common":    (111, 161, 74, 255),
    "uncommon":  (74, 143, 192, 255),
    "rare":      (196, 122, 46, 255),
    "legendary": (217, 181, 58, 255),
}
GEMS = {"common": 1, "uncommon": 2, "rare": 3, "legendary": 4}
GEM_EMPTY = (90, 66, 38, 255)

def fill(d, lx, ly, lw, lh, c):
    d.rectangle([lx * S, ly * S, (lx + lw) * S - 1, (ly + lh) * S - 1], fill=c)

def blob(d, lcx, lcy, lr, c):
    d.ellipse([(lcx - lr) * S, (lcy - lr) * S, (lcx + lr) * S, (lcy + lr) * S], fill=c)

def make(tier):
    inlay = RAR[tier]
    img = Image.new("RGBA", (LW * S, LH * S), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # corpo
    fill(d, 0, 0, LW, LH, DARK)
    fill(d, 2, 2, LW - 4, LH - 4, WOOD)
    fill(d, 2, 2, LW - 4, 1, WOODHI)
    fill(d, 2, LH - 3, LW - 4, 1, WOODLO)
    # inlay da raridade
    fill(d, 4, 4, LW - 8, 1, inlay); fill(d, 4, LH - 5, LW - 8, 1, inlay)
    fill(d, 4, 4, 1, LH - 8, inlay); fill(d, LW - 5, 4, 1, LH - 8, inlay)
    # studs
    for (x, y) in [(5, 5), (LW - 6, 5), (5, LH - 6), (LW - 6, LH - 6)]:
        fill(d, x - 1, y - 1, 2, 2, inlay)
    # borda da janela de arte
    fill(d, 7, 11, 78, 75, DARK)
    # faixa do nome
    fill(d, 6, 92, LW - 13, 14, WOODLO)
    fill(d, 7, 93, LW - 15, 1, WOOD)
    # gemas de raridade
    ng = GEMS[tier]
    for i in range(4):
        fill(d, LW // 2 - 8 + i * 5, 112, 3, 3, inlay if i < ng else GEM_EMPTY)
    # abre a janela de arte (transparente); a borda de 1px em DARK permanece ao redor
    window = (8 * S, 12 * S, 84 * S, 85 * S)  # x8..83, y12..84 inclusivo
    d.rectangle([window[0], window[1], window[2] - 1, window[3] - 1], fill=(0, 0, 0, 0))

    # moedas vazias por cima da janela (dígitos entram em runtime)
    for (cx, cy) in [(46, 16), (46, 81), (12, 48), (80, 48)]:
        blob(d, cx, cy, 11, DARK)
        blob(d, cx, cy, 10, (198, 150, 78, 255))
        blob(d, cx, cy, 8.2, (240, 211, 150, 255))
    return img

if __name__ == "__main__":
    import os
    os.makedirs(OUT, exist_ok=True)
    for tier in RAR:
        make(tier).save(f"{OUT}/{tier}.png")
        print("ok", tier)
