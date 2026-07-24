#!/usr/bin/env python3
"""Redimensiona/otimiza a arte gerada para a resolução exata que o mod desenha.

Elimina o inchaço (5 MB -> dezenas de KB) e evita blur: como o jogo desenha 1:1 e depois
apenas REDUZ o card na mesa, entregar na resolução-alvo garante nitidez.

Uso:
    python3 fit_art.py            # ajusta assets/art/*.png -> 304x292 e assets/frames/*.png -> 368x512
    python3 fit_art.py --punch    # nas molduras, ainda torna a janela de arte transparente

A janela de arte (para --punch) fica em x32..335, y48..339 px (WX*S..(WX+WW)*S, etc).
"""
import os, sys
from PIL import Image

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ART_DIR = os.path.join(ROOT, "assets", "art")
FRAME_DIR = os.path.join(ROOT, "assets", "frames")
ART_SIZE = (304, 292)     # janela de arte da carta (WW*S x WH*S)
FRAME_SIZE = (368, 512)   # card inteiro (LW*S x LH*S)
WINDOW = (32, 48, 336, 340)  # x0,y0,x1,y1 px — janela transparente na moldura

def fit(path, size):
    im = Image.open(path).convert("RGBA")
    if im.size != size:
        im = im.resize(size, Image.LANCZOS)
    im.save(path, optimize=True)
    return im

def punch_window(path):
    im = Image.open(path).convert("RGBA")
    px = im.load()
    for y in range(WINDOW[1], WINDOW[3]):
        for x in range(WINDOW[0], WINDOW[2]):
            px[x, y] = (0, 0, 0, 0)
    im.save(path, optimize=True)

def run(folder, size, punch=False):
    if not os.path.isdir(folder):
        return
    for f in sorted(os.listdir(folder)):
        ext = os.path.splitext(f)[1].lower()
        if ext not in (".png", ".jpg", ".jpeg"):
            continue
        src = os.path.join(folder, f)
        before = os.path.getsize(src)
        # sempre normaliza para .png (o mod só carrega .png; JPG não tem transparência)
        dst = os.path.join(folder, os.path.splitext(f)[0] + ".png")
        im = Image.open(src).convert("RGBA")
        if im.size != size:
            im = im.resize(size, Image.LANCZOS)
        im.save(dst, optimize=True)
        if ext != ".png":
            os.remove(src)  # remove o jpg original após converter
        if punch:
            punch_window(dst)
        after = os.path.getsize(dst)
        note = " (jpg→png)" if ext != ".png" else ""
        print(f"  {f} -> {os.path.basename(dst)}{note}: {before//1024} KB -> {after//1024} KB")

if __name__ == "__main__":
    punch = "--punch" in sys.argv
    print("Arte das cartas (assets/art -> 304x292):")
    run(ART_DIR, ART_SIZE)
    print("Molduras (assets/frames -> 368x512" + (" + janela transparente" if punch else "") + "):")
    run(FRAME_DIR, FRAME_SIZE, punch=punch)
    print("Pronto.")
