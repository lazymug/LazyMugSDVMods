#!/usr/bin/env python3
"""Rebuilds bagconfig.json from the stock ItemBags config.

Changes applied (see README.md in this folder):
  * Fish Bag removed        - the four regional fish bags + Crab Pot Bag cover it
  * Recycling Bag removed   - Crab Pot Bag covers the trash, Joja Cola moves to the Food Bag
  * Gem Bag removed         - the Mining Bag already holds every gem (Crystalarium moves there)
  * Smithing Bag merged into the Mining Bag
  * Food Bag gains Joja Cola
  * Construction Bag gains every item craftable from the vanilla crafting menu
  * Miscellaneous Fish Bag gains the fish orphaned by the Fish Bag removal
  * The fish bags are trimmed to fish plus their own jelly; the bait, worm bins,
    seaweed and algae they also held live in the Construction and Crab Pot bags
  * Treasure Chest moves to the Loot Bag, its only remaining home
  * Tea Leaves is added to the Crop Bag - stock Item Bags has it in no bag at all
  * Cooked dishes, the pantry, the uncooked drinks and snacks, and the tapper syrups
    are marked as having qualities - Item Bags reads quality off the item's category,
    which misses all of them

Usage:
    python3 tools/gen_bagconfig.py <stock-bagconfig.json> <out.json> \
        <CraftingRecipes.json> <CookingRecipes.json> <Objects.json>
"""
import json, sys, collections, uuid

REMOVE = ["Fish Bag", "Recycling Bag", "Gem Bag"]
MERGE_SOURCE = "Smithing Bag"
MERGE_TARGET = "Mining Bag"
SIZES = ["Small", "Medium", "Large", "Giant", "Massive"]

# fish that only lived in the catch-all Fish Bag: 10 legendaries + 3 Ginger Island fish
ORPHAN_FISH = {
    "Giant":   ["836", "837", "838"],
    "Massive": ["159", "160", "163", "682", "775", "898", "899", "900", "901", "902"],
}

# Namespace for deriving the new bags' ids, so a rebuild always produces the same ones.
FOOD_BAG_NAMESPACE = uuid.UUID("f2d4a639-53ab-4124-a80d-c59b1ce67a4b")

FISH_CATEGORY = -4
COOKING_CATEGORY = -7
# A fish bag keeps its fish and the one jelly caught in the same water. The Lake Fish
# Bag has no entry because vanilla has no lake jelly.
FISH_BAG_JELLY = {
    "Ocean Fish Bag": "SeaJelly",
    "River Fish Bag": "RiverJelly",
    "Lake Fish Bag": None,
    "Miscellaneous Fish Bag": "CaveJelly",
}
TREASURE_CHEST = "166"   # dropped by the fish bags, so the Loot Bag takes it
TEA_LEAVES = "815"

# With 84 of its items now carrying a quality star, the Food Bag's ungrouped layout would
# be 361 slots over 23 rows. Every other bag holding quality items groups them instead -
# one compact row of four quality slots per dish - so the Food Bag does too. Six groups
# per row matches the Foraging Bag, the other quality-heavy bag.
FOOD_BAG_GROUPS_PER_ROW = 6

# What the Food Bag keeps besides cooked dishes: the pantry, the drinks and snacks that
# are never cooked, and the seasonings. The rest of what stock Item Bags put in it has a
# better home - the buff items and the machines are all in the Construction Bag, and
# Maple Syrup is in the Tree Bag.
FOOD_BAG_REMOVE = [
    ("724", False),          # Maple Syrup -> Tree Bag
    ("773", False),          # Life Elixir -> Construction Bag
    ("772", False),          # Oil of Garlic -> Construction Bag
    ("879", False),          # Monster Musk -> Construction Bag
    ("926", False),          # Cookout Kit -> Construction Bag
    ("12", True),            # Keg -> Construction Bag
    ("Dehydrator", True),    # Dehydrator -> Construction Bag
    ("246", True),           # Coffee Maker -> see COFFEE_MAKER below
]
# The Coffee Maker is the one machine here that can't be crafted, so the Construction
# Bag - built from the crafting recipes - doesn't already hold it. It joins the Keg and
# the Dehydrator there rather than being left with nowhere to go.
COFFEE_MAKER = ("246", True)

