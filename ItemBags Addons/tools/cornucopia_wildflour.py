#!/usr/bin/env python3
"""Generates the Cornucopia and Wildflour modded-bag files for Item Bags.

Cornucopia's "All Crops in One" bag is replaced by five themed bags, each one
holding the produce *and* the seed/sapling that grows it:

    Tree Bag        fruit-tree and wild-tree produce + saplings (minus flowers)
    Herb Bag        herb_item produce + seeds
    Forage Bag      nuts, spices, forageables, and whatever is left over
    Fruit Bag       category -79 produce + seeds
    Vegetable Bag   category -75 produce + seeds
    Flowers Bag     category -80 produce + seeds/saplings (replaces the filter-based bag)

Wildflour's Atelier Goods gets one bag per profession, mirroring the mod's own
data files: Baker, Barista, Boutique, Brewer, Confectioner, Gourmand.

Usage: python3 tools/cornucopia_wildflour.py <Mods-folder> <output-folder>
"""
import json, os, re, sys

# ItemBags stores these categories with a quality star (ModdedBag.CategoriesWithQualities)
QUALITY_CATEGORIES = {-4, -5, -6, -14, -17, -26, -75, -79, -80, -81}

CORNUCOPIA_PACKS = ["[CP] Cornucopia More Crops",
                    "[CP] Cornucopia More Flowers",
                    "[CP] Cornucopia Artisan Machines"]
# Item Bags hides a bag whose ModUniqueId isn't installed. Every new Cornucopia bag
# holds More Crops items, so that's the pack to gate them on.
CORNUCOPIA_MOD_ID = "Cornucopia.MoreCrops"

# BagId, bag name, description. Ids continue the 1x000000-9220-… series the
# existing Cornucopia bags already use.
# The flowers bag keeps the BagId, BagName and file name Cornucopia already shipped, so
# bags players already own keep working - it only gains the seeds and saplings.
FLOWERS_FILENAME = "Cornucopia All Flowers in One.json"

CORNUCOPIA_BAGS = {
    "flowers":   ("15000000-9220-e0a8-420e-0e4a1aeb3d64", "Cornucopia All Flowers in One Bag",
                  "A bag for Cornucopia flowers, and the seeds and saplings they grow from."),
    "tree":      ("1d000000-9220-e0a8-420e-0e4a1aeb3d64", "Cornucopia Tree Bag",
                  "A bag for everything Cornucopia's trees drop, and the saplings that grow them."),
    "herb":      ("1c000000-9220-e0a8-420e-0e4a1aeb3d64", "Cornucopia Herb Bag",
                  "A bag for Cornucopia herbs and their seeds."),
    "forage":    ("1e000000-9220-e0a8-420e-0e4a1aeb3d64", "Cornucopia Forage Bag",
                  "A bag for Cornucopia nuts, spices and forage, and the seeds they grow from."),
    "fruit":     ("1a000000-9220-e0a8-420e-0e4a1aeb3d64", "Cornucopia Fruit Bag",
                  "A bag for Cornucopia fruit and the seeds it grows from."),
    "vegetable": ("1b000000-9220-e0a8-420e-0e4a1aeb3d64", "Cornucopia Vegetable Bag",
                  "A bag for Cornucopia vegetables and the seeds they grow from."),
}

