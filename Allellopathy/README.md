# 🌿 Allellopathy

> Adds realistic allelopathy to farming. Nearby crops help or hinder each other through biochemical interactions, affecting growth speed, quality, and yield. Plan your farm layout wisely — companion planting matters! Includes per-crop effects for spring, summer, fall, and special crops.

---

## 🧪 What is Allelopathy?

In real life, **allelopathy** is a biological phenomenon where plants release biochemical compounds that influence the growth of neighboring plants — for better or worse. Some plants are natural companions that boost each other, while others are rivals that compete and suppress growth.

This mod brings that concept into Stardew Valley! Every crop you plant now interacts with its neighbors. Choose your farm layout wisely to maximize profits, or suffer the consequences of poor planning. 🌾

---

## ✨ Features

- 🌱 **Companion Planting System** — Crops placed near each other can have positive or negative effects
- ⚡ **Growth Speed Effects** — Some combinations speed up or slow down growth phases
- ⭐ **Quality Effects** — Certain neighbors can apply, upgrade, or even remove fertilizer effects
- 📦 **Yield Effects** — Boost or reduce harvest cycles for regrowth crops
- 💧 **Soil Effects** — Some plants auto-water dry soil, while others dry it out
- 💬 **Hover Tooltips** — Hover over any crop to see if it's being affected by allelopathy
- ✨ **Particle Effects** — Occasional visual sparkles show positive and negative interactions
- 🔔 **In-game Messages** — Get notified when nearby crops interact
- ⚙️ **Fully Configurable** — Adjust effect strength, toggle messages, tooltips, and particles
- 📋 **Generic Mod Config Menu Support** — Configure everything from the in-game menu

---

## 🌸 Spring Interactions

| Source | Target | Effect | Type | Description |
|--------|--------|--------|------|-------------|
| Green Bean | All nearby crops | ✅ Positive | Quality | Fixes nitrogen in the soil, improving quality |
| Parsnip | Cauliflower | ❌ Negative | Quality | Inhibits cauliflower growth |
| Cauliflower | Potato | ❌ Negative | Quantity | Competes for nutrients, reducing harvest |
| Cauliflower | Parsnip | ❌ Negative | Quality | Inhibits parsnip growth |
| Cauliflower | Green Bean | ✅ Positive | Quality | Benefits from nitrogen fixation |
| Garlic | Strawberry | ✅ Positive | Quality | Repels pests that damage strawberries |
| Garlic | Green Bean | ❌ Negative | Quality | Inhibits green bean growth |
| Kale | Potato | ✅ Positive | Quantity | Deep roots bring up nutrients for potatoes |
| Kale | Green Bean | ✅ Positive | Quality | Benefits from nitrogen fixation |
| Potato | Rhubarb | ✅ Positive | Quality | Deters pests that attack rhubarb |
| Rhubarb | Cauliflower | ✅ Positive | Quality | Provides beneficial shade |
| Rhubarb | Parsnip | ✅ Positive | Quality | Enhances parsnip growth |
| Rhubarb | Green Bean | ✅ Positive | Quality | Benefits from nitrogen fixation |
| Rhubarb | Potato | ❌ Negative | Quantity | Competes for nutrients |
| Strawberry | Garlic | ✅ Positive | Growth Speed | Mutually beneficial, speeds garlic growth |
| Strawberry | Potato | ✅ Positive | Quality | Improves potato quality |
| Strawberry | Green Bean | ✅ Positive | Growth Speed | Nitrogen fixation reduces growth time |
| Strawberry | Strawberry | ❌ Negative | Quality | Compete when planted too closely |

---

## ☀️ Summer Interactions

| Source | Target | Effect | Type | Description |
|--------|--------|--------|------|-------------|
| Tomato | Potato | ❌ Negative | Default | Releases substances that inhibit potato growth |
| Sunflower | All nearby crops | ✅ Positive | Default | Attracts beneficial insects (radius: 2) |
| Green Bean | Corn | ✅ Positive | Default | Nitrogen fixation benefits corn growth |
| Blueberry | Strawberry | ❌ Negative | Default | Compete for similar soil resources |
| Strawberry | Blueberry | ❌ Negative | Default | Compete for similar soil resources |
| Hot Pepper | All nearby crops | ✅ Positive | Default | Deters pests from nearby plants |
| Melon | Radish | ❌ Negative | Default | Don't grow well together |

---

## 🍂 Fall Interactions