# Item Bags decides quality from the item's category, which leaves these out - but in
# 1.6 nothing stops an object carrying a quality, and the pantry staples and the
# uncooked drinks and snacks do. Marked by id, in every bag that holds them, so a
# silver Oil doesn't become unstoreable. The two seasonings never carry one.
FOOD_QUALITY_ITEMS = [
    "245", "246", "247", "419", "423", "Raisins",   # pantry
    "395", "167", "873", "349", "351",              # drinks
    "403", "874", "78", "814",                      # snacks
]

# Item Bags has the syrup category commented out of its quality list, so the tappers'
# output comes out unstarred. These were already fixed by hand in the config this
# generator reads; they are listed here so the fix survives a rebuild from a pristine
# bagconfig.json. (SVE's two syrups get the same treatment, via CategoryQualities on
# the Stardew Valley Expanded Comprehensive Bag.)
SYRUP_QUALITY_ITEMS = ["724", "725", "726", "MysticSyrup"]

# The Food Bag is split five ways. Each entry is the bag's name, description, the
# springobjects sprite index to take its icon from, how many quality groups fit on a
# menu row, and the items it claims - Lunch takes whatever the other four don't, so no
# dish can fall through. Lunch also inherits the Food Bag's own id, so a bag a player
# already owns stays a valid item instead of pointing at a type that no longer exists.
FOOD_SPLIT = [
    ("Food Ingredients Bag",
     "A bag for the staples you cook with.", 246, 4, [
         "245", "246", "247", "419", "423", "Raisins",   # sugar, flour, oil, vinegar, rice
         "78", "814", "917",                            # cave carrot, squid ink, qi seasoning
     ]),
    ("Drinks Bag",
     "A bag for everything you drink.", 395, 4, [
         "395", "253", "StardropTea",                   # coffee, espresso, stardrop tea
         "167", "903", "873",                           # joja cola, ginger ale, pina colada
         "349", "351",                                  # energy tonic, muscle remedy
         "346", "459", "303",                           # beer, mead, pale ale
     ]),
    ("Sweet Bag",
     "A bag for cakes, pies, puddings and candy.", 220, 5, [
         "611", "234", "220", "223", "612", "233", "731", "243", "221", "604",
         "651", "608", "222", "232", "904", "905", "265", "610", "238",
     ]),
    ("Soup Bag",
     "A bag for soups, stews, broths and bisques.", 199, 4, [
         "456", "199", "219", "236", "218", "MossSoup",
         "728", "730", "727", "457", "207",
     ]),
    ("Lunch Bag",
     "A bag for cooked meals, sides and snacks.", 196, 6, None),
]
SPRITE_SHEET_COLUMNS = 24   # springobjects.png is 24 tiles wide

# Bags whose stock icon doesn't match what they hold. The Mining Bag absorbed the
# Smithing Bag, so it shows a bar of metal rather than a pickaxe; the Crop Bag shipped
# with a hammer off the tools sheet.
VANILLA_ICONS = {
    "Mining Bag": 337,   # Iridium Bar
    "Crop Bag": 254,     # Melon
}


def sizes_of(bag):
    return {s["Size"]: s for s in bag["SizeSettings"]}


def item(item_id, has_qualities=False, big=False):
    return {"Id": item_id, "HasQualities": has_qualities, "IsBigCraftable": big}


def add_items(bag, size, new_items):
    """Add items to `size` and every larger size, skipping duplicates."""
    by_size = sizes_of(bag)
    for s in SIZES[SIZES.index(size):]:
        cfg = by_size[s]
        seen = {(i["Id"], i["IsBigCraftable"]) for i in cfg["Items"]}
        for it in new_items:
            key = (it["Id"], it["IsBigCraftable"])
            if key not in seen:
                cfg["Items"].append(dict(it))
                seen.add(key)


def recipe_outputs(recipes):
    """Every distinct item produced by a crafting or cooking recipe, in recipe order."""
    out, seen = [], set()
    for value in recipes.values():
        parts = value.split("/")
        item_id = parts[2].split()[0]
        big = len(parts) > 3 and parts[3].strip().lower() == "true"
        if (item_id, big) not in seen:
            seen.add((item_id, big))
            out.append(item(item_id, False, big))
    return out


