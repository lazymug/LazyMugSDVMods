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
        __result = 
            InventoryMenuPatcher.CustomCanThisBeAttached(__instance, o) ? 
                CustomAttach(__instance, o) : __instance.attach(o);

        return false;
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
    
    [HarmonyPatch(nameof(Tool.drawAttachments))]
    [HarmonyPatch(new []{ typeof(SpriteBatch), typeof(int), typeof(int) })]
    [HarmonyPrefix]
    public static bool Prefix_drawAttachments(Tool __instance, SpriteBatch b, int x, int y)
    {
        var isToolAmphora = __instance.ItemId == ItemHelper.GetToolAmphoraId() ||
                            __instance.ItemId == ItemHelper.GetToolAmphoraEchoesId() ||
                            __instance.ItemId == ItemHelper.GetToolAmphoraSpiritsId();
        // if (!isToolAmphora)
        // {
        //     return true;
        // }
        //
        // y += __instance.enchantments.Count > 0 ? 8 : 4;
        // var method = typeof(Tool).GetMethod("DrawAttachmentSlot", BindingFlags.NonPublic | BindingFlags.Instance);
        // if (__instance.ItemId == ItemHelper.GetToolAmphoraId())
        // {
        //     for (var slot = 0; slot < __instance.AttachmentSlotsCount; slot++)
        //     {
        //         method?.Invoke(__instance, new object[] { slot, b, x + slot * 68, y });
        //     }
        // }
        // else if (__instance.ItemId == ItemHelper.GetToolAmphoraEchoesId())
        // {
        //     for (var slot = 0; slot < __instance.AttachmentSlotsCount; slot++)
        //     {
        //         method?.Invoke(__instance, new object[] { slot, b, x + slot * 68, y });
        //     }
        // }
        // else if (__instance.ItemId == ItemHelper.GetToolAmphoraSpiritsId())
        // {
        //     for (var slot = 0; slot < __instance.AttachmentSlotsCount; slot++)
        //     {
        //         method?.Invoke(__instance, new object[] { slot, b, x + slot * 68, y });
        //     }
        // }

        return false;
    }

    private static Object? CustomAttach(Tool __instance, Object? o)
    {
        if (o == null)
        {
            for (int index = 0; index < __instance.attachments.Length; ++index)
            {
                Object attachment = __instance.attachments[index];
                if (attachment != null)
                {
                    __instance.attachments[index] = (Object) null;
                    Game1.playSound("dwop");
                    return attachment;
                }
            }
            return (Object) null;
        }
        
        int stack = o.Stack;
        int attachmentsLength = __instance.attachments.Length;

        if (attachmentsLength == 2)
        {
            // Tool with 2 attachments can only attach Essence items
            if (ItemHelper.IsElementalEssenceItem(o.ItemId))
            {
                for (int index = 0; index < attachmentsLength; ++index)
                {
                    Object attachment = __instance.attachments[index];
                    if (attachment == null)
                    {
                        __instance.attachments[index] = o;
                        o = (Object)null;
                        break;
                    }

                    if (attachment.canStackWith((ISalable)o))
                    {
                        int amount = o.Stack - attachment.addToStack((Item)o);
                        if (o.ConsumeStack(amount) == null)
                        {
                            o = (Object)null;
                            break;
                        }
                    }

                    (o, __instance.attachments[index]) = (__instance.attachments[index], o);
                }
            }
        }
        else if (attachmentsLength == 4)
        {
            // Tool with 4 attachments can only attach Shard items at the first slot and Essence items on the other slots
            if (ItemHelper.IsElementalShardItem(o.ItemId))
            {
                if (__instance.attachments[0] == null)
                {
                    __instance.attachments[0] = o;
                    o = (Object)null;
                }
                else
                {
                    (o, __instance.attachments[0]) = (__instance.attachments[0], o);
                }
            }
            else if (ItemHelper.IsElementalEssenceItem(o.ItemId))
            {
                for (int index = 1; index < attachmentsLength; ++index)
                {
                    Object attachment = __instance.attachments[index];
                    if (attachment == null)
                    {
                        __instance.attachments[index] = o;
                        o = (Object)null;
                        break;
                    }

                    if (attachment.canStackWith((ISalable)o))
                    {
                        int amount = o.Stack - attachment.addToStack((Item)o);
                        if (o.ConsumeStack(amount) == null)
                        {
                            o = (Object)null;
                            break;
                        }
                    }
                    
                    (o, __instance.attachments[index]) = (__instance.attachments[index], o);
                }
            }
        }
        else if (attachmentsLength == 10)
        {
            // Tool with 10 attachments can only attach Soul items at the first slot, Shard items at the next three slots, and Essence items on the other slots
            if (ItemHelper.IsElementalSoulItem(o.ItemId))
            {
                if (__instance.attachments[0] == null)
                {
                    __instance.attachments[0] = o;
                    o = (Object)null;
                }
                else
                {
                    (o, __instance.attachments[0]) = (__instance.attachments[0], o);
                }
            }
            else if (ItemHelper.IsElementalShardItem(o.ItemId))
            {
                for (int index = 1; index < 4; ++index)
                {
                    Object attachment = __instance.attachments[index];
                    if (attachment == null)
                    {
                        __instance.attachments[index] = o;
                        o = (Object)null;
                        break;
                    }

                    if (attachment.canStackWith((ISalable)o))
                    {
                        int amount = o.Stack - attachment.addToStack((Item)o);
                        if (o.ConsumeStack(amount) == null)
                        {
                            o = (Object)null;
                            break;
                        }
                    }
                    
                    (o, __instance.attachments[index]) = (__instance.attachments[index], o);
                }
            }
            else if (ItemHelper.IsElementalEssenceItem(o.ItemId))
            {
                for (int index = 4; index < attachmentsLength; ++index)
                {
                    Object attachment = __instance.attachments[index];
                    if (attachment == null)
                    {
                        __instance.attachments[index] = o;
                        o = (Object)null;
                        break;
                    }

                    if (attachment.canStackWith((ISalable)o))
                    {
                        int amount = o.Stack - attachment.addToStack((Item)o);
                        if (o.ConsumeStack(amount) == null)
                        {
                            o = (Object)null;
                            break;
                        }
                    }
                    
                    (o, __instance.attachments[index]) = (__instance.attachments[index], o);
                }
            }
        }
        
        if (o == null || o.Stack != stack)
        {
            Game1.playSound("button1");
            return o;
        }

        for (int index = 0; index < __instance.attachments.Length; ++index)
        {
            Object attachment = __instance.attachments[index];
            __instance.attachments[index] = (Object) null;
            if (o.ItemId == attachment.ItemId)
            {
                __instance.attachments[index] = o;
                Game1.playSound("button1");
                return attachment;
            }
            __instance.attachments[index] = attachment;
        }
        
        return o;
    }
}