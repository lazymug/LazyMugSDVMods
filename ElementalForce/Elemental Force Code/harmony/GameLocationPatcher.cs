using ElementalForce.Elemental_Force_Code.helpers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;

namespace ElementalForce.Elemental_Force_Code.harmony;

[HarmonyPatch(typeof(GameLocation))]
public class GameLocationPatcher
{
    private static int FreezeTime = 6000;
    
    [HarmonyPatch(nameof(GameLocation.damageMonster))]
    [HarmonyPatch(new [] { typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer), typeof(bool) } )]
    [HarmonyPostfix]
    public static void Postfix(
        ref bool __result,
        GameLocation __instance,
        Microsoft.Xna.Framework.Rectangle areaOfEffect,
        int minDamage,
        int maxDamage,
        bool isBomb,
        float knockBackModifier,
        int addedPrecision,
        float critChance,
        float critMultiplier,
        bool triggerMonsterInvincibleTimer,
        Farmer who,
        bool isProjectile = false)
    {
        if (__result) // Damage was applied
        {
            if (who.hasBuff(BuffHelper.GetBuffIceTombId()))
            {
                for (int index = __instance.characters.Count - 1; index >= 0; --index)
                {
                    if (index < __instance.characters.Count && __instance.characters[index] is Monster character && character.IsMonster && character.Health > 0 && character.TakesDamageFromHitbox(areaOfEffect))
                    {
                        var chance = new Random().Next(0, 100);
                        if (chance < 45)
                        {
                            FreezeEnemy(__instance, who, character);
                        }
                    }
                }
            }
        }
    }

    private static void FreezeEnemy(GameLocation location, Farmer farmer, Monster monster)
    {
        Vector2 motion = Utility.getVelocityTowardPoint(farmer.getStandingPosition(), monster.getStandingPosition(), 10f);
        DebuffingProjectile p = new DebuffingProjectile("frozen", 17, 0, 0, 0f, motion.X, motion.Y, farmer.getStandingPosition() - new Vector2(32f, 48f), location, farmer, hitsMonsters: true, playDefaultSoundOnFire: false);
        p.wavyMotion.Value = false;
        p.piercesLeft.Value = 99999;
        p.maxTravelDistance.Value = 32;
        p.IgnoreLocationCollision = true;
        p.ignoreObjectCollisions.Value = false;
        p.maxVelocity.Value = 12f;
        p.projectileID.Value = 15;
        p.alpha.Value = 0.001f;
        p.alphaChange.Value = 0.05f;
        p.light.Value = true;
        p.debuffIntensity.Value = FreezeTime;
        p.boundingBoxWidth.Value = 32;
        location.projectiles.Add(p);
        location.playSound("fireball");
        monster.stunTime.Value = 4000;
    }
}