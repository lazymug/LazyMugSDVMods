using ElementalForce.Elemental_Force_Code.helpers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Objects.Trinkets;
using StardewValley.Tools;
using Object = System.Object;

namespace ElementalForce.Elemental_Force_Code.harmony;

[HarmonyPatch(typeof(Farmer))]
public class FarmerPatcher
{
    [HarmonyPatch(nameof(Farmer.CanBeDamaged))]
    [HarmonyPostfix]
    public static void Postfix_CanBeDamaged(ref bool __result, Farmer __instance)
    {
        if (__instance.hasBuff(BuffHelper.GetBuffCompanionProtectionId())) // Companion Protection Buff
        {
            var random = new Random();
            var chance = random.Next(0, 100);
            __result = chance < 35;
        }
    }
    
    [HarmonyPatch(nameof(Farmer.takeDamage))]
    [HarmonyPatch(new []{ typeof(int), typeof(bool), typeof(Monster) })]
    [HarmonyPrefix]
    public static bool Prefix_takeDamage(Farmer __instance, int damage, bool overrideParry, Monster damager)
    {
        if (!__instance.hasBuff(BuffHelper.GetBuffMirrorReflectionId()))
        {
            return true;
        }
        CustomTakeDamage(__instance, damage, overrideParry, damager);
        return false;
    }
    
    private static void CustomTakeDamage(Farmer __instance, int damage, bool overrideParry, Monster damager)
    {
        if (Game1.eventUp || __instance.IsDedicatedPlayer || __instance.FarmerSprite.isPassingOut() || __instance.isInBed.Value && Game1.activeClickableMenu != null && Game1.activeClickableMenu is ReadyCheckDialog)
            return;
        int num1 = damager == null || damager.isInvincible() ? 0 : (!overrideParry ? 1 : 0);
        bool flag1 = (damager == null || !damager.isInvincible()) && (damager == null || !(damager is GreenSlime) && !(damager is BigSlime) || !__instance.isWearingRing("520"));
        bool flag2 = __instance.CurrentTool is MeleeWeapon && ((MeleeWeapon) __instance.CurrentTool).isOnSpecial && ((MeleeWeapon) __instance.CurrentTool).type.Value == 3;
        bool flag3 = __instance.CanBeDamaged();
        int num2 = flag2 ? 1 : 0;
        if ((num1 & num2) != 0)
        {
            Rumble.rumble(0.75f, 150f);
            __instance.playNearbySoundAll("parry");
            damager.parried(damage, __instance);
        }
        else
        {
            if (!(flag1 & flag3))
                return;
            damager?.onDealContactDamage(__instance);
            damage += Game1.random.Next(Math.Min(-1, -damage / 8), Math.Max(1, damage / 8));
            int defense = __instance.buffs.Defense;
            if (__instance.stats.Get("Book_Defense") > 0U)
                ++defense;
            if ((double) defense >= (double) damage * 0.5)
                defense -= (int) ((double) defense * (double) Game1.random.Next(3) / 10.0);
            if (damager != null)
            {
                Microsoft.Xna.Framework.Rectangle boundingBox = damager.GetBoundingBox();
                Vector2 vector2 = Utility.getAwayFromPlayerTrajectory(boundingBox, __instance) / 2f;
                int num3 = damage;
                int num4 = Math.Max(1, damage - defense);
                if (num4 < 10)
                    num3 = (int) Math.Ceiling((double) (num3 + num4) / 2.0);
                int ofWornRingsWithId = __instance.getNumberOfWornRingsWithID("839") + 1; // increment for the mirror reflection damage
                int minDamage = num3 * ofWornRingsWithId;
                __instance.currentLocation?.damageMonster(boundingBox, minDamage, minDamage + 1, false, __instance);
            }
            if (__instance.isWearingRing("524") && !__instance.hasBuff("21") && Game1.random.NextDouble() < (0.9 - (double) __instance.health / 100.0) / (double) (3 - __instance.LuckLevel / 10) + (__instance.health <= 15 ? 0.2 : 0.0))
            {
                __instance.playNearbySoundAll("yoba");
                __instance.applyBuff("21");
            }
            else
            {
                Rumble.rumble(0.75f, 150f);
                damage = Math.Max(1, damage - defense);
                if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && __instance.currentLocation is MineShaft && Game1.mine.getMineArea() == 121)
                {
                    float num5 = 1f;
                    int num6;
                    if (__instance.team.calicoStatueEffects.TryGetValue(8, out num6))
                        num5 += (float) num6 * 0.25f;
                    int num7;
                    if (__instance.team.calicoStatueEffects.TryGetValue(14, out num7))
                        num5 -= (float) num7 * 0.25f;
                    damage = Math.Max(1, (int) ((double) damage * (double) num5));
                }
                __instance.health = Math.Max(0, __instance.health - damage);
                foreach (Trinket trinketItem in __instance.trinketItems)
                    trinketItem?.OnReceiveDamage(__instance, damage);
                if (__instance.health <= 0 && __instance.GetEffectsOfRingMultiplier("863") > 0 && !__instance.hasUsedDailyRevive.Value)
                {
                    __instance.startGlowing(new Color((int) byte.MaxValue, (int) byte.MaxValue, 0), false, 0.25f);
                    DelayedAction.functionAfterDelay(new Action(((Character) __instance).stopGlowing), 500);
                    Game1.playSound("yoba");
                    for (int index = 0; index < 13; ++index)
                    {
                        float num8 = (float) Game1.random.Next(-32, 33);
                        __instance.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(114, 46, 2, 2), 200f, 5, 1, new Vector2(num8 + 32f, -96f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                        {
                            attachedCharacter = (Character) __instance,
                            positionFollowsAttachedCharacter = true,
                            motion = new Vector2(num8 / 32f, -3f),
                            delayBeforeAnimationStart = index * 50,
                            alphaFade = 1f / 1000f,
                            acceleration = new Vector2(0.0f, 0.1f)
                        });
                    }
                    __instance.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(157, 280, 28, 19), 2000f, 1, 1, new Vector2(-20f, -16f), false, false, 1E-06f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                    {
                        attachedCharacter = (Character) __instance,
                        positionFollowsAttachedCharacter = true,
                        alpha = 0.1f,
                        alphaFade = -0.01f,
                        alphaFadeFade = -0.00025f
                    });
                    __instance.health = (int) Math.Min((float) __instance.maxHealth, (float) __instance.maxHealth * 0.5f + (float) __instance.GetEffectsOfRingMultiplier("863"));
                    __instance.hasUsedDailyRevive.Value = true;
                }
                __instance.temporarilyInvincible = true;
                __instance.flashDuringThisTemporaryInvincibility = true;
                __instance.temporaryInvincibilityTimer = 0;
                __instance.currentTemporaryInvincibilityDuration = 1200 + __instance.GetEffectsOfRingMultiplier("861") * 400;
                Point standingPixel = __instance.StandingPixel;
                __instance.currentLocation.debris.Add(new Debris(damage, new Vector2((float) (standingPixel.X + 8), (float) standingPixel.Y), Color.Red, 1f, (Character) __instance));
                __instance.playNearbySoundAll("ow");
                Game1.hitShakeTimer = 100 * damage;
            }
        }
    }
}