def craft_outputs(recipes):
    """Every distinct item produced by the vanilla crafting menu, in recipe order."""
    out, seen = [], set()
    for value in recipes.values():
        parts = value.split("/")
        item_id = parts[2].split()[0]
        big = parts[3].strip().lower() == "true"
        if (item_id, big) not in seen:
            seen.add((item_id, big))
            out.append(item(item_id, False, big))
    return out


def split_food_bag(food_bag):
    """Five bags out of one.

    An item's minimum size can't simply be carried over: the Food Bag's smallest tier
    held no dessert at all, which would have left the small Sweet Bag empty. Each new bag
    instead spreads its own items over the five tiers in the proportions the Food Bag
    used, keeping them in the order they unlocked in - so the cheap early dishes are
    still the ones a small bag holds.
    """
    by_size = {cfg["Size"]: cfg for cfg in food_bag["SizeSettings"]}
    total = len(by_size["Massive"]["Items"])
    share = [len(by_size[size]["Items"]) / total for size in SIZES]

    order = {}   # item id -> (tier it unlocked at, position within that tier)
    for tier, size in enumerate(SIZES):
        for position, it in enumerate(by_size[size]["Items"]):
            order.setdefault(it["Id"], (tier, position))

    claimed = {item_id for _, _, _, _, ids in FOOD_SPLIT if ids for item_id in ids}
    result = []
    for name, description, sprite, groups_per_row, ids in FOOD_SPLIT:
        keep = set(ids) if ids else None
        items = [it for it in by_size["Massive"]["Items"]
                 if (it["Id"] in keep if keep is not None else it["Id"] not in claimed)]
        items.sort(key=lambda it: order[it["Id"]])

        sizes = []
        for tier, size in enumerate(SIZES):
            source = by_size[size]
            count = max(1, round(len(items) * share[tier])) if items else 0
            sizes.append({
                "Size": size,
                "MenuOptions": {
                    "GroupByQuality": True,
                    "InventoryColumns": source["MenuOptions"]["InventoryColumns"],
                    "InventorySlotSize": source["MenuOptions"]["InventorySlotSize"],
                    "GroupedLayoutOptions": {"GroupsPerRow": groups_per_row,
                                             "ShowValueColumn": True, "SlotSize": 64},
                    "UngroupedLayoutOptions": {"Columns": 12, "LineBreakIndices": [],
                                               "LineBreakHeights": [], "SlotSize": 64},
                },
                "Price": source["Price"],
                "Sellers": list(source["Sellers"]),
                "CapacityMultiplier": source["CapacityMultiplier"],
                "Items": [dict(it) for it in items[:count]],
            })
        result.append({
            # Lunch, the bag with no claim of its own, keeps the Food Bag's identity
            "Id": food_bag["Id"] if ids is None else str(uuid.uuid5(FOOD_BAG_NAMESPACE, name)),
            "Name": name,
            "Description": description,
            "IconSourceTexture": food_bag["IconSourceTexture"],
            "IconSourceRect": {"X": sprite % SPRITE_SHEET_COLUMNS * 16,
                               "Y": sprite // SPRITE_SHEET_COLUMNS * 16,
                               "Width": 16, "Height": 16},
            "SizeSettings": sizes,
        })
    return result


