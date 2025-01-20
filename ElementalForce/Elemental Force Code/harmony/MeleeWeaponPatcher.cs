using ElementalForce.Elemental_Force_Code.helpers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.Weapons;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

namespace ElementalForce.Elemental_Force_Code.harmony;

[HarmonyPatch(typeof(MeleeWeapon))]
public class MeleeWeaponPatcher
{

    private static bool CanCastFireball = true;
    
    [HarmonyPatch(nameof(MeleeWeapon.leftClick))]
    [HarmonyPatch(new []{ typeof(Farmer) })]
    [HarmonyPrefix]
    public static void Prefix_LeftClick(ref MeleeWeapon __instance, Farmer who)
    {
        if (who.hasBuff(BuffHelper.GetBuffFireballId()) && who.CanMove)
        {
            CanCastFireball = true;
        }
    }
    
    [HarmonyPatch(nameof(MeleeWeapon.leftClick))]
    [HarmonyPatch(new []{ typeof(Farmer) })]
    [HarmonyPostfix]
    public static void Postfix_LeftClick(ref MeleeWeapon __instance, Farmer who)
    {
        if (who.hasBuff(BuffHelper.GetBuffFireballId()) && CanCastFireball)
        {
            // Get nearest monster
            Monster monster = Utility.findClosestMonsterWithinRange(who.currentLocation, who.getStandingPosition(), 640);
            var random = new Random();
            var chance = random.Next(0, 100);
            if (chance > 60 && monster != null)
            {
                // Call create fireball
                who.currentLocation.projectiles.Add(CreateFireball(who, monster));
            }
            CanCastFireball = false;
        }

        if (who.hasBuff(BuffHelper.GetBuffExplosionId()))
        {
            Monster monster = Utility.findClosestMonsterWithinRange(who.currentLocation, who.getStandingPosition(), 32);
            GameLocation location = who.currentLocation;
            location?.explode(monster.Tile, 2, who, damageFarmers: false, 20, !(location is Farm) && !(location is SlimeHutch));
        }
    }

    private static Projectile CreateFireball(Farmer who, Monster nearestMonster)
    {
        Vector2 standingPixel = who.getStandingPosition();
        Vector2 motion = Utility.getVelocityTowardPoint(standingPixel, nearestMonster.getStandingPosition(), 12.0f); //todo: check if speed is okay
        BasicProjectile fireball = new BasicProjectile(30, 10, 0, 2, 0f, motion.X, motion.Y, standingPixel - new Vector2(32f, 0f), "flameSpellHit", null, "flameSpell", explode: true, damagesMonsters: true, who.currentLocation, who)
            {
                collisionBehavior = GetFireballCollisionBehavior(),
                ignoreCharacterCollisions =
                {
                    Value = true
                },
                maxTravelDistance =
                {
                    Value = 960
                },
                maxVelocity =
                {
                    Value = 16.0f
                },
                height =
                {
                    Value = 32.0f
                }
            };
        return fireball;
    }

    private static BasicProjectile.onCollisionBehavior GetFireballCollisionBehavior() =>
        BasicProjectile.explodeOnImpact;
}