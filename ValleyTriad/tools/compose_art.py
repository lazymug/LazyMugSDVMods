#!/usr/bin/env python3
"""Compose final per-card art for Valley Triad.

Pipeline per card:
  1. Load the subject PNG (subject on a pure-white background) from assets/frames/<cat>/<name>.png
  2. Key out the white BACKGROUND only, via a flood-fill from the image borders (interior
     whites — cauliflower, a white hen, a diamond, teeth — are preserved).
  3. Trim to the subject bounding box and scale to fit the card window.
  4. Composite over the category scene (assets/frames/<scene>.png) with a soft ground shadow.
  5. Save to assets/art/<cardId>.png at the exact window size the mod draws (304x292).

Usage:
  python3 compose_art.py                 # all 52 cards
  python3 compose_art.py --only abigail,cauliflower,diamond,crimsonfish
  python3 compose_art.py --out preview   # write to assets/<out>/ instead of assets/art/
"""
import os, sys, json, collections
import numpy as np
from PIL import Image, ImageFilter, ImageDraw

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
FRAMES = os.path.join(ROOT, "assets", "frames")
CARDS = os.path.join(ROOT, "assets", "cards.json")

ART_SIZE = (304, 292)          # window the mod draws (WW*S x WH*S)
KEY_MAX = 512                  # work resolution for keying
WHITE_LO = 206                 # background is bright (min channel >= this) ...
NEUTRAL = 16                   # ... and near-neutral (max-min <= this). Lets the flood cross
                               # faint gray frame lines while preserving tinted near-whites
                               # (cream cauliflower, off-white fur) that carry real color.
FEATHER_D = 4                  # de-halo reaches this many px in from the removed background
FEATHER_LO = 196               # brightness at/below which an edge pixel keeps full opacity
WHITE_T = 238                  # feather target: fully-white edge pixels fade to alpha 0
SUBJECT_W = 0.92               # subject max width as fraction of window
SUBJECT_H = 0.86               # subject max height as fraction of window
GROUND_Y = 0.95                # subject feet sit at this fraction of window height

RENAME = {"cranberry": "cranberries", "peperrex": "pepperrex", "elliot": "elliott"}
CATS = ["animals", "crops", "fish", "forage", "minerals", "monsters", "specials", "villagers"]
SCENE = {"Crop": "field", "Forage": "field", "Animal": "pasture", "Mineral": "mine",
         "Monster": "mine", "Fish": "sea", "Villager": "saloon", "Special": "night"}
SCENE_OVERRIDE = {"wizard": "night"}   # wizard is a Villager but uses the night scene


def load_cards():
    d = json.load(open(CARDS))
    cards = d if isinstance(d, list) else next((v for v in d.values() if isinstance(v, list)), [])
    return {(c.get("Id") or c.get("id")): (c.get("Category") or c.get("category")) for c in cards}


def find_subject_files():
    """stem -> (category_folder, full_path)"""
    out = {}
    for cat in CATS:
        d = os.path.join(FRAMES, cat)
        if not os.path.isdir(d):
            continue
        for f in sorted(os.listdir(d)):
            if f.lower().endswith(".png"):
                out[os.path.splitext(f)[0]] = (cat, os.path.join(d, f))
    return out


