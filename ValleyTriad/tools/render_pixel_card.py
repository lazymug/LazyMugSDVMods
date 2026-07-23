#!/usr/bin/env python3
"""Carta em pixel art (chassi + cena procedural) — estilo fofo SDV, sprite destacado."""
from PIL import Image, ImageDraw, ImageFont

XNB = "/private/tmp/claude-501/-Volumes-SSD-Work-Workspace-LazyMugSDVMods/fced3107-da36-458c-a9ca-44020c5ce3cc/scratchpad/xnb"
OUT = "/Volumes/SSD_Work/Workspace/LazyMugSDVMods/ValleyTriad/card-previews"
S = 6  # fator de upscale (nearest) para o look pixel

spring = Image.open(f"{XNB}/springobjects.png").convert("RGBA")
crops  = Image.open(f"{XNB}/crops.png").convert("RGBA")
def obj(i): return spring.crop((i%24*16, i//24*16, i%24*16+16, i//24*16+16))
plant_mature = crops.crop((96,0,112,32))
plant_mid    = crops.crop((80,0,96,32))

# paleta madeira SDV fofo
DARK=(59,38,20); WOOD=(139,94,52); WOODHI=(178,131,80); WOODLO=(101,66,36)
RAR={"Comum":(111,161,74),"Incomum":(74,143,192),"Raro":(196,122,46),"Lendário":(217,181,58)}
SEASON={"Primavera":(79,170,69),"Verão":(224,168,40),"Outono":(210,120,50),"Inverno":(90,165,205),"Nenhum":None}

def outline(spr, col=(40,26,14,255), r=1):
    w,h=spr.size; pad=r*2
    base=Image.new("RGBA",(w+pad,h+pad),(0,0,0,0))
    sil=Image.new("RGBA",(w,h),(0,0,0,0)); sil.paste(Image.new("RGBA",(w,h),col),(0,0),spr)
    for dx in range(-r,r+1):
        for dy in range(-r,r+1):
            if dx or dy: base.alpha_composite(sil,(r+dx,r+dy))
    base.alpha_composite(spr,(r,r))
    return base

def scene(w,h,hero,season):
    img=Image.new("RGBA",(w,h)); d=ImageDraw.Draw(img)
    horizon=int(h*0.44)
    sky=[(150,200,232),(176,214,235),(198,224,236)]
    for i,y in enumerate(range(0,horizon)):
        d.line([(0,y),(w,y)],fill=sky[min(len(sky)-1,i*len(sky)//horizon)])
    soil=[(126,92,58),(112,80,48),(96,66,40)]
    for i,y in enumerate(range(horizon,h)):
        d.line([(0,y),(w,y)],fill=soil[min(len(soil)-1,(y-horizon)*len(soil)//(h-horizon))])
    yy=horizon+2; step=3
    while yy<h:
        d.line([(0,yy),(w,yy)],fill=(78,54,32)); step+=1; yy+=step
    # fileiras (atrás menor -> frente maior)
    for base_y,scl,sp,off in [(horizon+3,0.8,10,2),(horizon+13,1.05,12,-4),(horizon+30,1.35,15,3)]:
        src=plant_mid if scl<1 else plant_mature
        pw,ph=max(6,int(16*scl)),max(12,int(32*scl))
        p=src.resize((pw,ph),Image.NEAREST)
        x=off-sp
        while x<w: img.alpha_composite(p,(x,int(base_y)-ph)); x+=sp
    # escurece bordas (vinheta pixel) p/ destacar o herói
    vg=Image.new("RGBA",(w,h),(0,0,0,0)); dv=ImageDraw.Draw(vg)
    for i in range(4):
        a=44-i*10
        dv.rectangle([i,i,w-1-i,h-1-i],outline=(20,24,14,max(0,a)))
    img.alpha_composite(vg)
    # spotlight atrás do herói
    cx,cy=w//2,int(h*0.60)
    glow=Image.new("RGBA",(w,h),(0,0,0,0)); dg=ImageDraw.Draw(glow)
    for rr,a in [(20,60),(14,70),(9,80)]:
        dg.ellipse([cx-rr,cy-rr//2-2,cx+rr,cy+rr//2+2],fill=(255,244,200,a))
    img.alpha_composite(glow)
    # sombra + herói com contorno
    d.ellipse([cx-11,cy+8,cx+11,cy+14],fill=(0,0,0,90))
    hs=outline(hero.resize((32,32),Image.NEAREST))
    img.alpha_composite(hs,(cx-hs.width//2, cy-20))
    return img

def coin1x(d,cx,cy,r,inlay):
    d.ellipse([cx-r,cy-r,cx+r,cy+r], fill=(198,150,78), outline=DARK)
    d.ellipse([cx-r+1,cy-r+1,cx+r-1,cy-1], fill=(240,211,150))
    d.ellipse([cx-r,cy-r,cx+r,cy+r], outline=inlay)  # aro na cor da raridade

def pixel_card(name, tier, elem, stats, sprite):
    W,H=92,128
    c=Image.new("RGBA",(W,H),(0,0,0,0)); d=ImageDraw.Draw(c)
    inlay=RAR[tier]
    # corpo com cantos chanfrados
    d.rectangle([2,0,W-3,H-1],fill=WOOD); d.rectangle([0,2,W-1,H-3],fill=WOOD)
    d.rectangle([2,0,W-3,0],fill=DARK); d.rectangle([2,H-1,W-3,H-1],fill=DARK)
    d.rectangle([0,2,0,H-3],fill=DARK); d.rectangle([W-1,2,W-1,H-3],fill=DARK)
    d.line([(2,0),(0,2)],fill=DARK); d.line([(W-3,0),(W-1,2)],fill=DARK)
    d.line([(2,H-1),(0,H-3)],fill=DARK); d.line([(W-3,H-1),(W-1,H-3)],fill=DARK)
    d.rectangle([3,1,W-4,1],fill=WOODHI); d.rectangle([2,H-3,W-3,H-3],fill=WOODLO)
    d.rectangle([4,4,W-5,H-5],outline=inlay,width=1)
    for (x,y) in [(5,5),(W-6,5),(5,H-6),(W-6,H-6)]:
        d.ellipse([x-1,y-1,x+1,y+1],fill=inlay,outline=DARK)
    # janela de arte (maior, menos espaço morto)
    ax0,ay0,ax1,ay1=7,11,W-8,86
    sc=scene(ax1-ax0, ay1-ay0, sprite, SEASON[elem])
    c.alpha_composite(sc,(ax0,ay0))
    d.rectangle([ax0-1,ay0-1,ax1,ay1],outline=DARK,width=1)
    d.rectangle([ax0-2,ay0-2,ax1+1,ay1+1],outline=WOODLO,width=1)
    # faixa do nome
    d.rectangle([6,92,W-7,106],fill=WOODLO,outline=DARK)
    d.rectangle([7,93,W-8,94],fill=WOOD)
    # gemas de raridade
    ng={"Comum":1,"Incomum":2,"Raro":3,"Lendário":4}[tier]
    gx=W//2-8
    for i in range(4):
        cx=gx+i*5+2; cy=114
        if i<ng: d.polygon([(cx,cy-2),(cx+2,cy),(cx,cy+2),(cx-2,cy)],fill=inlay,outline=DARK)
        else:    d.polygon([(cx,cy-2),(cx+2,cy),(cx,cy+2),(cx-2,cy)],fill=(90,66,38))
    # moedas de valor (pixel) nas bordas da janela
    edges={"N":((ax0+ax1)//2,ay0),"S":((ax0+ax1)//2,ay1),"O":(ax0,(ay0+ay1)//2),"L":(ax1,(ay0+ay1)//2)}
    for cx,cy in edges.values(): coin1x(d,cx,cy,7,inlay)
    # selo de estação (pixel) no canto sup direito da janela
    se=SEASON[elem]; badge=None
    if se:
        bx,by=ax1-3,ay0+3
        d.ellipse([bx-6,by-6,bx+6,by+6],fill=se,outline=(245,233,205))
        d.ellipse([bx-6,by-6,bx+6,by+6],outline=DARK); badge=(bx,by)
    return c,{k:v for k,v in edges.items()},badge,(ax0,ay0),stats,name,elem

def finalize(name, tier, elem, stats, sprite):
    card,edges,badge,_,st,nm,el=pixel_card(name,tier,elem,stats,sprite)
    big=card.resize((card.width*S, card.height*S), Image.NEAREST)
    d=ImageDraw.Draw(big)
    FB="/System/Library/Fonts/Supplemental/Arial Bold.ttf"
    fnum=ImageFont.truetype(FB,40); fname=ImageFont.truetype(FB,30); fbadge=ImageFont.truetype(FB,24)
    def Nv(n): return "A" if n==10 else str(n)
    W,H=card.width,card.height
    vals={"N":st[0],"L":st[1],"S":st[2],"O":st[3]}
    for k,(cx,cy) in edges.items():
        d.text((cx*S,cy*S),Nv(vals[k]),font=fnum,fill=(40,26,14),anchor="mm",
               stroke_width=4,stroke_fill=(245,224,170))
    d.text((W*S//2, 99*S), nm, font=fname, fill=(245,233,205), anchor="mm",
           stroke_width=4, stroke_fill=(40,26,14))
    if badge:
        bx,by=badge
        d.text((bx*S,by*S), el[0], font=fbadge, fill=(255,255,255), anchor="mm",
               stroke_width=3, stroke_fill=(30,40,20))
    return big

if __name__=="__main__":
    import os; os.makedirs(OUT,exist_ok=True)
    demos=[
        ("Pastinaga","Comum","Primavera",(3,1,2,2),obj(24)),
        ("Abóbora","Comum","Outono",(6,3,4,1),obj(276)),
    ]
    imgs=[finalize(*args) for args in demos]
    imgs[0].save(f"{OUT}/pixel_pastinaga.png")
    # contact
    pad=24; bg=(58,63,46,255); cw,ch=imgs[0].size
    sheet=Image.new("RGBA",(len(imgs)*cw+pad*(len(imgs)+1), ch+pad*2),bg)
    for i,im in enumerate(imgs): sheet.alpha_composite(im,(pad+i*(cw+pad),pad))
    sheet.save(f"{OUT}/pixel_demo.png")
    print("ok", imgs[0].size)