| Source | Target | Effect | Type | Description |
|--------|--------|--------|------|-------------|
| Pumpkin | All nearby crops | ❌ Negative | Default | Heavy feeder, depletes soil nutrients |
| Fairy Rose | All nearby crops | ✅ Positive | Default | Attracts beneficial insects (radius: 2) |
| Amaranth | All nearby crops | ✅ Positive | Default | Helps suppress weeds |
| Eggplant | Potato | ❌ Negative | Default | Same family, share diseases |
| Yam | Beet | ❌ Negative | Default | Compete for similar soil resources |
| Beet | Bok Choy | ✅ Positive | Default | Improves soil conditions for bok choy |

---

## 💎 Special Crop Interactions

| Source | Target | Effect | Type | Description |
|--------|--------|--------|------|-------------|
| Ancient Fruit | All nearby crops | ✅ Positive | Default | Mysterious beneficial properties (radius: 2) |
| Any crop | Sweet Gem Berry | ❌ Negative | Default | Extremely sensitive to any nearby plants |
| Coffee Bean | All nearby crops | ✅ Positive | Default | Releases compounds that stimulate growth |

---

## 🎮 How Effects Work

Each interaction has a **type** that determines what happens to the target crop:

| Effect Type | ✅ Positive | ❌ Negative |
|-------------|-------------|-------------|
| ⚡ **Growth Speed** | Advances growth phase | Delays day progress in current phase |
| ⭐ **Quality** | Applies/upgrades fertilizer (Basic → Quality → Deluxe) | Downgrades/removes fertilizer |
| 📦 **Quantity** | Advances growth (more harvest cycles) | Resets day progress (fewer harvests) |
| 💧 **Default** | Auto-waters dry soil | Dries out watered soil |

Effects are processed **once at the start of each day** and **every 2 in-game hours**. Each effect has a random chance to trigger based on the interaction's strength multiplied by your configured Effect Strength.

---

## ⚙️ Configuration

All options can be changed via **Generic Mod Config Menu** or by editing `config.json`:

| Option | Default | Description |
|--------|---------|-------------|
| `EnableMod` | `true` | Enable or disable the mod |
| `EffectStrength` | `0.5` | Global multiplier for all effects (0.0 – 1.0) |
| `ShowEffectMessages` | `true` | Show in-game messages when interactions occur |
| `ShowParticleEffects` | `true` | Show sparkle particles on affected crops |
| `ShowHoverText` | `true` | Show tooltip when hovering over affected crops |

---

## 📥 Installation

1. Install [SMAPI](https://smapi.io/) (4.0+)
2. *(Optional)* Install [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) for in-game configuration
3. Download this mod and unzip it into your `Stardew Valley/Mods` folder
4. Launch the game through SMAPI

---

## 🔧 Compatibility

- ✅ Stardew Valley **1.6+**
- ✅ Windows, macOS, and Linux
- ✅ Single-player and multiplayer
- ✅ Compatible with most other mods
- ✅ Works on Farm, Greenhouse, and Ginger Island

---

## 🌐 Supported Languages

| Language | Status |
|----------|--------|
| English | ✅ Full support (default) |
| Português (Brazilian) | ✅ Full support |

Want to help translate? Create a new file in the `i18n` folder following the [SMAPI translation guide](https://stardewvalleywiki.com/Modding:Translations).

---

## 💡 Tips for Players

- 🫘 **Green Beans are your best friend** — They fix nitrogen and benefit ALL nearby crops. Plant them everywhere!
- 🌻 **Sunflowers and Fairy Roses are area buffers** — They help all crops in a 2-tile radius
- 🍓 **Don't mass-plant strawberries** — They compete with each other when too close
- 🎃 **Isolate your pumpkins** — They drain nutrients from everything nearby
- 💠 **Give Sweet Gem Berries space** — Any crop next to them causes negative effects
- ☕ **Coffee is a great neighbor** — Stimulates growth in all nearby plants
- 🧄🍓 **Garlic + Strawberry** is one of the strongest positive combos in spring

---

## 🗺️ Future Plans

- 🎨 Visual tile indicators when holding seeds (showing compatible/incompatible spots)
- 📖 In-game almanac with all crop interactions
- 🌿 Support for modded crops (Cornucopia and others)
- 🏆 Achievements for discovering crop combinations

---

## 👤 Credits

Developed by **Lazy Mug** 🍵

---

*"Nature is not a place to visit. It is home." — Gary Snyder* 🌍