def key_white_background(im):
    """Return RGBA with the border-connected white background made transparent."""
    im = im.convert("RGBA")
    if max(im.size) > KEY_MAX:
        s = KEY_MAX / max(im.size)
        im = im.resize((round(im.size[0] * s), round(im.size[1] * s)), Image.LANCZOS)
    arr = np.asarray(im).copy()
    h, w = arr.shape[:2]
    rgb = arr[:, :, :3].astype(int)
    mn = rgb.min(axis=2); mx = rgb.max(axis=2)
    whiteish = (mn >= WHITE_LO) & ((mx - mn) <= NEUTRAL)

    visited = np.zeros((h, w), dtype=bool)
    dq = collections.deque()
    for x in range(w):
        for y in (0, h - 1):
            if whiteish[y, x] and not visited[y, x]:
                visited[y, x] = True; dq.append((y, x))
    for y in range(h):
        for x in (0, w - 1):
            if whiteish[y, x] and not visited[y, x]:
                visited[y, x] = True; dq.append((y, x))
    while dq:
        y, x = dq.popleft()
        for ny, nx in ((y - 1, x), (y + 1, x), (y, x - 1), (y, x + 1)):
            if 0 <= ny < h and 0 <= nx < w and not visited[ny, nx] and whiteish[ny, nx]:
                visited[ny, nx] = True; dq.append((ny, nx))

    # second pass: also drop enclosed pockets of *pure* white (e.g. background trapped between
    # a character's legs or under the feet, which the border flood can't reach). Restricted to
    # near-255 neutral pixels, so shaded/tinted subject whites (cream cauliflower, a hen, bluish
    # quartz) are never touched.
    pure = (mn >= 246) & ((mx - mn) <= 8)
    visited |= pure

    # de-halo by feathering: within FEATHER_D px of the removed background, fade a pixel's
    # alpha in proportion to how white it is. This dissolves the light AA ring without a hard
    # erosion, and — being distance-limited — never touches interior whites (cauliflower head,
    # a white hen, teeth) that sit well inside the subject.
    zone = visited.copy()
    for _ in range(FEATHER_D):
        n = zone.copy()
        n[1:, :] |= zone[:-1, :]; n[:-1, :] |= zone[1:, :]
        n[:, 1:] |= zone[:, :-1]; n[:, :-1] |= zone[:, 1:]
        zone = n
    zone &= ~visited
    bright = (arr[:, :, 0].astype(float) + arr[:, :, 1] + arr[:, :, 2]) / 3.0
    whiteness = np.clip((bright - FEATHER_LO) / (WHITE_T - FEATHER_LO), 0.0, 1.0)  # 0..1
    alpha = arr[:, :, 3].astype(float)
    alpha = np.where(zone, alpha * (1.0 - whiteness), alpha)
    alpha[visited] = 0   # removed background is fully transparent (must be applied last)
    arr[:, :, 3] = np.clip(alpha, 0, 255).astype(np.uint8)
    return Image.fromarray(arr, "RGBA")


def trim(im):
    a = np.asarray(im)[:, :, 3]
    ys, xs = np.where(a > 8)
    if len(xs) == 0:
        return im
    return im.crop((xs.min(), ys.min(), xs.max() + 1, ys.max() + 1))


def compose(card_id, category, subject_path, out_dir):
    scene_name = SCENE_OVERRIDE.get(card_id, SCENE[category])
    scene = Image.open(os.path.join(FRAMES, scene_name + ".png")).convert("RGBA").resize(ART_SIZE, Image.LANCZOS)

    subj = trim(key_white_background(Image.open(subject_path)))
    maxw, maxh = int(ART_SIZE[0] * SUBJECT_W), int(ART_SIZE[1] * SUBJECT_H)
    sc = min(maxw / subj.size[0], maxh / subj.size[1])
    subj = subj.resize((max(1, round(subj.size[0] * sc)), max(1, round(subj.size[1] * sc))), Image.LANCZOS)

    x = (ART_SIZE[0] - subj.size[0]) // 2
    y = int(ART_SIZE[1] * GROUND_Y) - subj.size[1]
    y = max(2, y)

    # soft ground shadow
    shadow = Image.new("RGBA", ART_SIZE, (0, 0, 0, 0))
    sd = ImageDraw.Draw(shadow)
    sw = int(subj.size[0] * 0.7); sh = max(6, int(subj.size[1] * 0.10))
    cx = ART_SIZE[0] // 2; cy = y + subj.size[1] - sh // 2
    sd.ellipse([cx - sw // 2, cy - sh // 2, cx + sw // 2, cy + sh // 2], fill=(0, 0, 0, 110))
    shadow = shadow.filter(ImageFilter.GaussianBlur(4))
    scene = Image.alpha_composite(scene, shadow)

    scene.paste(subj, (x, y), subj)

    os.makedirs(out_dir, exist_ok=True)
    dst = os.path.join(out_dir, card_id + ".png")
    scene.convert("RGBA").save(dst, optimize=True)
    return dst, os.path.getsize(dst)


def main():
    only = None
    out_dir = os.path.join(ROOT, "assets", "art")
    args = sys.argv[1:]
    i = 0
    while i < len(args):
        if args[i] == "--only":
            only = set(args[i + 1].split(",")); i += 2
        elif args[i] == "--out":
            out_dir = os.path.join(ROOT, "assets", args[i + 1]); i += 2
        else:
            i += 1

    ids = load_cards()
    files = find_subject_files()
    done = 0
    for stem, (cat, path) in files.items():
        card_id = RENAME.get(stem, stem)
        if card_id not in ids:
            print(f"  SKIP {stem}: no cardId"); continue
        if only and card_id not in only:
            continue
        dst, size = compose(card_id, ids[card_id], path, out_dir)
        print(f"  {card_id:16s} [{ids[card_id]:8s} -> {SCENE_OVERRIDE.get(card_id, SCENE[ids[card_id]])}]  {size//1024} KB")
        done += 1
    print(f"done: {done} cards -> {out_dir}")


if __name__ == "__main__":
    main()
