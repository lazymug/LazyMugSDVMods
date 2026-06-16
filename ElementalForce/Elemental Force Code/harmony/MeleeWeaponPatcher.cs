using ElementalForce.Elemental_Force_Code.helpers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
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
            Monster monster = Utility.findClosestMonsterWithinRange(who.currentLocation, who.getStandingPosition(), BuffConstants.FireballDetectionRange);
            var chance = Game1.random.Next(0, 100);
            if (chance > ModEntry.Instance.Config.FireballChancePercent && monster != null)
            {
                who.currentLocation.projectiles.Add(CreateFireball(who, monster));
            }
            CanCastFireball = false;
        }

        if (who.hasBuff(BuffHelper.GetBuffExplosionId()))
        {
            Monster monster = Utility.findClosestMonsterWithinRange(who.currentLocation, who.getStandingPosition(), BuffConstants.ExplosionDetectionRange);
            if (monster != null)
            {
                GameLocation location = who.currentLocation;
                location?.explode(monster.Tile, BuffConstants.ExplosionRadius, who, damageFarmers: false, ModEntry.Instance.Config.ExplosionDamage, !(location is Farm) && !(location is SlimeHutch));
            }
        }
    }

    private static Projectile CreateFireball(Farmer who, Monster nearestMonster)
    {
        Vector2 standingPixel = who.getStandingPosition();
        Vector2 motion = Utility.getVelocityTowardPoint(standingPixel, nearestMonster.getStandingPosition(), BuffConstants.FireballSpeed);
        BasicProjectile fireball = new BasicProjectile(ModEntry.Instance.Config.FireballDamage, 10, 0, 2, 0f, motion.X, motion.Y, standingPixel - new Vector2(32f, 0f), "flameSpellHit", null, "flameSpell", explode: true, damagesMonsters: true, who.currentLocation, who)
            {
                collisionBehavior = BasicProjectile.explodeOnImpact,
                ignoreCharacterCollisions =
                {
                    Value = true
                },
                maxTravelDistance =
                {
                    Value = BuffConstants.FireballMaxTravelDistance
                },
                maxVelocity =
                {
                    Value = BuffConstants.FireballMaxVelocity
                },
                height =
                {
                    Value = BuffConstants.FireballHeight
                }
            };
        return fireball;
    }
}