# BagId, bag name, description, and the filter that claims the bag's items. Wildflour's
# items are gated behind the mod's Simple/Advanced difficulty setting, so these bags
# match on filters rather than a fixed id list - that way they follow whichever set of
# items is actually loaded. A claim of None marks the catch-all bag.
WILDFLOUR_BAGS = {
    "baker":        ("20000000-9220-e0a8-420e-0e4a1aeb3d64", "Wildflour Baker Bag",
                     "A bag for the breads, cakes and pastries of Wildflour's bakery.",
                     "HasContextTag:wildflour_bakery_item"),
    "barista":      ("21000000-9220-e0a8-420e-0e4a1aeb3d64", "Wildflour Barista Bag",
                     "A bag for Wildflour's coffees, teas and other cafe drinks.",
                     "HasContextTag:wildflour_barista_item"),
    "boutique":     ("22000000-9220-e0a8-420e-0e4a1aeb3d64", "Wildflour Boutique Bag",
                     "A bag for Wildflour's soaps, perfumes, cosmetics and floral arrangements.",
                     "HasContextTag:wildflour_boutique_item"),
    "brewer":       ("23000000-9220-e0a8-420e-0e4a1aeb3d64", "Wildflour Brewer Bag",
                     "A bag for Wildflour's ales, meads and kombuchas.",
                     "HasContextTag:wildflour_brewer_item"),
    "confectioner": ("24000000-9220-e0a8-420e-0e4a1aeb3d64", "Wildflour Confectioner Bag",
                     "A bag for Wildflour's candies, chocolates and frozen treats.",
                     "HasContextTag:wildflour_confectioner_item"),
    "jam":          ("26000000-9220-e0a8-420e-0e4a1aeb3d64", "Wildflour Jam Bag",
                     "A bag for Wildflour's jams, jellies, marmalades and fruit butters.",
                     "HasContextTag:wildflour_jam_item"),
    "pickle":       ("27000000-9220-e0a8-420e-0e4a1aeb3d64", "Wildflour Pickle Bag",
                     "A bag for everything Wildflour puts up in a pickling jar.",
                     "HasContextTag:wildflour_pickle_item"),
    "syrup":        ("28000000-9220-e0a8-420e-0e4a1aeb3d64", "Wildflour Syrup Bag",
                     "A bag for Wildflour's syrups.",
                     "HasContextTag:wildflour_syrup_item"),
    # The shipping crates and baskets carry no context tag of their own, so they are
    # matched by id suffix instead.
    "crate":        ("29000000-9220-e0a8-420e-0e4a1aeb3d64", "Wildflour Crate Bag",
                     "A bag for the crates, boxes and baskets you pack goods into.",
                     "|".join(f"LocalIdSuffix:{suffix}"
                              for suffix in ("_Basket", "_Box", "_Crate", "_Jars"))),
    # Catch-all, so anything a future Wildflour update adds lands here rather than nowhere.
    "pantry":       ("25000000-9220-e0a8-420e-0e4a1aeb3d64", "Wildflour Pantry Bag",
                     "A bag for Wildflour's pantry staples and cooking ingredients.",
                     None),
}

WILDFLOUR_MOD_ID = "Wildflour.AtelierGoods"
# Categories claimed by the pre-existing Wildflour Nature Bag (-74..-81) and
# Artisan Machines Bag (-9), so the new bags stay disjoint from them.
WILDFLOUR_EXCLUDED_CATEGORIES = "-9,-74,-75,-79,-80,-81"
# Item Bags leaves category -7 (Cooking) out of its quality list, but 34 of Wildflour's
# -7 items are cooking recipes, and a seasoning puts a quality star on a cooked dish.
WILDFLOUR_CATEGORY_QUALITIES = "-7:true"

# Bag icons: the springobjects sprite index of an item that says what the bag is for.
# Item Bags can only draw its icons from the vanilla sheets, so these are vanilla
# stand-ins for the modded contents.
SPRITE_SHEET_COLUMNS = 24
CORNUCOPIA_ICONS = {
    "flowers": 376,       # Poppy
    "tree": 636,          # Peach
    "herb": 815,          # Tea Leaves
    "forage": 408,        # Hazelnut
    "fruit": 613,         # Apple
    "vegetable": 190,     # Cauliflower
}
WILDFLOUR_ICONS = {
    "baker": 216,         # Bread
    "barista": 253,       # Triple Shot Espresso
    "boutique": 458,      # Bouquet
    "brewer": 346,        # Beer
    "confectioner": 612,  # Cranberry Candy
    "jam": 344,           # Jelly
    "pickle": 342,        # Pickles
    "syrup": 724,         # Maple Syrup
    "crate": 922,         # Supply Crate
    "pantry": 245,        # Sugar
}

PRICES = {"Small": 2000, "Medium": 5000, "Large": 20000, "Giant": 50000, "Massive": 100000}
CAPACITIES = {"Small": 30, "Medium": 99, "Large": 300, "Giant": 999, "Massive": 9999}
SIZES = list(PRICES)

MENU_OPTIONS = {
    "GroupByQuality": True,
    "InventoryColumns": 12,
    "InventorySlotSize": 64,
    "GroupedLayoutOptions": {"GroupsPerRow": 5, "ShowValueColumn": True, "SlotSize": 64},
    "UngroupedLayoutOptions": {"Columns": 12, "LineBreakIndices": [], "LineBreakHeights": [], "SlotSize": 64},
}


# --------------------------------------------------------------------------- io

def load_json5(path):
    """Content Patcher files allow comments and trailing commas; json does not."""
    text = open(path, encoding="utf-8-sig").read()
    out, i, n, in_string, escaped = [], 0, len(text), False, False
    while i < n:
        c = text[i]
        if in_string:
            out.append(c)
            if escaped:
                escaped = False
            elif c == "\\":
                escaped = True
            elif c == '"':
                in_string = False
            i += 1
            continue
        if c == '"':
            in_string = True
            out.append(c)
            i += 1
        elif c == "/" and i + 1 < n and text[i + 1] == "/":
            while i < n and text[i] not in "\r\n":
                i += 1
        elif c == "/" and i + 1 < n and text[i + 1] == "*":
            end = text.find("*/", i + 2)
            i = n if end < 0 else end + 2
        else:
            out.append(c)
            i += 1
    return json.loads(re.sub(r",(\s*[}\]])", r"\1", "".join(out)))


