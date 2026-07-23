#!/usr/bin/env python3
"""Carta pixel-art: chassi fofo SDV + campo com perspectiva + fonte pixel desenhada."""
from PIL import Image, ImageDraw, ImageEnhance
import random

XNB = "/private/tmp/claude-501/-Volumes-SSD-Work-Workspace-LazyMugSDVMods/fced3107-da36-458c-a9ca-44020c5ce3cc/scratchpad/xnb"
OUT = "/Volumes/SSD_Work/Workspace/LazyMugSDVMods/ValleyTriad/card-previews"
S = 6

spring = Image.open(f"{XNB}/springobjects.png").convert("RGBA")
crops  = Image.open(f"{XNB}/crops.png").convert("RGBA")
def obj(i): return spring.crop((i%24*16, i//24*16, i%24*16+16, i//24*16+16))
plant_leafy = crops.crop((96,0,112,32))   # pastinaga madura (folhagem verde genérica)
plant_leafy = ImageEnhance.Brightness(plant_leafy).enhance(1.18)  # mais viçosa
chicken = Image.open(f"{XNB}/chicken.png").convert("RGBA").crop((0,0,16,16))  # frame idle

DARK=(53,34,18); WOOD=(139,94,52); WOODHI=(178,131,80); WOODLO=(101,66,36)
RAR={"Comum":(111,161,74),"Incomum":(74,143,192),"Raro":(196,122,46),"Lendário":(217,181,58)}
SEASON={"Primavera":(79,170,69),"Verão":(224,168,40),"Outono":(210,120,50),"Inverno":(90,165,205),"Nenhum":None}

# ---------------- fonte pixel 5x7 ----------------
F = {
'0':["01110","10001","10011","10101","11001","10001","01110"],
'1':["00100","01100","00100","00100","00100","00100","01110"],
'2':["01110","10001","00001","00110","01000","10000","11111"],
'3':["11111","00010","00100","00010","00001","10001","01110"],
'4':["00010","00110","01010","10010","11111","00010","00010"],
'5':["11111","10000","11110","00001","00001","10001","01110"],
'6':["00110","01000","10000","11110","10001","10001","01110"],
'7':["11111","00001","00010","00100","01000","01000","01000"],
'8':["01110","10001","10001","01110","10001","10001","01110"],
'9':["01110","10001","10001","01111","00001","00010","01100"],
'A':["01110","10001","10001","11111","10001","10001","10001"],
'B':["11110","10001","10001","11110","10001","10001","11110"],
'C':["01110","10001","10000","10000","10000","10001","01110"],
'D':["11100","10010","10001","10001","10001","10010","11100"],
'E':["11111","10000","10000","11100","10000","10000","11111"],
'F':["11111","10000","10000","11100","10000","10000","10000"],
'G':["01110","10001","10000","10111","10001","10001","01110"],
'H':["10001","10001","10001","11111","10001","10001","10001"],
'I':["01110","00100","00100","00100","00100","00100","01110"],
'J':["00111","00010","00010","00010","10010","10010","01100"],
'K':["10001","10010","10100","11000","10100","10010","10001"],
'L':["10000","10000","10000","10000","10000","10000","11111"],
'M':["10001","11011","10101","10101","10001","10001","10001"],
'N':["10001","10001","11001","10101","10011","10001","10001"],
'O':["01110","10001","10001","10001","10001","10001","01110"],
'P':["11110","10001","10001","11110","10000","10000","10000"],
'Q':["01110","10001","10001","10001","10101","10010","01101"],
'R':["11110","10001","10001","11110","10100","10010","10001"],
'S':["01110","10001","10000","01110","00001","10001","01110"],
'T':["11111","00100","00100","00100","00100","00100","00100"],
'U':["10001","10001","10001","10001","10001","10001","01110"],
'V':["10001","10001","10001","10001","10001","01010","00100"],
'W':["10001","10001","10001","10101","10101","11011","10001"],
'X':["10001","10001","01010","00100","01010","10001","10001"],
'Y':["10001","10001","01010","00100","00100","00100","00100"],
'Z':["11111","00001","00010","00100","01000","10000","11111"],
'-':["00000","00000","00000","11111","00000","00000","00000"],
' ':["00000","00000","00000","00000","00000","00000","00000"],
}
ACC={'acute':["00010","00100"],'grave':["01000","00100"],
     'til':["01101","10010"],'circ':["00100","01010"]}
ACCENTED={'Á':('A','acute'),'À':('A','grave'),'Ã':('A','til'),'Â':('A','circ'),
          'É':('E','acute'),'Ê':('E','circ'),'Í':('I','acute'),
          'Ó':('O','acute'),'Ô':('O','circ'),'Õ':('O','til'),'Ú':('U','acute'),'Ç':('C','ced')}

def glyph(ch):
    ch=ch.upper()
    if ch in ACCENTED:
        base,acc=ACCENTED[ch]; return F.get(base,F[' ']),acc
    return F.get(ch,F[' ']),None

def text_w(text): return len(text)*6-1  # em "pixels de fonte"

def draw_ptext(d, text, cx, cy, blk, fill, outline):
    total=text_w(text)*blk
    x0=cx-total//2; y0=cy-(7*blk)//2
    def block(px,py,col):
        d.rectangle([px,py,px+blk-1,py+blk-1],fill=col)
    for ci,ch in enumerate(text):
        rows,acc=glyph(ch)
        gx=x0+ci*6*blk
        pts=[(c,r) for r,row in enumerate(rows) for c,v in enumerate(row) if v=='1']
        if acc=='ced': pts+= [(1,7),(1,8)]  # cedilha simples
        elif acc in ACC:
            for r,row in enumerate(ACC[acc]):
                for c,v in enumerate(row):
                    if v=='1': pts.append((c,r-2))
        # contorno (8-viz)
        for (c,r) in pts:
            X=gx+c*blk; Y=y0+r*blk
            for ox,oy in [(-1,0),(1,0),(0,-1),(0,1),(-1,-1),(1,1),(1,-1),(-1,1)]:
                block(X+ox*blk,Y+oy*blk,outline)
        for (c,r) in pts:
            block(gx+c*blk, y0+r*blk, fill)

# ---------------- cena com perspectiva ----------------
def outline_spr(spr,col=(38,24,12,255),r=1):
    w,h=spr.size; base=Image.new("RGBA",(w+2*r,h+2*r),(0,0,0,0))
    sil=Image.new("RGBA",(w,h),(0,0,0,0)); sil.paste(Image.new("RGBA",(w,h),col),(0,0),spr)
    for dx in range(-r,r+1):
        for dy in range(-r,r+1):
            if dx or dy: base.alpha_composite(sil,(r+dx,r+dy))
    base.alpha_composite(spr,(r,r)); return base

def lerp(a,b,t): return tuple(int(a[i]+(b[i]-a[i])*t) for i in range(3))
def vgrad(d,x0,y0,x1,y1,c0,c1):
    for y in range(y0,y1):
        t=(y-y0)/max(1,(y1-y0)); d.line([(x0,y),(x1,y)],fill=lerp(c0,c1,t))

def bg_field(d,w,h):
    hz=int(h*0.40)
    for y in range(hz):
        sky=[(250,226,168),(214,224,224),(176,208,226),(150,196,224)]
        d.line([(0,y),(w,y)],fill=sky[min(3,y*4//hz)])
    d.rectangle([0,hz-4,w,hz+1],fill=(78,120,58))
    for x in range(0,w,4): d.ellipse([x-2,hz-7,x+3,hz+1],fill=(90,132,64))
    vgrad(d,0,hz,w,h,(120,88,56),(80,58,36))
    vp=(w//2,hz)
    for bx in range(-w,2*w,16): d.line([(bx,h),vp],fill=(116,84,52))
    haze=(150,190,178)
    def clump(cx,cy,r,t):
        dk=lerp((44,88,38),haze,(1-t)*0.45); md=lerp((74,134,54),haze,(1-t)*0.45); lt=lerp((126,182,80),haze,(1-t)*0.4)
        d.ellipse([cx-r,cy-r*0.5,cx+r,cy+r*0.55],fill=dk)
        d.ellipse([cx-r*0.92,cy-r*0.85,cx+r*0.92,cy+r*0.4],fill=md)
        d.ellipse([cx-r*0.55,cy-r*0.95,cx+r*0.45,cy-r*0.1],fill=lt)
    rows=[(hz+2,1.4),(hz+6,1.9),(hz+11,2.5),(hz+18,3.2),(hz+27,4.1),(hz+39,5.1),(hz+55,6.4)]
    for i,(ry,r) in enumerate(rows):
        t=(i+1)/len(rows); step=max(3,int(r*1.25)); x=-r+(i%2)*step//2
        while x<w+r: clump(x,ry,r,t); x+=step

def bg_pasture(d,w,h):
    hz=int(h*0.42)
    for y in range(hz): d.line([(0,y),(w,y)],fill=lerp((150,200,232),(206,228,236),y/hz))
    vgrad(d,0,hz,w,h,(120,176,84),(86,140,60))
    for cx in range(6,w,14):  # colinas ao fundo
        d.ellipse([cx-10,hz-4,cx+10,hz+8],fill=(104,158,72))
    # cerca
    fy=int(h*0.6)
    d.rectangle([0,fy,w,fy+2],fill=(120,86,52)); d.rectangle([0,fy+7,w,fy+9],fill=(120,86,52))
    for px in range(6,w,16): d.rectangle([px,fy-4,px+2,fy+12],fill=(96,66,38))
    for gx in range(3,w,6):  # tufos de grama
        d.line([(gx,h-3),(gx,h-8)],fill=(70,124,52)); d.line([(gx+2,h-2),(gx+2,h-6)],fill=(84,140,62))

def bg_mine(d,w,h):
    vgrad(d,0,0,w,h,(64,54,60),(34,28,34))
    random.seed(7)
    for _ in range(60):  # textura de rocha
        x,y=random.randint(0,w),random.randint(0,h); r=random.randint(1,3)
        col=random.choice([(52,44,50),(78,66,74)]); d.ellipse([x-r,y-r,x+r,y+r],fill=col)
    d.rectangle([0,h-8,w,h],fill=(46,38,42))  # chão
    # cristais recuando
    gems=[(150,90,200),(90,200,150),(90,150,220),(210,120,90)]
    random.seed(3)
    for i in range(22):
        t=random.random(); yy=int(h*0.45+t*h*0.5); xx=random.randint(4,w-4); s=int(2+t*5)
        c=random.choice(gems); c=lerp(c,(60,54,60),(1-t)*0.4)
        d.polygon([(xx,yy-s*2),(xx+s,yy),(xx,yy+s),(xx-s,yy)],fill=c)
        d.line([(xx,yy-s*2),(xx,yy)],fill=lerp(c,(255,255,255),0.4))
    # brilho de tocha
    gl=Image.new("RGBA",(w,h),(0,0,0,0)); dg=ImageDraw.Draw(gl)
    for rr,a in [(30,50),(20,60),(10,70)]: dg.ellipse([10-rr,12-rr,10+rr,12+rr],fill=(255,180,90,a))
    return gl

def bg_sea(d,w,h):
    hz=int(h*0.46)
    for y in range(hz): d.line([(0,y),(w,y)],fill=lerp((250,214,150),(160,200,224),y/hz))
    sx=int(w*0.62)
    d.ellipse([sx-6,hz-10,sx+6,hz+2],fill=(255,238,180))  # sol
    for y in range(hz,h):
        t=(y-hz)/(h-hz); d.line([(0,y),(w,y)],fill=lerp((84,150,196),(28,74,132),t))
    for y in range(hz,h,3):  # reflexo do sol
        wob=2 if (y//3)%2 else -2
        d.line([(sx-4+wob,y),(sx+4+wob,y)],fill=(230,224,180))
    random.seed(5)
    for _ in range(40):  # cristas de onda
        x=random.randint(0,w); y=random.randint(hz+2,h-2); ln=random.randint(2,5)
        d.line([(x,y),(x+ln,y)],fill=(170,206,224))
    for x in range(0,w,5): d.line([(x,h-3),(x+2,h-3)],fill=(226,238,240))  # espuma

def bg_saloon(d,w,h):
    for y in range(h):  # parede de madeira
        d.line([(0,y),(w,y)],fill=(146,100,58) if (y//5)%2 else (134,90,50))
    for y in range(0,h,5): d.line([(0,y),(w,y)],fill=(112,74,40))
    d.rectangle([0,int(h*0.20),w,int(h*0.22)],fill=(92,60,32))  # prateleira
    random.seed(9); bx=6
    while bx<w-6:  # garrafas
        bc=random.choice([(70,150,90),(200,150,60),(160,60,60),(80,120,180)])
        d.rectangle([bx,int(h*0.20)-8,bx+3,int(h*0.20)-1],fill=bc); bx+=7
    d.rectangle([0,int(h*0.66),w,h],fill=(104,68,36))  # balcão
    d.rectangle([0,int(h*0.66),w,int(h*0.66)+2],fill=(150,104,60))
    gl=Image.new("RGBA",(w,h),(0,0,0,0)); dg=ImageDraw.Draw(gl)
    for rr,a in [(34,42),(22,52)]: dg.ellipse([w//2-rr,8-rr,w//2+rr,8+rr],fill=(255,214,140,a))
    return gl

def bg_night(d,w,h):
    vgrad(d,0,0,w,h,(24,26,64),(64,52,96))
    random.seed(11)
    for _ in range(50):  # estrelas
        x,y=random.randint(0,w),random.randint(0,int(h*0.6)); d.point((x,y),fill=(240,240,220))
    mx,my=int(w*0.72),int(h*0.2)  # lua
    d.ellipse([mx-8,my-8,mx+8,my+8],fill=(238,236,208))
    d.ellipse([mx-2,my-5,mx+6,my+4],fill=(224,222,196))
    d.polygon([(0,int(h*0.55)),(w*0.3,int(h*0.42)),(w*0.6,int(h*0.5)),(w,int(h*0.44)),(w,h),(0,h)],fill=(30,40,52))
    random.seed(2)
    for _ in range(14):  # vagalumes
        x,y=random.randint(4,w-4),random.randint(int(h*0.5),h-6); d.point((x,y),fill=(210,240,140))

BG={"field":bg_field,"pasture":bg_pasture,"mine":bg_mine,"sea":bg_sea,"saloon":bg_saloon,"night":bg_night}
HEROY={"saloon":0.62,"night":0.66}

def scene(w,h,hero,kind="field"):
    img=Image.new("RGBA",(w,h)); d=ImageDraw.Draw(img)
    overlay=BG.get(kind,bg_field)(d,w,h)
    if overlay is not None: img.alpha_composite(overlay)
    vg=Image.new("RGBA",(w,h),(0,0,0,0)); dv=ImageDraw.Draw(vg)
    for i in range(4): dv.rectangle([i,i,w-1-i,h-1-i],outline=(18,20,26,max(0,44-i*10)))
    img.alpha_composite(vg)
    cx,cy=w//2,int(h*HEROY.get(kind,0.70))
    gl=Image.new("RGBA",(w,h),(0,0,0,0)); dg=ImageDraw.Draw(gl)
    for rr,a in [(16,60),(11,72),(7,84)]: dg.ellipse([cx-rr,cy-rr//2-2,cx+rr,cy+rr//2+2],fill=(255,246,206,a))
    img.alpha_composite(gl)
    d.ellipse([cx-12,cy+9,cx+12,cy+15],fill=(0,0,0,95))
    hs=outline_spr(hero.resize((34,34),Image.NEAREST))
    img.alpha_composite(hs,(cx-hs.width//2,cy-22))
    return img

# ---------------- carta ----------------
def coin1x(d,cx,cy,r,inlay):
    d.ellipse([cx-r,cy-r,cx+r,cy+r],fill=(198,150,78),outline=DARK)
    d.ellipse([cx-r+1,cy-r+1,cx+r-1,cy-1],fill=(240,211,150))
    d.ellipse([cx-r,cy-r,cx+r,cy+r],outline=inlay)

def build(name,tier,elem,stats,sprite,kind="field"):
    W,H=92,128; c=Image.new("RGBA",(W,H),(0,0,0,0)); d=ImageDraw.Draw(c); inlay=RAR[tier]
    d.rectangle([2,0,W-3,H-1],fill=WOOD); d.rectangle([0,2,W-1,H-3],fill=WOOD)
    d.rectangle([2,0,W-3,0],fill=DARK); d.rectangle([2,H-1,W-3,H-1],fill=DARK)
    d.rectangle([0,2,0,H-3],fill=DARK); d.rectangle([W-1,2,W-1,H-3],fill=DARK)
    d.line([(2,0),(0,2)],fill=DARK); d.line([(W-3,0),(W-1,2)],fill=DARK)
    d.line([(2,H-1),(0,H-3)],fill=DARK); d.line([(W-3,H-1),(W-1,H-3)],fill=DARK)
    d.rectangle([3,1,W-4,1],fill=WOODHI); d.rectangle([2,H-3,W-3,H-3],fill=WOODLO)
    d.rectangle([4,4,W-5,H-5],outline=inlay,width=1)
    for (x,y) in [(5,5),(W-6,5),(5,H-6),(W-6,H-6)]: d.ellipse([x-1,y-1,x+1,y+1],fill=inlay,outline=DARK)
    ax0,ay0,ax1,ay1=7,11,W-8,86
    c.alpha_composite(scene(ax1-ax0,ay1-ay0,sprite,kind),(ax0,ay0))
    d.rectangle([ax0-1,ay0-1,ax1,ay1],outline=DARK,width=1)
    d.rectangle([ax0-2,ay0-2,ax1+1,ay1+1],outline=WOODLO,width=1)
    d.rectangle([6,92,W-7,106],fill=WOODLO,outline=DARK); d.rectangle([7,93,W-8,94],fill=WOOD)
    ng={"Comum":1,"Incomum":2,"Raro":3,"Lendário":4}[tier]; gx=W//2-8
    for i in range(4):
        cx=gx+i*5+2; cy=114
        d.polygon([(cx,cy-2),(cx+2,cy),(cx,cy+2),(cx-2,cy)],fill=inlay if i<ng else (90,66,38),outline=DARK if i<ng else None)
    edges={"N":((ax0+ax1)//2,ay0),"S":((ax0+ax1)//2,ay1),"O":(ax0,(ay0+ay1)//2),"L":(ax1,(ay0+ay1)//2)}
    for cx,cy in edges.values(): coin1x(d,cx,cy,7,inlay)
    badge=None; se=SEASON[elem]
    if se:
        bx,by=ax1-3,ay0+3
        d.ellipse([bx-6,by-6,bx+6,by+6],fill=se,outline=(245,233,205)); d.ellipse([bx-6,by-6,bx+6,by+6],outline=DARK)
        badge=(bx,by)
    return c,edges,badge,stats,name,elem

def finalize(name,tier,elem,stats,sprite,kind="field"):
    card,edges,badge,st,nm,el=build(name,tier,elem,stats,sprite,kind)
    big=card.resize((card.width*S,card.height*S),Image.NEAREST); d=ImageDraw.Draw(big)
    def Nv(n): return "A" if n==10 else str(n)
    vals={"N":st[0],"L":st[1],"S":st[2],"O":st[3]}
    for k,(cx,cy) in edges.items():
        draw_ptext(d,Nv(vals[k]),cx*S,cy*S,4,(44,28,14),(245,224,170))
    up=nm.upper()
    blk=5
    while text_w(up)*blk > (card.width-16)*S and blk>3: blk-=1
    draw_ptext(d,up,card.width*S//2,99*S,blk,(245,233,205),(44,28,14))
    if badge:
        bx,by=badge; draw_ptext(d,el[0],bx*S,by*S,3,(255,255,255),(30,40,20))
    return big

HERO_ABIGAIL=Image.open(f"{XNB}/abigail.png").convert("RGBA").crop((0,0,64,64))

if __name__=="__main__":
    import os; os.makedirs(OUT,exist_ok=True)
    # uma carta por categoria/cena (nome, tier, elem, stats, sprite, kind)
    demos=[
        ("Couve-flor","Comum","Primavera",(4,5,3,2),obj(190),"field"),
        ("Galinha","Comum","Nenhum",(2,2,3,2),chicken,"pasture"),
        ("Diamante","Raro","Nenhum",(7,6,7,6),obj(72),"mine"),
        ("Peixe Carmesim","Lendário","Nenhum",(9,8,9,7),obj(159),"sea"),
        ("Abigail","Incomum","Nenhum",(6,6,4,5),HERO_ABIGAIL,"saloon"),
        ("Frag. Prismático","Lendário","Nenhum",(10,9,9,8),obj(74),"night"),
    ]
    imgs=[finalize(*a) for a in demos]
    names=["field","pasture","mine","sea","saloon","night"]
    for im,nm in zip(imgs,names): im.save(f"{OUT}/scene_{nm}.png")
    # contact sheet 3x2
    pad=22; bg=(58,63,46,255); cw,ch=imgs[0].size; cols=3
    rows=(len(imgs)+cols-1)//cols
    sheet=Image.new("RGBA",(cols*cw+pad*(cols+1),rows*ch+pad*(rows+1)),bg)
    for i,im in enumerate(imgs):
        r,c=divmod(i,cols); sheet.alpha_composite(im,(pad+c*(cw+pad),pad+r*(ch+pad)))
    sheet.save(f"{OUT}/scenes_all.png"); print("ok",len(imgs),"cenas")
