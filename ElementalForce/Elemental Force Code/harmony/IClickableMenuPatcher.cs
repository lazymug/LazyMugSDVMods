
using System.Drawing;
using System.Text;
using ElementalForce.Elemental_Force_Code.helpers;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ElementalForce.Elemental_Force_Code.harmony
{
    [HarmonyPatch(typeof(IClickableMenu))]
    [HarmonyPatch(nameof(IClickableMenu.drawHoverText))]
    public static class DrawHoverTextPatcher
    {
        public static bool Prefix(
            SpriteBatch b,
            StringBuilder text,
            SpriteFont font,
            int xOffset = 0,
            int yOffset = 0,
            int moneyAmountToDisplayAtBottom = -1,
            string boldTitleText = null,
            int healAmountToDisplay = -1,
            string[] buffIconsToDisplay = null,
            Item hoveredItem = null,
            int currencySymbol = 0,
            string extraItemToShowIndex = null,
            int extraItemToShowAmount = -1,
            int overrideX = -1,
            int overrideY = -1,
            float alpha = 1f,
            CraftingRecipe craftingIngredients = null,
            IList<Item> additional_craft_materials = null,
            Texture2D boxTexture = null,
            Rectangle? boxSourceRect = null,
            Color? textColor = null,
            Color? textShadowColor = null,
            float boxScale = 1f,
            int boxWidthOverride = -1,
            int boxHeightOverride = -1)
        {
            if (hoveredItem is Tool tool)
            {
                if (ItemHelper.IsAmphoraTool(tool.ItemId))
                {
                    DrawBaseAmphoraHoverText(b, font, xOffset, yOffset, alpha, hoveredItem);
                    return false;
                }
                else if (ItemHelper.IsAmphoraLevel2Tool(tool.ItemId))
                {
                    DrawLevel2AmphoraHoverText(b, font, xOffset, yOffset, alpha, hoveredItem);
                    return false;
                }
                else if (ItemHelper.IsAmphoraLevel3Tool(tool.ItemId))
                {
                    DrawLevel3AmphoraHoverText(b, font, xOffset, yOffset, alpha, hoveredItem);
                    return false;
                }
            }
            return true;
        }

        private static void DrawBaseAmphoraHoverText(SpriteBatch b, SpriteFont font, int xOffset, int yOffset, float alpha, Item hoveredItem)
        {
            var description = new StringBuilder();
            description.AppendLine("Base Amphora");
            description.AppendLine();
            description.AppendLine("A mystical vessel that can harness elemental powers.");
            description.AppendLine();
            description.AppendLine("Available Slots: 2 (Essence)");
            description.AppendLine();
            description.AppendLine("Right-click to attach an essence and unlock its Level 1 buff.");
            description.AppendLine();
            description.AppendLine("Attached Essences:");
            if (hoveredItem is Tool tool)
            {
                var attachedEssences = tool.attachments?.Where(a => a != null && ItemHelper.IsElementalEssenceItem(a.ItemId));
                if (attachedEssences?.Any() == true)
                {
                    foreach (var essence in attachedEssences)
                    {
                        var type = essence.ItemId.Contains("Carbuncle") ? "Carbuncle" :
                                  essence.ItemId.Contains("Ifrit") ? "Ifrit" :
                                  essence.ItemId.Contains("Shiva") ? "Shiva" :
                                  essence.ItemId.Contains("Titan") ? "Titan" :
                                  essence.ItemId.Contains("Leviathan") ? "Leviathan" :
                                  essence.ItemId.Contains("Phoenix") ? "Phoenix" :
                                  essence.ItemId.Contains("Kirin") ? "Kirin" :
                                  essence.ItemId.Contains("Ramuh") ? "Ramuh" : "Unknown";
                        description.AppendLine($"- {type} Essence (Level 1)");
                    }
                }
                else
                {
                    description.AppendLine("None");
                }
            }

            IClickableMenu.drawHoverText(b, description, font, xOffset, yOffset, -1, null, -1, null, null, 0, null, -1, -1, -1, alpha);
        }

        private static void DrawLevel2AmphoraHoverText(SpriteBatch b, SpriteFont font, int xOffset, int yOffset, float alpha, Item hoveredItem)
        {
            var description = new StringBuilder();
            description.AppendLine("Amphora of Echoes");
            description.AppendLine();
            description.AppendLine("An enhanced vessel that resonates with crystallized elemental power.");
            description.AppendLine();
            description.AppendLine("Available Slots: 1 (Shard) | 3 (Essence)");
            description.AppendLine();
            description.AppendLine("Right-click to attach a Level 2 shard and unlock enhanced abilities.");
            description.AppendLine();
            description.AppendLine("Attached Powers:");
            if (hoveredItem is Tool tool)
            {
                var attachments = tool.attachments?.Where(a => a != null);
                if (attachments?.Any() == true)
                {
                    foreach (var attachment in attachments)
                    {
                        var type = attachment.ItemId.Contains("Carbuncle") ? "Carbuncle" :
                                  attachment.ItemId.Contains("Ifrit") ? "Ifrit" :
                                  attachment.ItemId.Contains("Shiva") ? "Shiva" :
                                  attachment.ItemId.Contains("Titan") ? "Titan" :
                                  attachment.ItemId.Contains("Leviathan") ? "Leviathan" :
                                  attachment.ItemId.Contains("Phoenix") ? "Phoenix" :
                                  attachment.ItemId.Contains("Kirin") ? "Kirin" :
                                  attachment.ItemId.Contains("Ramuh") ? "Ramuh" : "Unknown";

                        if (ItemHelper.IsElementalEssenceItem(attachment.ItemId))
                        {
                            description.AppendLine($"- {type} Essence (Level 1)");
                        }
                        else if (ItemHelper.IsElementalShardItem(attachment.ItemId))
                        {
                            description.AppendLine($"- {type} Shard (Level 2)");
                        }
                    }
                }
                else
                {
                    description.AppendLine("None");
                }
            }

            IClickableMenu.drawHoverText(b, description, font, xOffset, yOffset, -1, null, -1, null, null, 0, null, -1, -1, -1, alpha);
        }

        private static void DrawLevel3AmphoraHoverText(SpriteBatch b, SpriteFont font, int xOffset, int yOffset, float alpha, Item hoveredItem)
        {
            var description = new StringBuilder();
            description.AppendLine("Amphora of Spirits");
            description.AppendLine();
            description.AppendLine("A legendary vessel that channels the pure essence of elemental souls.");
            description.AppendLine();
            description.AppendLine("Available Slots: 1 (Soul) | 3 (Shard) | 6 (Essence)");
            description.AppendLine();
            description.AppendLine("Right-click to attach a Level 3 soul and unlock its ultimate power.");
            description.AppendLine();
            description.AppendLine("Attached Powers:");
            if (hoveredItem is Tool tool)
            {
                var attachments = tool.attachments?.Where(a => a != null);
                if (attachments?.Any() == true)
                {
                    foreach (var attachment in attachments)
                    {
                        var type = attachment.ItemId.Contains("Carbuncle") ? "Carbuncle" :
                                  attachment.ItemId.Contains("Ifrit") ? "Ifrit" :
                                  attachment.ItemId.Contains("Shiva") ? "Shiva" :
                                  attachment.ItemId.Contains("Titan") ? "Titan" :
                                  attachment.ItemId.Contains("Leviathan") ? "Leviathan" :
                                  attachment.ItemId.Contains("Phoenix") ? "Phoenix" :
                                  attachment.ItemId.Contains("Kirin") ? "Kirin" :
                                  attachment.ItemId.Contains("Ramuh") ? "Ramuh" : "Unknown";

                        if (ItemHelper.IsElementalEssenceItem(attachment.ItemId))
                        {
                            description.AppendLine($"- {type} Essence (Level 1)");
                        }
                        else if (ItemHelper.IsElementalShardItem(attachment.ItemId))
                        {
                            description.AppendLine($"- {type} Shard (Level 2)");
                        }
                        else if (ItemHelper.IsElementalSoulItem(attachment.ItemId))
                        {
                            description.AppendLine($"- {type} Soul (Level 3)");
                        }
                    }
                }
                else
                {
                    description.AppendLine("None");
                }
            }

            IClickableMenu.drawHoverText(b, description, font, xOffset, yOffset, -1, null, -1, null, null, 0, null, -1, -1, -1, alpha);
        }
    }
}