def pack_config(pack_folder):
    """A Content Patcher pack's config.json, or {} if it has none yet."""
    path = os.path.join(pack_folder, "config.json")
    return json.load(open(path, encoding="utf-8-sig")) if os.path.exists(path) else {}


def is_active(change, config):
    """False when a change is gated behind a config option the player turned off.

    Cornucopia hides whole item sets this way - "Rose Color Explosion" is off by
    default, for instance, so its 16 extra roses do not exist. Conditions that are
    not config options (HasMod and friends) are left alone.
    """
    for key, expected in (change.get("When") or {}).items():
        if key in config and str(config[key]).lower() != str(expected).lower():
            return False
    return True


def entries(path, target, config=None):
    """All Entries of every active EditData change in `path` aimed at `target`."""
    result = {}
    if not os.path.exists(path):
        return result
    for change in load_json5(path).get("Changes", []):
        if change.get("Target") != target or not is_active(change, config or {}):
            continue
        for key, value in (change.get("Entries") or {}).items():
            if isinstance(value, dict):
                result[key] = value
    return result


def unqualify(item_id):
    return item_id[3:] if item_id and item_id.startswith("(O)") else item_id


# ------------------------------------------------------------------ bag writing

def icon_rect(sprite):
    """The 16x16 source rectangle of a springobjects sprite index."""
    return {"X": sprite % SPRITE_SHEET_COLUMNS * 16, "Y": sprite // SPRITE_SHEET_COLUMNS * 16,
            "Width": 16, "Height": 16}


def write_bag(folder, bag_id, name, description, sellers, sprite, item_ids=(), catalogue=None,
              filters=None, filename=None, mod_id="", category_qualities=None):
    items = []
    for item_id in item_ids:
        category = catalogue[item_id]["cat"]
        items.append({
            "Name": item_id,
            "ObjectId": item_id,
            "IsBigCraftable": False,
            "HasQualities": category in QUALITY_CATEGORIES,
            "RequiredSize": "Small",
        })
    bag = {
        "IsEnabled": True,
        "ModUniqueId": mod_id,
        "BagId": bag_id,
        "BagName": name,
        "BagDescription": description,
        "IconTexture": "SpringObjects",
        "IconPosition": icon_rect(sprite),
        "Prices": dict(PRICES),
        "Capacities": dict(CAPACITIES),
        "SizeSellers": {s: list(sellers) for s in SIZES},
        "SizeMenuOptions": {s: json.loads(json.dumps(MENU_OPTIONS)) for s in SIZES},
        "Items": items,
    }
    if filters:
        bag["ItemFilters"] = list(filters)
        bag["ItemFiltersSorting"] = "CategoryId,DisplayName"
    if category_qualities:
        bag["CategoryQualities"] = category_qualities
    filename = filename or (name + ".json")
    json.dump(bag, open(os.path.join(folder, filename), "w", encoding="utf-8"),
              indent=2, ensure_ascii=False)
    return filename, len(items)


# ------------------------------------------------------------------- cornucopia

