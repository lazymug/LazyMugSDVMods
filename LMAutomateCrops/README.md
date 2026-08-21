# LM Automate Crops

Teaches [Automate](https://www.nexusmods.com/stardewvalley/mods/1063) to harvest crops.

A tilled tile with a crop is exposed to Automate as a machine: it becomes *ready* when the crop is
ready to pick, and the produce is delivered into a connected chest. Harvesting this way grants the
same Farming experience as doing it by hand, and one-off crops can be replanted from seeds in the
same chests.

## Why this is a separate mod

Registering with Automate means implementing its `IAutomationFactory` and `IMachine` interfaces.
Implementing an interface (unlike calling an API) needs a compile-time reference to `Automate.dll`,
and SMAPI's assembly rewriter refuses to load a mod whose references it can't resolve. Folding this
into LM QoL would therefore have made Automate a hard requirement for that whole mod, so it lives
here instead, with Automate as a required dependency.

## How the harvest is calculated

`Crop.harvest` can't be reused directly — it pushes the produce into the player's inventory, plays
animations and sets `canMove = false`, none of which suits a machine ticking away in the
background. `CropHarvest` therefore mirrors its maths:

- **Quality** — the fertiliser and Farming-level roll, clamped to the crop's own min/max quality
- **Stack size** — `HarvestMinStack`..`HarvestMaxStack`, the per-level increase, the extra-harvest
  chance, and the luck-based double harvest
- **Bonus drops** — hay from wheat (40%), mixed seeds from fibre (10%)
- **Experience** — `16 * ln(0.018 * price + 1)`, the same formula the game uses

The crop is only cleared, and the experience only paid, once Automate has actually moved the
produce into a chest.

## Settings

Configurable in Generic Mod Config Menu:

| Setting | Default | Notes |
|---|---|---|
| Enable Crop Harvesting | on | |
| Grant Farming Experience | on | Turn off if automation shouldn't level you up |
| Replant Automatically | on | Uses seeds from the connected chests; only sows what can grow there |
| Include Greenhouse | on | |
| Include Ginger Island | on | |

## Notes

- Forage crops (spring onions and the like) are left alone — they're an interactive harvest.
- Dead crops are ignored rather than cleared, so you can still see what failed.
- Experience is credited to the main player, the way a Junimo hut's output belongs to its owner.
