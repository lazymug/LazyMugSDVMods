# Friendly Trades

> Sell items directly to NPCs based on their specialty for bonus prices. The bonus scales with friendship level — the closer you are, the better the deal! Works seamlessly inside regular NPC shops.

---

## How It Works

Each specialist NPC buys items related to their profession at premium prices. The bonus increases with your friendship level:

| Hearts | Bonus per Heart | Total Bonus |
|--------|----------------|-------------|
| 1-4 | +2.5% | 2.5% - 10% |
| 5-6 | +5.0% | 15% - 20% |
| 7-8 | +7.5% | 27.5% - 35% |

**Maximum bonus: 35% at 8 hearts** (scalable via config multiplier).

---

## NPC Specialists

| NPC | Specialty | Items They Buy |
|-----|-----------|---------------|
| Clint | Blacksmith | Ores, Bars, Gems, Minerals, Geodes, Coal |
| Marnie | Rancher | Eggs, Milk, Wool, Cheese, Mayonnaise, Truffle Oil, Cloth, Butter |
| Gus | Chef | All cooked food, Coffee, Tea, Gold+ quality Vegetables & Fruits |
| Willy | Fisherman | All fish, Crab pot items, Roe, Aged Roe, Caviar |
| Robin | Carpenter | Wood, Hardwood, Stone, Clay, Fiber, Sap, Moss, Fences, Gates, Torches |
| Pierre | Farmer | All Vegetables, Fruits, and Flowers |
| Krobus | Shadow Trader | Monster loot, Void/Solar Essence, Void Egg, Void Mayo, Void Salmon |

---

## Two Ways to Sell

### 1. Inside NPC Shops (Automatic)
When you open any specialist NPC's regular shop (Pierre's store, Willy's fish shop, etc.), the bonus is automatically applied. Just sell items from your inventory as usual — specialty items get the friendship bonus, other items sell at normal price.

### 2. Direct Trade (Outside Shops)
When you meet a specialist NPC outside their shop (walking in town, at festivals, etc.), click them with empty hands to get a trade dialogue. Choose "I'd like to sell you some items" to open a dedicated trade menu where only specialty items can be sold.

---

## Configuration

All options can be changed via **Generic Mod Config Menu** or by editing `config.json`:

| Option | Default | Description |
|--------|---------|-------------|
| `EnableMod` | `true` | Enable or disable the mod |
| `BonusMultiplier` | `1.0` | Scales the friendship bonus (0.5 = half, 2.0 = double) |
| `ShowBonusMessages` | `true` | Show HUD message with current bonus when opening a shop |

---

## Installation

1. Install [SMAPI](https://smapi.io/) (4.0+)
2. *(Optional)* Install [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) for in-game configuration
3. Download this mod and unzip it into your `Stardew Valley/Mods` folder
4. Launch the game through SMAPI

---

## Compatibility

- Stardew Valley **1.6+**
- Windows, macOS, and Linux
- Single-player and multiplayer
- Compatible with most other mods

---

## Supported Languages

| Language | Status |
|----------|--------|
| English | Full support (default) |
| Portugues (Brazilian) | Full support |

---

## Credits

Developed by **Lazy Mug**
