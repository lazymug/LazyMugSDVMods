using System.Reflection;
using ElementalForce.Elemental_Force_Code.helpers;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
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
        // Check if it's a vanilla tool first
        if (!ItemHelper.IsAmphoraTool(__instance.ItemId) && 
            !ItemHelper.IsAmphoraLevel2Tool(__instance.ItemId) && 
            !ItemHelper.IsAmphoraLevel3Tool(__instance.ItemId))
        {
            // Let vanilla code handle it
            return true;
        }

        // Handle Amphora tools with custom logic
        __result = CustomAttach(__instance, o);
        return false;
    }

    private static Object? CustomAttach(Tool __instance, Object? o)
    {
        // Handle detachment (when o is null)
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

        // Handle different Amphora tool types
        if (attachmentsLength == 2)  // Basic Amphora
        {
            // Tool with 2 attachments can only attach Essence items
            if (ItemHelper.IsElementalEssenceItem(o.ItemId))
            {
                for (int index = 0; index < attachmentsLength; ++index)
                {
                    if (__instance.attachments[index] == null)
                    {
                        __instance.attachments[index] = o;
                        Game1.playSound("button1");
                        return null;
                    }
                }
                
                // If no empty slots, swap with first slot
                var temp = __instance.attachments[0];
                __instance.attachments[0] = o;
                Game1.playSound("button1");
                return temp;
            }
        }
        else if (attachmentsLength == 4)  // Echoes Amphora
        {
            // Tool with 4 attachments can only attach Shard items at the first slot and Essence items on the other slots
            if (ItemHelper.IsElementalShardItem(o.ItemId))
            {
                if (__instance.attachments[0] == null)
                {
                    __instance.attachments[0] = o;
                    Game1.playSound("button1");
                    return null;
                }
                
                var temp = __instance.attachments[0];
                __instance.attachments[0] = o;
                Game1.playSound("button1");
                return temp;
            }
            else if (ItemHelper.IsElementalEssenceItem(o.ItemId))
            {
                for (int index = 1; index < attachmentsLength; ++index)
                {
                    if (__instance.attachments[index] == null)
                    {
                        __instance.attachments[index] = o;
                        Game1.playSound("button1");
                        return null;
                    }
                }
                
                // If no empty slots, swap with first available essence slot
                var temp = __instance.attachments[1];
                __instance.attachments[1] = o;
                Game1.playSound("button1");
                return temp;
            }
        }
        else if (attachmentsLength == 10)  // Spirits Amphora
        {
            // Tool with 10 attachments can only attach Soul items at the first slot, Shard items at the next three slots, and Essence items on the other slots
            if (ItemHelper.IsElementalSoulItem(o.ItemId))
            {
                if (__instance.attachments[0] == null)
                {
                    __instance.attachments[0] = o;
                    Game1.playSound("button1");
                    return null;
                }
                
                var temp = __instance.attachments[0];
                __instance.attachments[0] = o;
                Game1.playSound("button1");
                return temp;
            }
            else if (ItemHelper.IsElementalShardItem(o.ItemId))
            {
                for (int index = 1; index < 4; ++index)
                {
                    if (__instance.attachments[index] == null)
                    {
                        __instance.attachments[index] = o;
                        Game1.playSound("button1");
                        return null;
                    }
                }
                
                // If no empty slots, swap with first available shard slot
                var temp = __instance.attachments[1];
                __instance.attachments[1] = o;
                Game1.playSound("button1");
                return temp;
            }
            else if (ItemHelper.IsElementalEssenceItem(o.ItemId))
            {
                for (int index = 4; index < attachmentsLength; ++index)
                {
                    if (__instance.attachments[index] == null)
                    {
                        __instance.attachments[index] = o;
                        Game1.playSound("button1");
                        return null;
                    }
                }
                
                // If no empty slots, swap with first available essence slot
                var temp = __instance.attachments[4];
                __instance.attachments[4] = o;
                Game1.playSound("button1");
                return temp;
            }
        }
        
        return o;
    }

    [HarmonyPatch(nameof(Tool.beginUsing))]
    [HarmonyPatch(new []{ typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) })]
    [HarmonyPrefix]
    public static bool Prefix_beginUsing(Tool __instance, GameLocation location, int x, int y, Farmer who)
    {
        return __instance.ItemId != ItemHelper.GetToolAmphoraId() &&
               __instance.ItemId != ItemHelper.GetToolAmphoraEchoesId() &&
               __instance.ItemId != ItemHelper.GetToolAmphoraSpiritsId();
    }
    
    // [HarmonyPatch(nameof(Tool.drawAttachments))]
    // [HarmonyPatch(new []{ typeof(SpriteBatch), typeof(int), typeof(int) })]
    // [HarmonyPrefix]
    // public static bool Prefix_drawAttachments(Tool __instance, SpriteBatch b, int x, int y)
    // {
    //     var isToolAmphora = __instance.ItemId == ItemHelper.GetToolAmphoraId() ||
    //                         __instance.ItemId == ItemHelper.GetToolAmphoraEchoesId() ||
    //                         __instance.ItemId == ItemHelper.GetToolAmphoraSpiritsId();
    //     // if (!isToolAmphora)
    //     // {
    //     //     return true;
    //     // }
    //     //
    //     // y += __instance.enchantments.Count > 0 ? 8 : 4;
    //     // var method = typeof(Tool).GetMethod("DrawAttachmentSlot", BindingFlags.NonPublic | BindingFlags.Instance);
    //     // if (__instance.ItemId == ItemHelper.GetToolAmphoraId())
    //     // {
    //     //     for (var slot = 0; slot < __instance.AttachmentSlotsCount; slot++)
    //     //     {
    //     //         method?.Invoke(__instance, new object[] { slot, b, x + slot * 68, y });
    //     //     }
    //     // }
    //     // else if (__instance.ItemId == ItemHelper.GetToolAmphoraEchoesId())
    //     // {
    //     //     for (var slot = 0; slot < __instance.AttachmentSlotsCount; slot++)
    //     //     {
    //     //         method?.Invoke(__instance, new object[] { slot, b, x + slot * 68, y });
    //     //     }
    //     // }
    //     // else if (__instance.ItemId == ItemHelper.GetToolAmphoraSpiritsId())
    //     // {
    //     //     for (var slot = 0; slot < __instance.AttachmentSlotsCount; slot++)
    //     //     {
    //     //         method?.Invoke(__instance, new object[] { slot, b, x + slot * 68, y });
    //     //     }
    //     // }
    //
    //     return false;
    // }
}