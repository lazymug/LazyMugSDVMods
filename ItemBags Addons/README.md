# Item Bags Addons

Configuration for [Item Bags](https://www.nexusmods.com/stardewvalley/mods/5382) 3.1.0.

| File | Goes to |
| --- | --- |
| `bagconfig.json` | `Mods/ItemBags/bagconfig.json` |
| `*.json` (bags) | `Mods/ItemBags/assets/Modded Bags/` |

## Vanilla bags (`bagconfig.json`)

21 bag types, reorganised from the stock 21:

* **Fish Bag** removed — the four regional fish bags plus the Crab Pot Bag already
  cover it. The 10 legendaries and the three Ginger Island fish that only lived in
  the catch-all bag moved to the **Miscellaneous Fish Bag**.
* **Recycling Bag** removed — the Crab Pot Bag holds the trash it pulled up, Refined
  Quartz is in the Mining Bag and the Leek is in the Foraging Bag. Only **Joja Cola**
  had nowhere to go, so it moved to the **Food Bag**.
* **Gem Bag** removed — the Mining Bag already held every gem. Its **Crystalarium**
  moved there too.
* **Smithing Bag** merged into the **Mining Bag** (41 items: ores, bars, gems, geodes,
  furnaces, anvil, bombs).
* **Construction Bag** now holds every item the vanilla crafting menu can make —
  154 items, baits, tackles, bombs, sprinklers, machines and all.
* **Fish bags** hold only fish plus the jelly caught in the same water. The bait, worm
  bins, seaweed and algae they used to carry live in the Construction and Crab Pot bags;
  Treasure Chest moved to the Loot Bag, its only other home.
* **Tea Leaves** is in the Crop Bag. Stock Item Bags puts it in no bag at all.
* **Cooked dishes carry a quality star.** A seasoning — vanilla's Qi Seasoning, or the
  ones Love of Cooking adds — puts one on any cooked dish, and Item Bags leaves the
  cooking category out of its quality list. The 81 vanilla cooking recipes are marked
  instead of the whole category, so Piña Colada, Life Elixir and Oil of Garlic (bought
  or crafted, never cooked) stay quality-less.

  The pantry staples, the uncooked drinks and snacks, and the four tapper syrups (Maple
  Syrup, Oak Resin, Pine Tar, Mystic Syrup) carry a quality star too, which Item Bags
  also misses: it reads quality off the item's category, and in 1.6 an object's category
  does not decide whether it can hold a quality. All of them are marked by id in every
  bag that holds them, so a silver Oil is never left unstoreable. Only the two
  seasonings — Qi Seasoning and Stardrop Tea — stay without.

  All five food bags group by quality, the way every other bag holding quality items
  already does — a dish becomes one compact row of four quality slots instead of four
  loose slots in a grid.
* **Food Bag split five ways.** Stock Item Bags puts 108 items in one bag; with every
  dish now carrying a quality star that menu would be 398 slots. It is five bags instead:

  | Bag | Items | Contents |
  | --- | --- | --- |
  | Lunch Bag | 51 | cooked meals, sides and snacks |
  | Sweet Bag | 19 | cakes, pies, puddings, candy |
  | Drinks Bag | 11 | coffee, sodas, tonics, beer, mead, pale ale |
  | Soup Bag | 11 | soups, stews, broths, bisques |
  | Food Ingredients Bag | 9 | sugar, flour, oil, vinegar, rice, raisins, seasoning |

  Lunch takes whatever the other four don't claim, so no dish can fall through, and it
  inherits the Food Bag's own id — a bag a player already owns stays a valid item rather
  than pointing at a type that no longer exists. All five keep Gus, the Food Bag's price
  ladder and its capacity multipliers.

  What stock Item Bags also put in the Food Bag has a better home: the buff items and the
  machines are in the Construction Bag, Maple Syrup in the Tree Bag. The Coffee Maker is
  the one machine that can't be crafted, so the Construction Bag — built from the
  crafting recipes — gets it added explicitly rather than losing it.

## Modded bags

* **Crab Pot Bag** — shellfish, algae and crab-pot junk.
* **Cornucopia** — `All Crops in One` and `All Seeds in One` are replaced by five
  themed bags. Each one holds the produce *and* the seed or sapling it grows from,
  so every Cornucopia item lives in exactly one bag:

  | Bag | Items | Contents |
  | --- | --- | --- |
  | Tree Bag | 62 | fruit-tree and wild-tree produce + saplings |
  | Vegetable Bag | 74 | category -75 produce + seeds |
  | Herb Bag | 40 | `herb_item` produce + seeds |
  | Forage Bag | 33 | nuts, spices, forageables + seeds |
  | Fruit Bag | 31 | category -79 produce + seeds |
  | All Flowers in One | 114 | category -80 produce + seeds/saplings |

  `All Flowers in One` keeps its original BagId and file name — it only gained the
  seeds and saplings — so bags players already own keep working.

* **Wildflour's Atelier Goods** — one bag per profession, on top of the pack's own
  Nature and Artisan Machines bags:

  | Bag | Simple | Advanced |
  | --- | --- | --- |
  | Baker Bag | 12 | 129 |
  | Barista Bag | 9 | 107 |
  | Boutique Bag | 6 | 165 |
  | Brewer Bag | 2 | 26 |
  | Confectioner Bag | 13 | 156 |
  | Jam Bag | 29 | 29 |
  | Crate Bag | 30 | 30 |
  | Pickle Bag | 23 | 23 |
  | Syrup Bag | 20 | 20 |
  | Pantry Bag | 31 | 31 |

  The Gourmand profession is split into Jam, Pickle, Syrup, Crate and Pantry. The
  crates and baskets carry no context tag of their own, so the Crate Bag matches them
  by id suffix (`_Basket`, `_Box`, `_Crate`, `_Jars`) instead. The Pantry Bag is the
  catch-all, so anything a future Wildflour update adds lands there rather than nowhere.

  These use `ItemFilters` on Wildflour's own context tags rather than a fixed item
  list, because the mod's Simple/Advanced difficulty setting decides which items
  actually load. They exclude categories -9 and -74..-81 so they never overlap the
  Nature Bag or the Artisan Machines Bag.

## Icons

Item Bags can only draw a bag's icon from the vanilla sheets (`springobjects`,
`Craftables`, `debris`, `tools`, `Cursors`), so a modded bag's icon is a vanilla stand-in
for what it holds. Every bag has one — the stock modded-bag files ship with
`"IconPosition": {"X": 0, "Y": 0, "Width": 0, "Height": 0}`, a zero-size source rect that
draws nothing at all.

