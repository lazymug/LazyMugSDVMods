#!/usr/bin/env python3
"""Carta pixel-art: chassi fofo SDV + campo com perspectiva + fonte pixel desenhada."""
from PIL import Image, ImageDraw, ImageEnhance

XNB = "/private/tmp/claude-501/-Volumes-SSD-Work-Workspace-LazyMugSDVMods/fced3107-da36-458c-a9ca-44020c5ce3cc/scratchpad/xnb"
OUT = "/Volumes/SSD_Work/Workspace/LazyMugSDVMods/ValleyTriad/card-previews"
S = 6

spring = Image.open(f"{XNB}/springobjects.png").convert("RGBA")
crops  = Image.open(f"{XNB}/crops.png").convert("RGBA")
def obj(i): return spring.crop((i%24*16, i//24*16, i%24*16+16, i//24*16+16))
plant_leafy = crops.crop((96,0,112,32))   # pastinaga madura (folhagem verde genérica)
plant_leafy = ImageEnhance.Brightness(plant_leafy).enhance(1.18)  # mais viçosa

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

def scene(w,h,hero):
    img=Image.new("RGBA",(w,h)); d=ImageDraw.Draw(img)
    hz=int(h*0.40)
    # céu
    sky=[(250,226,168),(214,224,224),(176,208,226),(150,196,224)]
    for y in range(hz):
        d.line([(0,y),(w,y)],fill=sky[min(len(sky)-1,y*len(sky)//hz)])
    # linha de árvores/colina ao fundo
    d.rectangle([0,hz-4,w,hz+1],fill=(78,120,58))
    for x in range(0,w,4): d.ellipse([x-2,hz-7,x+3,hz+1],fill=(90,132,64))
    # solo base
    for y in range(hz,h):
        t=(y-hz)/(h-hz); d.line([(0,y),(w,y)],fill=(int(120-40*t),int(88-30*t),int(56-20*t)))
    # sulcos convergindo p/ ponto de fuga (bem sutis, quase só no primeiro plano)
    vp=(w//2,hz)
    for bx in range(-w,2*w,16):
        d.line([(bx,h),vp],fill=(116,84,52),width=1)
    # canteiros PROCEDURAIS: touceiras verdes sobrepostas, com perspectiva + neblina
    haze=(150,190,178)
    def lerp(a,b,t): return tuple(int(a[i]+(b[i]-a[i])*t) for i in range(3))
    def clump(cx,cy,r,t):
        dk=lerp((44,88,38),haze,(1-t)*0.45); md=lerp((74,134,54),haze,(1-t)*0.45)
        lt=lerp((126,182,80),haze,(1-t)*0.4)
        d.ellipse([cx-r,cy-r*0.5,cx+r,cy+r*0.55],fill=dk)
        d.ellipse([cx-r*0.92,cy-r*0.85,cx+r*0.92,cy+r*0.4],fill=md)
        d.ellipse([cx-r*0.55,cy-r*0.95,cx+r*0.45,cy-r*0.1],fill=lt)
    # fileiras de trás (pequenas) para frente (grandes)
    rows=[(hz+2,1.4),(hz+6,1.9),(hz+11,2.5),(hz+18,3.2),(hz+27,4.1),(hz+39,5.1),(hz+55,6.4)]
    n=len(rows)
    for i,(ry,r) in enumerate(rows):
        t=(i+1)/n
        step=max(3,int(r*1.25)); off=(i%2)*step//2
        x=-r+off
        while x<w+r:
            clump(x,ry,r,t); x+=step
    # vinheta pixel p/ foco
    vg=Image.new("RGBA",(w,h),(0,0,0,0)); dv=ImageDraw.Draw(vg)
    for i in range(4): dv.rectangle([i,i,w-1-i,h-1-i],outline=(20,24,14,max(0,40-i*10)))
    img.alpha_composite(vg)
    # spotlight + herói destacado
    cx,cy=w//2,int(h*0.70)
    glow=Image.new("RGBA",(w,h),(0,0,0,0)); dg=ImageDraw.Draw(glow)
    for rr,a in [(16,60),(11,72),(7,84)]: dg.ellipse([cx-rr,cy-rr//2-2,cx+rr,cy+rr//2+2],fill=(255,246,206,a))
    img.alpha_composite(glow)
    d.ellipse([cx-12,cy+9,cx+12,cy+15],fill=(0,0,0,95))
    hs=outline_spr(hero.resize((34,34),Image.NEAREST))
    img.alpha_composite(hs,(cx-hs.width//2,cy-22))
    return img

# ---------------- carta ----------------
def coin1x(d,cx,cy,r,inlay):
    d.ellipse([cx-r,cy-r,cx+r,cy+r],fill=(198,150,78),outline=DARK)
    d.ellipse([cx-r+1,cy-r+1,cx+r-1,cy-1],fill=(240,211,150))
    d.ellipse([cx-r,cy-r,cx+r,cy+r],outline=inlay)

def build(name,tier,elem,stats,sprite):
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
    c.alpha_composite(scene(ax1-ax0,ay1-ay0,sprite),(ax0,ay0))
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

def finalize(name,tier,elem,stats,sprite):
    card,edges,badge,st,nm,el=build(name,tier,elem,stats,sprite)
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

if __name__=="__main__":
    import os; os.makedirs(OUT,exist_ok=True)
    demos=[("Couve-flor","Comum","Primavera",(4,5,3,2),obj(190)),
           ("Pastinaga","Comum","Primavera",(3,1,2,2),obj(24)),
           ("Abóbora","Comum","Outono",(6,3,4,1),obj(276))]
    imgs=[finalize(*a) for a in demos]
    imgs[0].save(f"{OUT}/pixel_couveflor.png")
    pad=24; bg=(58,63,46,255); cw,ch=imgs[0].size
    sheet=Image.new("RGBA",(len(imgs)*cw+pad*(len(imgs)+1),ch+pad*2),bg)
    for i,im in enumerate(imgs): sheet.alpha_composite(im,(pad+i*(cw+pad),pad))
    sheet.save(f"{OUT}/pixel_demo.png"); print("ok",imgs[0].size)
