#!/usr/bin/env python3
"""Demo: carta de Pastinaga com fundo de plantação por composição procedural."""
from PIL import Image, ImageDraw, ImageFont

XNB = "/private/tmp/claude-501/-Volumes-SSD-Work-Workspace-LazyMugSDVMods/fced3107-da36-458c-a9ca-44020c5ce3cc/scratchpad/xnb"
OUT = "/Volumes/SSD_Work/Workspace/LazyMugSDVMods/ValleyTriad/card-previews"

spring = Image.open(f"{XNB}/springobjects.png").convert("RGBA")
crops  = Image.open(f"{XNB}/crops.png").convert("RGBA")
FB = "/System/Library/Fonts/Supplemental/Arial Bold.ttf"
f_num, f_name, f_badge = ImageFont.truetype(FB, 26), ImageFont.truetype(FB, 15), ImageFont.truetype(FB, 14)

parsnip_icon  = spring.crop((0, 0, 16, 16))                # (O)24 -> col0,row1? recalc below
def obj(idx):
    return spring.crop((idx % 24 * 16, idx // 24 * 16, idx % 24 * 16 + 16, idx // 24 * 16 + 16))
parsnip_icon  = obj(24)
plant_mature  = crops.crop((96, 0, 112, 32))              # frame 6 (maduro, com raiz)
plant_mid     = crops.crop((80, 0, 96, 32))              # frame 5

def lerp(a, b, t): return tuple(int(a[i] + (b[i]-a[i])*t) for i in range(3))

def scene(w, h):
    """Fundo procedural: céu em gradiente + solo com sulcos + fileiras de pastinaga."""
    img = Image.new("RGBA", (w, h))
    d = ImageDraw.Draw(img)
    horizon = int(h * 0.42)
    sky_top, sky_bot = (139, 197, 232), (205, 228, 235)
    for y in range(horizon):
        d.line([(0, y), (w, y)], fill=lerp(sky_top, sky_bot, y / horizon) + (255,))
    soil_top, soil_bot = (126, 92, 58), (92, 64, 40)
    for y in range(horizon, h):
        t = (y - horizon) / (h - horizon)
        d.line([(0, y), (w, y)], fill=lerp(soil_top, soil_bot, t) + (255,))
    # sulcos (linhas mais escuras que se aproximam)
    yy, step = horizon + 4, 5
    while yy < h:
        d.line([(0, yy), (w, yy)], fill=(70, 48, 30, 110))
        step += 2; yy += step
    # fileiras de pastinaga recuando: base na linha, menor atrás -> maior à frente
    rows = [(horizon + 8, 1.2, 20, 4), (horizon + 30, 1.6, 22, -6),
            (horizon + 60, 2.1, 26, 6), (horizon + 100, 2.7, 32, -10)]
    for base_y, scale, spacing, off in rows:
        pw, ph = int(16 * scale), int(32 * scale)
        src = plant_mid if scale < 1.7 else plant_mature
        p = src.resize((pw, ph), Image.NEAREST)
        x = off - spacing
        while x < w:
            img.alpha_composite(p, (x, int(base_y) - ph))
            x += spacing
    return img

def card_field(name, tier, elem, stats):
    W, H, R, B = 150, 200, 16, 6
    RAR = {"Comum": (127,143,109), "Incomum": (74,143,192), "Raro": (181,106,46), "Lendário": (199,160,23)}
    card = Image.new("RGBA", (W, H), (0,0,0,0))
    d = ImageDraw.Draw(card)
    # cena recortada no retângulo interno
    iw, ih = W - 2*B, H - 2*B
    sc = scene(iw, ih)
    mask = Image.new("L", (iw, ih), 0)
    ImageDraw.Draw(mask).rounded_rectangle([0, 0, iw-1, ih-1], radius=R-4, fill=255)
    card.paste(sc, (B, B), mask)
    # vinheta suave só no rodapé (legibilidade do nome) — alpha_composite preserva a cena
    vg = Image.new("RGBA", (iw, ih), (0,0,0,0)); dv = ImageDraw.Draw(vg)
    for y in range(ih):
        a = int(70 * max(0, (y - ih*0.72) / (ih*0.28)))
        dv.line([(0,y),(iw,y)], fill=(20,25,15,a))
    vg.putalpha(Image.composite(vg.getchannel("A"), Image.new("L",(iw,ih),0), mask))
    card.alpha_composite(vg, (B,B))
    # borda de raridade
    d.rounded_rectangle([0,0,W-1,H-1], radius=R, outline=RAR[tier], width=B)
    # herói: parsnip colhido em destaque (menor, com sombra) sobre a plantação
    hero = parsnip_icon.resize((58,58), Image.NEAREST)
    sh = Image.new("RGBA",(58,58),(0,0,0,0)); ImageDraw.Draw(sh).ellipse([8,48,50,58],fill=(0,0,0,90))
    card.alpha_composite(sh,(W//2-29, 96)); card.alpha_composite(hero,(W//2-29, 84))
    # faixa do nome
    d.rounded_rectangle([B+3,H-30,W-B-3,H-B-2], radius=6, fill=(20,20,15,150))
    d.text((W//2,H-16), name, font=f_name, fill=(255,255,255), anchor="mm", stroke_width=3, stroke_fill=(20,25,15))
    # números
    N,L,S,O = stats; tc,sc2=(255,255,255),(20,25,15)
    d.text((W//2,10), "A" if N==10 else str(N), font=f_num, fill=tc, anchor="mt", stroke_width=4, stroke_fill=sc2)
    d.text((W//2,H-46), str(S), font=f_num, fill=tc, anchor="mm", stroke_width=4, stroke_fill=sc2)
    d.text((13,H//2-16), str(O), font=f_num, fill=tc, anchor="lt", stroke_width=4, stroke_fill=sc2)
    d.text((W-15,H//2-16), str(L), font=f_num, fill=tc, anchor="rt", stroke_width=4, stroke_fill=sc2)
    # selo estação
    col={"Primavera":(79,154,69)}[elem]; cx,cy,r=W-20,20,13
    d.ellipse([cx-r,cy-r,cx+r,cy+r], fill=col+(255,), outline=(244,240,228), width=2)
    d.text((cx,cy),"P",font=f_badge,fill=(255,255,255),anchor="mm")
    return card

c = card_field("Pastinaga", "Comum", "Primavera", (3,1,2,2))
c.save(f"{OUT}/pastinaga_plantacao.png")
# comparativo lado a lado com a versão simples
simple = Image.open(f"{OUT}/pastinaga.png")
comp = Image.new("RGBA", (c.width*2 + 60, c.height + 40), (58,63,46,255))
comp.alpha_composite(simple, (20, 20)); comp.alpha_composite(c, (c.width+40, 20))
comp.save(f"{OUT}/comparativo_pastinaga.png")
print("ok", c.size)