| Bag | Icon |
| --- | --- |
| Crab Pot | Crab |
| Cornucopia Fruit / Vegetable / Herb / Tree / Forage / Flowers / Artisan | Apple / Cauliflower / Tea Leaves / Peach / Hazelnut / Poppy / Wine |
| Wildflour Baker / Barista / Boutique / Brewer / Confectioner | Bread / Triple Shot Espresso / Bouquet / Beer / Cranberry Candy |
| Wildflour Jam / Pickle / Syrup / Crate / Pantry | Jelly / Pickles / Maple Syrup / Supply Crate / Sugar |
| Wildflour Nature / Artisan Machines | Common Mushroom / Battery Pack |
| SVE Fish / Cooking / Agricultures / Comprehensive | Tuna / Pizza / Starfruit / Treasure Chest |
| Food Ingredients / Drinks / Sweet / Soup / Lunch | Wheat Flour / Coffee / Chocolate Cake / Parsnip Soup / Salad |

Two vanilla icons were replaced: the Mining Bag showed a pickaxe before it absorbed the
Smithing Bag and now shows an Iridium Bar, and the Crop Bag shipped with a hammer off the
tools sheet and now shows a Melon.

## Regenerating

`tools/` holds the generators.

```bash
python3 tools/cornucopia_wildflour.py "$GAME/Mods" .
python3 tools/gen_bagconfig.py stock-bagconfig.json bagconfig.json CraftingRecipes.json
```

`gen_bagconfig.py` needs `Data/CraftingRecipes`, `Data/CookingRecipes` and
`Data/Objects` as JSON. The game ships them as LZX-compressed `.xnb`; decompress them
with [xnbcli](https://github.com/LeonBlade/xnbcli), then run the recipe bodies through
`tools/xnb_dict.py` and the objects body through `tools/xnb_objects.py`.

It rebuilds from a pristine `bagconfig.json`, so it may be run against a fresh Item Bags
install — the hand-made quality fixes are in the script, not only in the output.

Re-run `cornucopia_wildflour.py` after a Cornucopia update — its bags are explicit
item lists, so new crops would otherwise have no home.
