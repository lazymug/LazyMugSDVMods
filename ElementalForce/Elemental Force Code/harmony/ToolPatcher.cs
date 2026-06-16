using ElementalForce.Elemental_Force_Code.helpers;
using HarmonyLib;
using StardewValley;
using Object = StardewValley.Object;

namespace ElementalForce.Elemental_Force_Code.harmony;

[HarmonyPatch(typeof(Tool))]
public class ToolPatcher
{

    [HarmonyPatch(nameof(Tool.attach))]
    [HarmonyPatch(new[] { typeof(Object) })]
    [HarmonyPrefix]
    public static bool Prefix_attach(Tool __instance, Object o, ref Object? __result)
    {
        if (!ItemHelper.IsAnyAmphoraTool(__instance.ItemId))
            return true;

        __result = CustomAttach(__instance, o);
        return false;
    }

    private static Object? CustomAttach(Tool __instance, Object? o)
    {
        if (o == null)
        {
            for (int i = __instance.attachments.Length - 1; i >= 0; i--)
            {
                if (__instance.attachments[i] != null)
                {
                    Object detached = __instance.attachments[i];
                    __instance.attachments[i] = null;
                    Game1.playSound("dwop");
                    return detached;
                }
            }
            return null;
        }

        int attachmentsLength = __instance.attachments.Length;

        if (attachmentsLength == 2)
        {
            if (ItemHelper.IsElementalEssenceItem(o.ItemId))
            {
                return AttachToSlotRange(__instance, o, 0, attachmentsLength);
            }
        }
        else if (attachmentsLength == 4)
        {
            if (ItemHelper.IsElementalShardItem(o.ItemId))
            {
                return AttachToSlotRange(__instance, o, 0, 1);
            }
            else if (ItemHelper.IsElementalEssenceItem(o.ItemId))
            {
                return AttachToSlotRange(__instance, o, 1, attachmentsLength);
            }
        }
        else if (attachmentsLength == 10)
        {
            if (ItemHelper.IsElementalSoulItem(o.ItemId))
            {
                return AttachToSlotRange(__instance, o, 0, 1);
            }
            else if (ItemHelper.IsElementalShardItem(o.ItemId))
            {
                return AttachToSlotRange(__instance, o, 1, 4);
            }
            else if (ItemHelper.IsElementalEssenceItem(o.ItemId))
            {
                return AttachToSlotRange(__instance, o, 4, attachmentsLength);
            }
        }

        return o;
    }

    private static Object? AttachToSlotRange(Tool tool, Object item, int startSlot, int endSlot)
    {
        for (int index = startSlot; index < endSlot; ++index)
        {
            if (tool.attachments[index] == null)
            {
                tool.attachments[index] = item;
                Game1.playSound("button1");
                return null;
            }
        }

        var temp = tool.attachments[startSlot];
        tool.attachments[startSlot] = item;
        Game1.playSound("button1");
        return temp;
    }

    [HarmonyPatch(nameof(Tool.beginUsing))]
    [HarmonyPatch(new []{ typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) })]
    [HarmonyPrefix]
    public static bool Prefix_beginUsing(Tool __instance, GameLocation location, int x, int y, Farmer who)
    {
        return !ItemHelper.IsAnyAmphoraTool(__instance.ItemId);
    }
}
