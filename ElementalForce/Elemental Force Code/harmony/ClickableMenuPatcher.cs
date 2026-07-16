using ElementalForce.Elemental_Force_Code.helpers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ElementalForce.Elemental_Force_Code.harmony;

[HarmonyPatch(typeof(IClickableMenu))]
public class ClickableMenuPatcher
{
    private const int SlotSize = 64;
    private const int Padding = 16;
    private const int SectionSpacing = 8;
    private const int SlotGap = 4;

    [HarmonyPatch(nameof(IClickableMenu.drawToolTip),
        new[]
        {
            typeof(SpriteBatch), typeof(string), typeof(string), typeof(Item),
            typeof(bool), typeof(int), typeof(int), typeof(string),
            typeof(int), typeof(CraftingRecipe), typeof(int), typeof(IList<Item>)
        })]
    [HarmonyPrefix]
    public static bool Prefix_drawToolTip(
        SpriteBatch b,
        string hoverText,
        string hoverTitle,
        Item hoveredItem,
        bool heldItem)
    {
        if (hoveredItem is not Tool tool || !ItemHelper.IsAnyAmphoraTool(tool.ItemId))
            return true;

        if (tool.attachments == null || tool.attachments.Length == 0)
            return true;

        DrawCustomAmphoraTooltip(b, hoverText, hoverTitle, tool, heldItem);
        return false;
    }

    private static void DrawCustomAmphoraTooltip(SpriteBatch b, string hoverText, string hoverTitle, Tool tool, bool heldItem)
    {
        var titleFont = Game1.dialogueFont;
        var textFont = Game1.smallFont;

        if (string.IsNullOrEmpty(hoverTitle))
            hoverTitle = tool.DisplayName;
        if (string.IsNullOrEmpty(hoverText))
            hoverText = tool.getDescription();

        var sections = GetSections(tool);

        int maxSlotRowWidth = 0;
        foreach (var section in sections)
        {
            int rowWidth = section.Count * SlotSize + (section.Count - 1) * SlotGap;
            if (rowWidth > maxSlotRowWidth)
                maxSlotRowWidth = rowWidth;
        }

        int minDescWidth = Math.Max(272, maxSlotRowWidth);
        string wrappedText = Game1.parseText(hoverText, textFont, minDescWidth);

        Vector2 titleSize = titleFont.MeasureString(hoverTitle);
        Vector2 descSize = textFont.MeasureString(wrappedText);

        int contentWidth = (int)Math.Max(titleSize.X, Math.Max(descSize.X, maxSlotRowWidth));
        int tooltipWidth = contentWidth + Padding * 2 + 16;

        int tooltipHeight = Padding;

        tooltipHeight += (int)titleSize.Y + 4;
        tooltipHeight += (int)descSize.Y + Padding;

        foreach (var section in sections)
        {
            tooltipHeight += (int)textFont.MeasureString(section.Label).Y + SlotGap;
            tooltipHeight += SlotSize + SectionSpacing;
        }

        tooltipHeight += Padding / 2;

        int mouseX = Game1.getOldMouseX();
        int mouseY = Game1.getOldMouseY();
        int x = mouseX + 32 + (heldItem ? 48 : 0);
        int y = mouseY + 32;

        var safeArea = Utility.getSafeArea();
        if (x + tooltipWidth > safeArea.Right)
            x = safeArea.Right - tooltipWidth;
        if (y + tooltipHeight > safeArea.Bottom)
            y = safeArea.Bottom - tooltipHeight;
        if (x < safeArea.X)
            x = safeArea.X;
        if (y < safeArea.Y)
            y = safeArea.Y;

        IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
            x, y, tooltipWidth, tooltipHeight, Color.White, drawShadow: true);

        int currentY = y + Padding;

        Utility.drawTextWithShadow(b, hoverTitle, titleFont,
            new Vector2(x + Padding, currentY), Game1.textColor);
        currentY += (int)titleSize.Y + 4;

        Utility.drawTextWithShadow(b, wrappedText, textFont,
            new Vector2(x + Padding, currentY), Game1.textColor);
        currentY += (int)descSize.Y + Padding;

        foreach (var section in sections)
        {
            Utility.drawTextWithShadow(b, section.Label, textFont,
                new Vector2(x + Padding, currentY), Color.DarkGoldenrod);
            currentY += (int)textFont.MeasureString(section.Label).Y + SlotGap;

            int slotX = x + Padding;
            for (int i = section.StartIndex; i < section.StartIndex + section.Count; i++)
            {
                b.Draw(Game1.menuTexture, new Vector2(slotX, currentY),
                    new Rectangle(128, 128, 64, 64), Color.White);

                if (i < tool.attachments.Length && tool.attachments[i] != null)
                {
                    tool.attachments[i].drawInMenu(b, new Vector2(slotX, currentY), 1f);
                }

                slotX += SlotSize + SlotGap;
            }

            currentY += SlotSize + SectionSpacing;
        }
    }

    private static List<SlotSection> GetSections(Tool tool)
    {
        var sections = new List<SlotSection>();
        int slotCount = tool.attachments.Length;
        string essences = ModEntry.Instance.GetTextTranslation("tooltip.section.essences");
        string shards = ModEntry.Instance.GetTextTranslation("tooltip.section.shards");
        string souls = ModEntry.Instance.GetTextTranslation("tooltip.section.souls");

        if (slotCount == 2)
        {
            sections.Add(new SlotSection(essences, 0, 2));
        }
        else if (slotCount == 4)
        {
            sections.Add(new SlotSection(shards, 0, 1));
            sections.Add(new SlotSection(essences, 1, 3));
        }
        else if (slotCount == 10)
        {
            sections.Add(new SlotSection(souls, 0, 1));
            sections.Add(new SlotSection(shards, 1, 3));
            sections.Add(new SlotSection(essences, 4, 6));
        }

        return sections;
    }

    private record SlotSection(string Label, int StartIndex, int Count);
}
