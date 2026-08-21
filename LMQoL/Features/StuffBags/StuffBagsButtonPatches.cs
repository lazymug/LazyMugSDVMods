using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace LMQoL.Features.StuffBags
{
    /// <summary>Adds a "stuff bags" button to the inventory page, under the organise button.
    ///
    /// The page builds its own buttons in the constructor, so ours is positioned relative to
    /// <c>organizeButton</c> and drawn, clicked and hovered through the same three methods the
    /// vanilla buttons use.</summary>
    internal static class StuffBagsButtonPatches
    {
        private static ClickableTextureComponent? Button;

        /// <summary>Sits one slot below the organise button, matching its size and column.</summary>
        private static void Reposition(InventoryPage page)
        {
            var anchor = page.organizeButton;
            if (anchor == null)
                return;

            Button ??= new ClickableTextureComponent(
                "",
                new Rectangle(0, 0, 64, 64),
                "",
                ModEntry.ModHelper.Translation.Get("stuffbags.button"),
                Game1.mouseCursors,
                new Rectangle(274, 284, 16, 16),   // the "bag" cursor icon
                4f);

            Button.bounds = new Rectangle(anchor.bounds.X, anchor.bounds.Y + 72, 64, 64);
            Button.hoverText = ModEntry.ModHelper.Translation.Get("stuffbags.button");
        }

        [HarmonyPatch(typeof(InventoryPage), nameof(InventoryPage.draw), new[] { typeof(SpriteBatch) })]
        internal static class DrawPatch
        {
            internal static void Postfix(InventoryPage __instance, SpriteBatch b)
            {
                if (!StuffBagsFeature.Available)
                    return;

                Reposition(__instance);
                Button?.draw(b);

                // the vanilla page draws its tooltip before we add ours, so redraw the cursor over it
                if (Button != null && Button.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
                    IClickableMenu.drawHoverText(b, Button.hoverText, Game1.smallFont);

                __instance.drawMouse(b);
            }
        }

        [HarmonyPatch(typeof(InventoryPage), nameof(InventoryPage.receiveLeftClick))]
        internal static class ClickPatch
        {
            internal static void Postfix(int x, int y)
            {
                if (!StuffBagsFeature.Available || Button == null || !Button.containsPoint(x, y))
                    return;

                Game1.playSound("Ship");
                ModEntry.StuffBags.Run();
            }
        }

        [HarmonyPatch(typeof(InventoryPage), nameof(InventoryPage.performHoverAction))]
        internal static class HoverPatch
        {
            internal static void Postfix(int x, int y)
            {
                if (!StuffBagsFeature.Available || Button == null)
                    return;

                Button.tryHover(x, y, 0.1f);
            }
        }
    }
}