def main(src, dst, crafting_path, cooking_path, objects_path):
    cfg = json.load(open(src, encoding="utf-8"))
    recipes = json.load(open(crafting_path, encoding="utf-8"))
    objects = json.load(open(objects_path, encoding="utf-8"))
    dishes = {it["Id"] for it in recipe_outputs(json.load(open(cooking_path,
                                                              encoding="utf-8")))}
    bags = {b["Name"]: b for b in cfg["BagTypes"]}

    def category(it):
        if it["IsBigCraftable"]:
            return None
        entry = objects.get(it["Id"])
        if entry is None:
            raise KeyError(f"no category for item {it['Id']} - stale {objects_path}?")
        return entry["category"]

    # --- merge Smithing Bag into Mining Bag (plus the Gem Bag's Crystalarium) ---
    target, source = bags[MERGE_TARGET], bags[MERGE_SOURCE]
    t_sizes, s_sizes = sizes_of(target), sizes_of(source)
    for size in SIZES:
        seen = {(i["Id"], i["IsBigCraftable"]) for i in t_sizes[size]["Items"]}
        for it in s_sizes[size]["Items"]:
            if (it["Id"], it["IsBigCraftable"]) not in seen:
                t_sizes[size]["Items"].append(dict(it))
                seen.add((it["Id"], it["IsBigCraftable"]))
    target["Description"] = ("A bag for storing ores, bars, gems, geodes, "
                             "and everything else that comes out of the mines or the forge.")
    add_items(target, "Giant", [item("21", False, True)])  # Crystalarium, from the Gem Bag

    # --- Food Bag gains Joja Cola (orphaned by the Recycling Bag removal) ---
    add_items(bags["Food Bag"], "Small", [item("167")])

    # --- Construction Bag gains everything craftable ---
    add_items(bags["Construction Bag"], "Small", craft_outputs(recipes))
    bags["Construction Bag"]["Description"] = (
        "A bag for storing every item you can make at the crafting menu, "
        "plus the wood and stone you make them from.")

    # --- Miscellaneous Fish Bag adopts the fish the Fish Bag used to hold alone ---
    for size, fish in ORPHAN_FISH.items():
        add_items(bags["Miscellaneous Fish Bag"], size, [item(f, True) for f in fish])

    # --- fish bags keep only their fish and their own jelly ---
    for bag_name, jelly in FISH_BAG_JELLY.items():
        for cfg_size in bags[bag_name]["SizeSettings"]:
            cfg_size["Items"] = [it for it in cfg_size["Items"]
                                 if category(it) == FISH_CATEGORY or it["Id"] == jelly]
    add_items(bags["Loot Bag"], "Small", [item(TREASURE_CHEST)])

    # --- Tea Leaves: a vegetable, so it carries a quality star ---
    add_items(bags["Crop Bag"], "Small", [item(TEA_LEAVES, True)])

    # --- seasonings give cooked dishes a quality star, and the pantry has one of its own ---
    with_qualities = dishes | set(FOOD_QUALITY_ITEMS) | set(SYRUP_QUALITY_ITEMS)
    for bag in bags.values():
        for cfg_size in bag["SizeSettings"]:
            for it in cfg_size["Items"]:
                if it["Id"] in with_qualities and not it["IsBigCraftable"]:
                    it["HasQualities"] = True

    # --- the Food Bag keeps its dishes, pantry, drinks and seasonings ---
    add_items(bags["Construction Bag"], "Small", [item(COFFEE_MAKER[0], False, COFFEE_MAKER[1])])
    drop = set(FOOD_BAG_REMOVE)
    for cfg_size in bags["Food Bag"]["SizeSettings"]:
        cfg_size["Items"] = [it for it in cfg_size["Items"]
                             if (it["Id"], it["IsBigCraftable"]) not in drop]

    # --- icons that no longer match their bag ---
    for bag_name, sprite in VANILLA_ICONS.items():
        bags[bag_name]["IconSourceTexture"] = "SpringObjects"
        bags[bag_name]["IconSourceRect"] = {
            "X": sprite % SPRITE_SHEET_COLUMNS * 16, "Y": sprite // SPRITE_SHEET_COLUMNS * 16,
            "Width": 16, "Height": 16}

    # --- split the Food Bag five ways ---
    food_bags = split_food_bag(bags["Food Bag"])

    cfg["BagTypes"] = [b for b in cfg["BagTypes"]
                       if b["Name"] not in REMOVE and b["Name"] != MERGE_SOURCE]
    at = cfg["BagTypes"].index(bags["Food Bag"])
    cfg["BagTypes"][at:at + 1] = food_bags

    json.dump(cfg, open(dst, "w", encoding="utf-8"), indent=2, ensure_ascii=False)

    counts = collections.OrderedDict(
        (b["Name"], len(sizes_of(b)["Massive"]["Items"])) for b in cfg["BagTypes"])
    print(f"{len(cfg['BagTypes'])} bag types")
    for name, n in counts.items():
        print(f"  {n:4d}  {name}")


if __name__ == "__main__":
    main(*sys.argv[1:6])
