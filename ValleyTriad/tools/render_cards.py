#!/usr/bin/env python3
"""Compõe cartas de exemplo do Valley Triad usando os sprites reais do jogo."""
import os
from PIL import Image, ImageDraw, ImageFont

XNB = "/private/tmp/claude-501/-Volumes-SSD-Work-Workspace-LazyMugSDVMods/fced3107-da36-458c-a9ca-44020c5ce3cc/scratchpad/xnb"
OUT = "/Volumes/SSD_Work/Workspace/LazyMugSDVMods/ValleyTriad/card-previews"
os.makedirs(OUT, exist_ok=True)

spring = Image.open(f"{XNB}/springobjects.png").convert("RGBA")
abigail = Image.open(f"{XNB}/abigail.png").convert("RGBA")

FB = "/System/Library/Fonts/Supplemental/Arial Bold.ttf"
f_num  = ImageFont.truetype(FB, 26)
f_name = ImageFont.truetype(FB, 15)
f_badge= ImageFont.truetype(FB, 14)

RARITY = {  # cor da borda
    "Comum":    (127, 143, 109), "Incomum": (74, 143, 192),
    "Raro":     (181, 106, 46),  "Lendário": (199, 160, 23),
}
SEASON = {  # (cor, letra)
    "Primavera": ((79, 154, 69), "P"), "Verão": ((217, 154, 32), "V"),
    "Outono":    ((197, 106, 41), "O"), "Inverno": ((76, 141, 176), "I"), "Nenhum": None,
}

def obj_sprite(idx):
    col, row = idx % 24, idx // 24
    return spring.crop((col*16, row*16, col*16+16, row*16+16))

def num(n): return "A" if n == 10 else str(n)

def draw_card(name, tier, elem, stats, sprite):
    W, H, R, B = 150, 200, 16, 6
    card = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    d = ImageDraw.Draw(card)
    # fundo pergaminho + borda de raridade
    border = RARITY[tier]
    d.rounded_rectangle([0, 0, W-1, H-1], radius=R, fill=(244, 240, 228, 255), outline=border, width=B)
    d.rounded_rectangle([B+2, B+2, W-B-3, H-B-3], radius=R-6, outline=(0, 0, 0, 30), width=1)
    # sprite (upscale nearest)
    box = 92
    spr = sprite.resize((box, box), Image.NEAREST)
    card.alpha_composite(spr, ((W-box)//2, 44))
    # faixa do nome
    d.rounded_rectangle([B+3, H-30, W-B-3, H-B-2], radius=6, fill=(0, 0, 0, 22))
    d.text((W//2, H-16), name, font=f_name, fill=(35, 40, 29), anchor="mm",
           stroke_width=3, stroke_fill=(244, 240, 228))
    # números N/L/S/O
    N, L, S, O = stats
    tcol, scol = (30, 34, 24), (255, 255, 255)
    d.text((W//2, 10),    num(N), font=f_num, fill=tcol, anchor="mt", stroke_width=3, stroke_fill=scol)
    d.text((W//2, H-46),  num(S), font=f_num, fill=tcol, anchor="mm", stroke_width=3, stroke_fill=scol)
    d.text((13, H//2-16), num(O), font=f_num, fill=tcol, anchor="lt", stroke_width=3, stroke_fill=scol)
    d.text((W-15, H//2-16),num(L),font=f_num, fill=tcol, anchor="rt", stroke_width=3, stroke_fill=scol)
    # selo de estação
    se = SEASON.get(elem)
    if se:
        col, letter = se
        cx, cy, r = W-20, 20, 13
        d.ellipse([cx-r, cy-r, cx+r, cy+r], fill=col+(255,), outline=(244, 240, 228), width=2)
        d.text((cx, cy), letter, font=f_badge, fill=(255, 255, 255), anchor="mm")
    return card

# (nome, tier, elemento, (N,L,S,O), sprite)
ab = abigail.crop((0, 0, 64, 64))
cards = [
    ("Pastinaga",   "Comum",    "Primavera", (3,1,2,2),  obj_sprite(24)),
    ("Abóbora",     "Comum",    "Outono",    (6,3,4,1),  obj_sprite(276)),
    ("Mirtilo",     "Comum",    "Verão",     (4,2,4,3),  obj_sprite(258)),
    ("Abigail",     "Incomum",  "Nenhum",    (6,6,4,5),  ab),
    ("Diamante",    "Raro",     "Nenhum",    (7,6,7,6),  obj_sprite(72)),
    ("Fruta Anc.",  "Raro",     "Nenhum",    (7,7,6,6),  obj_sprite(454)),
    ("Esturjão",    "Raro",     "Nenhum",    (6,6,8,5),  obj_sprite(698)),
    ("P. Carmesim", "Lendário", "Nenhum",    (9,8,9,7),  obj_sprite(159)),
    ("Frag. Prism.","Lendário", "Nenhum",    (10,9,9,8), obj_sprite(74)),
]

imgs = []
for name, tier, elem, stats, spr in cards:
    c = draw_card(name, tier, elem, stats, spr)
    fn = name.lower().replace(" ", "_").replace(".", "").replace("á","a").replace("ó","o").replace("ã","a").replace("ê","e")
    c.save(f"{OUT}/{fn}.png")
    imgs.append(c)

# contact sheet 3x3
cols, pad, bg = 3, 22, (58, 63, 46, 255)
cw, ch = imgs[0].size
rows = (len(imgs) + cols - 1) // cols
sheet = Image.new("RGBA", (cols*cw + pad*(cols+1), rows*ch + pad*(rows+1)), bg)
for i, im in enumerate(imgs):
    r, c = divmod(i, cols)
    sheet.alpha_composite(im, (pad + c*(cw+pad), pad + r*(ch+pad)))
sheet.save(f"{OUT}/gallery.png")
print("salvo em", OUT, "| cartas:", len(imgs), "| gallery:", sheet.size)