def build_cornucopia(mods, out):
    configs = {pack: pack_config(os.path.join(mods, pack)) for pack in CORNUCOPIA_PACKS}

    catalogue = {}
    for pack in CORNUCOPIA_PACKS:
        for item_id, data in entries(os.path.join(mods, pack, "data", "objects.json"),
                                     "Data/Objects", configs[pack]).items():
            catalogue[item_id] = {"cat": data.get("Category"),
                                  "tags": set(data.get("ContextTags") or [])}

    # seed/sapling -> produce, from Data/Crops, Data/FruitTrees and CustomBush
    grows_into = {}
    for pack in CORNUCOPIA_PACKS:
        data_dir, config = os.path.join(mods, pack, "data"), configs[pack]
        for filename in ("crops.json", "crops_uncolored.json"):
            for seed, crop in entries(os.path.join(data_dir, filename), "Data/Crops", config).items():
                grows_into.setdefault(unqualify(seed), unqualify(crop.get("HarvestItemId")))
        for sapling, tree in entries(os.path.join(data_dir, "fruittrees.json"),
                                     "Data/FruitTrees", config).items():
            fruit = [f.get("ItemId") for f in (tree.get("Fruit") or []) if isinstance(f, dict)]
            if fruit:
                grows_into.setdefault(unqualify(sapling), unqualify(fruit[0]))
        for seed, bush in entries(os.path.join(data_dir, "teabushes.json"),
                                  "furyx639.CustomBush/Data", config).items():
            produced = [p.get("ItemId") for p in (bush.get("ItemsProduced") or [])
                        if isinstance(p, dict)]
            if produced:
                grows_into.setdefault(unqualify(seed), unqualify(produced[0]))
        for _, tree in entries(os.path.join(data_dir, "wildtrees.json"),
                               "Data/WildTrees", config).items():
            # a wild tree's yield can come from shaking, chopping or tapping it
            produced = [d.get("ItemId")
                        for key in ("ShakeItems", "SeedDropItems", "ChopItems", "TapItems")
                        for d in (tree.get(key) or []) if isinstance(d, dict)]
            seed = unqualify(tree.get("SeedItemId"))
            if seed and produced:
                grows_into.setdefault(seed, unqualify(produced[0]))

    TREE_ROLES = {"cornucopia_fruittree_produce", "cornucopia_fruittree_sapling",
                  "cornucopia_wildtree_produce"}
    FORAGE_TAGS = {"nut_item", "spice_item", "cornucopia_forage"}

    def bag_of(item_id):
        """Which bag a piece of produce belongs to. Seeds inherit their produce's bag."""
        info = catalogue.get(item_id)
        if info is None:
            return None
        category, tags = info["cat"], info["tags"]
        if category == -80:
            return "flowers"
        if tags & TREE_ROLES:
            return "tree"
        if "herb_item" in tags:
            return "herb"
        if tags & FORAGE_TAGS:
            return "forage"
        if category == -79:
            return "fruit"
        if category == -75:
            return "vegetable"
        return "forage"

    groups = {key: [] for key in CORNUCOPIA_BAGS}
    unplaced = []
    for item_id, info in catalogue.items():
        if "cornucopia_artisangood" in info["tags"] or info["cat"] == -26:
            continue  # stays in the existing "All Artisan in One" bag
        if info["cat"] == -74:  # a seed or sapling: follow whatever it grows into
            produce = grows_into.get(item_id)
            key = bag_of(produce) if produce else None
            if key is None:
                unplaced.append(item_id)
                continue
        else:
            key = bag_of(item_id)
        groups[key].append(item_id)

    for key, (bag_id, name, description) in CORNUCOPIA_BAGS.items():
        path, count = write_bag(out, bag_id, name, description, ["Pierre"],
                                CORNUCOPIA_ICONS[key],
                                item_ids=sorted(groups[key]), catalogue=catalogue,
                                filename=FLOWERS_FILENAME if key == "flowers" else None,
                                mod_id="" if key == "flowers" else CORNUCOPIA_MOD_ID)
        print(f"  {count:4d} items  {path}")
    if unplaced:
        print(f"  !! {len(unplaced)} unplaced: {', '.join(sorted(unplaced))}")
    return groups, catalogue


# -------------------------------------------------------------------- wildflour

def build_wildflour(mods, out):
    """One bag per Wildflour profession, matched on the mod's own item metadata."""
    del mods  # the bags are filter-based, so no item data is needed

    def negate(claim):
        """An entry's alternatives are ORed, so excluding it means one !entry each."""
        return ["!" + alternative for alternative in claim.split("|")]

    claimed = [claim for _, _, _, claim in WILDFLOUR_BAGS.values() if claim]
    for key, (bag_id, name, description, claim) in WILDFLOUR_BAGS.items():
        filters = [f"FromMod:{WILDFLOUR_MOD_ID}",
                   f"!CategoryId:{WILDFLOUR_EXCLUDED_CATEGORIES}"]
        if claim:
            filters.append(claim)
        else:
            for other in claimed:
                filters += negate(other)
            filters.append("!HasContextTag:wildflour_forage")
        path, _ = write_bag(out, bag_id, name, description, ["Pierre"], WILDFLOUR_ICONS[key],
                            filters=filters, mod_id=WILDFLOUR_MOD_ID,
                            category_qualities=WILDFLOUR_CATEGORY_QUALITIES)
        print(f"  {len(filters):4d} filters  {path}")


if __name__ == "__main__":
    mods_folder, out_folder = sys.argv[1], sys.argv[2]
    os.makedirs(out_folder, exist_ok=True)
    print("Cornucopia:")
    build_cornucopia(mods_folder, out_folder)
    print("Wildflour:")
    build_wildflour(mods_folder, out_folder)
