using System.Reflection.Emit;
using ElementalForce.Elemental_Force_Code.helpers;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ElementalForce.Elemental_Force_Code.harmony
{
    [HarmonyPatch(typeof(InventoryMenu))]
    public static class InventoryMenuPatch
    {
        [HarmonyPatch(nameof(InventoryMenu.rightClick))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> RightClickTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            // Locate the Tool.canThisBeAttached method explicitly
            var targetMethod = AccessTools.Method(
                typeof(Tool),
                nameof(Tool.canThisBeAttached),
                new[] { typeof(StardewValley.Object) }
            );

            if (targetMethod == null)
                throw new Exception("Harmony: Failed to locate Tool.canThisBeAttached method.");

            // Get the custom replacement method
            var customMethod = AccessTools.Method(
                typeof(InventoryMenuPatch),
                nameof(CustomCanThisBeAttached)
            );

            for (int i = 0; i < codes.Count; i++)
            {
                // Find the call to `Tool.canThisBeAttached`
                if (codes[i].Calls(targetMethod))
                {
                    // Replace it with a call to our custom method
                    codes[i] = new CodeInstruction(OpCodes.Call, customMethod);
                }
            }

            return codes;
        }

        public static bool CustomCanThisBeAttached(Tool tool, StardewValley.Object? obj)
        {
            if (ItemHelper.IsAmphoraTool(tool.ItemId))
            {
                ModEntry.Instance.OnCheckIfEquipmentHasChanged();
                
                if (obj != null)
                {
                    var canThisBeAttached = ItemHelper.IsElementalEssenceItem(obj.ItemId);
                    return canThisBeAttached;
                }
            }

            // Default logic for other tools
            return tool.canThisBeAttached(obj);
        }
    }
}