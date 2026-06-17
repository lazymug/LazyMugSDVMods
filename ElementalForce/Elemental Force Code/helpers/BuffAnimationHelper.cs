using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

namespace ElementalForce.Elemental_Force_Code.helpers;

public static class BuffAnimationHelper
{
    private const string ParticleSheet = "LooseSprites\\Cursors2";
    private static readonly Rectangle SmallParticle = new(114, 46, 2, 2);

    public static void PlayFireballCast(Farmer farmer)
    {
        var location = farmer.currentLocation;
        var pos = farmer.getStandingPosition();
        for (int i = 0; i < 8; i++)
        {
            float x = Game1.random.Next(-24, 25);
            float y = Game1.random.Next(-64, -16);
            location.temporarySprites.Add(new TemporaryAnimatedSprite(
                ParticleSheet, SmallParticle, 120f, 5, 1,
                pos + new Vector2(x, y),
                false, false, 1f, 0f, Color.OrangeRed, 4f, 0f, 0f, 0f)
            {
                motion = new Vector2(x / 20f, -2.5f),
                alphaFade = 0.015f,
                acceleration = new Vector2(0f, 0.08f),
                delayBeforeAnimationStart = i * 25
            });
        }
    }

    public static void PlayIceTombFreeze(GameLocation location, Monster monster)
    {
        var pos = monster.getStandingPosition();
        for (int i = 0; i < 10; i++)
        {
            float x = Game1.random.Next(-32, 33);
            float y = Game1.random.Next(-48, 8);
            location.temporarySprites.Add(new TemporaryAnimatedSprite(
                ParticleSheet, SmallParticle, 180f, 5, 1,
                pos + new Vector2(x, y),
                false, false, 1f, 0f, Color.LightCyan, 3.5f, 0f, 0f, 0f)
            {
                motion = new Vector2(x / 24f, -1.5f),
                alphaFade = 0.012f,
                acceleration = new Vector2(0f, 0.03f),
                delayBeforeAnimationStart = i * 40
            });
        }

        location.temporarySprites.Add(new TemporaryAnimatedSprite(
            "LooseSprites\\Cursors2", new Rectangle(157, 280, 28, 19), 1500f, 1, 1,
            new Vector2(-20f, -16f),
            false, false, 1E-06f, 0f, Color.LightBlue * 0.6f, 3f, 0f, 0f, 0f)
        {
            attachedCharacter = monster,
            positionFollowsAttachedCharacter = true,
            alpha = 0.1f,
            alphaFade = -0.02f,
            alphaFadeFade = -0.0005f
        });
    }

    public static void PlayCompanionProtectionBlock(Farmer farmer)
    {
        var location = farmer.currentLocation;
        var pos = farmer.getStandingPosition();

        for (int i = 0; i < 6; i++)
        {
            double angle = i * Math.PI / 3.0;
            float x = (float)(Math.Cos(angle) * 24);
            float y = (float)(Math.Sin(angle) * 24) - 32f;
            location.temporarySprites.Add(new TemporaryAnimatedSprite(
                ParticleSheet, SmallParticle, 100f, 5, 1,
                pos + new Vector2(x, y),
                false, false, 1f, 0f, Color.Gold, 5f, 0f, 0f, 0f)
            {
                motion = new Vector2(x / 12f, y / 16f),
                alphaFade = 0.03f,
                delayBeforeAnimationStart = i * 20
            });
        }

        farmer.startGlowing(Color.Gold, false, 0.1f);
        DelayedAction.functionAfterDelay(() => farmer.stopGlowing(), 300);
        location.playSound("yoba");
    }

    public static void PlayMirrorReflection(Farmer farmer, Monster damager)
    {
        var location = farmer.currentLocation;
        var farmerPos = farmer.getStandingPosition();

        for (int i = 0; i < 8; i++)
        {
            float x = Game1.random.Next(-20, 21);
            float y = Game1.random.Next(-48, -8);
            location.temporarySprites.Add(new TemporaryAnimatedSprite(
                ParticleSheet, SmallParticle, 100f, 5, 1,
                farmerPos + new Vector2(x, y),
                false, false, 1f, 0f, Color.MediumPurple, 3.5f, 0f, 0f, 0f)
            {
                motion = new Vector2(x / 10f, -1f),
                alphaFade = 0.02f,
                delayBeforeAnimationStart = i * 15
            });
        }

        if (damager != null)
        {
            var monsterPos = damager.getStandingPosition();
            for (int i = 0; i < 5; i++)
            {
                float x = Game1.random.Next(-16, 17);
                float y = Game1.random.Next(-32, 0);
                location.temporarySprites.Add(new TemporaryAnimatedSprite(
                    ParticleSheet, SmallParticle, 120f, 5, 1,
                    monsterPos + new Vector2(x, y),
                    false, false, 1f, 0f, Color.Silver, 3f, 0f, 0f, 0f)
                {
                    motion = new Vector2(0f, -1.5f),
                    alphaFade = 0.025f,
                    delayBeforeAnimationStart = 100 + i * 30
                });
            }
        }
    }

    public static void PlayWarySpeedActivation(Farmer farmer)
    {
        var location = farmer.currentLocation;
        var pos = farmer.getStandingPosition();

        for (int i = 0; i < 5; i++)
        {
            location.temporarySprites.Add(new TemporaryAnimatedSprite(
                ParticleSheet, SmallParticle, 80f, 5, 1,
                pos + new Vector2(-16 + i * 8, -32f),
                false, false, 1f, 0f, Color.LightGreen, 3f, 0f, 0f, 0f)
            {
                motion = new Vector2(-3f + i * 0.5f, -0.5f),
                alphaFade = 0.025f,
                delayBeforeAnimationStart = i * 40
            });
        }

        farmer.startGlowing(Color.LightGreen, false, 0.08f);
        DelayedAction.functionAfterDelay(() => farmer.stopGlowing(), 400);
    }

    public static void PlayPhoenixDownRevival(Farmer farmer)
    {
        var location = farmer.currentLocation;
        var pos = farmer.getStandingPosition();

        for (int i = 0; i < 16; i++)
        {
            float x = Game1.random.Next(-36, 37);
            float y = Game1.random.Next(-16, 16);
            location.temporarySprites.Add(new TemporaryAnimatedSprite(
                ParticleSheet, SmallParticle, 200f, 5, 1,
                pos + new Vector2(x, y),
                false, false, 1f, 0f, Color.Orange, 4f, 0f, 0f, 0f)
            {
                motion = new Vector2(x / 32f, -3.5f),
                alphaFade = 0.008f,
                acceleration = new Vector2(0f, 0.05f),
                delayBeforeAnimationStart = i * 50
            });
        }

        location.temporarySprites.Add(new TemporaryAnimatedSprite(
            "LooseSprites\\Cursors2", new Rectangle(157, 280, 28, 19), 2500f, 1, 1,
            new Vector2(-20f, -48f),
            false, false, 1E-06f, 0f, Color.Orange * 0.7f, 4f, 0f, 0f, 0f)
        {
            attachedCharacter = farmer,
            positionFollowsAttachedCharacter = true,
            alpha = 0.05f,
            alphaFade = -0.015f,
            alphaFadeFade = -0.0003f
        });

        farmer.startGlowing(Color.Orange, false, 0.15f);
        DelayedAction.functionAfterDelay(() => farmer.stopGlowing(), 800);
        location.playSound("yoba");
    }

    public static void PlayHealingAura(Farmer farmer)
    {
        var location = farmer.currentLocation;
        var pos = farmer.getStandingPosition();

        for (int i = 0; i < 12; i++)
        {
            float x = Game1.random.Next(-32, 33);
            location.temporarySprites.Add(new TemporaryAnimatedSprite(
                ParticleSheet, SmallParticle, 160f, 5, 1,
                pos + new Vector2(x, Game1.random.Next(-8, 16)),
                false, false, 1f, 0f, Color.LimeGreen, 3.5f, 0f, 0f, 0f)
            {
                motion = new Vector2(0f, -2f),
                alphaFade = 0.012f,
                delayBeforeAnimationStart = i * 60
            });
        }

        farmer.startGlowing(Color.LimeGreen, false, 0.1f);
        DelayedAction.functionAfterDelay(() => farmer.stopGlowing(), 500);
    }

    public static void PlayRegenBlessingTick(Farmer farmer)
    {
        var location = farmer.currentLocation;
        var pos = farmer.getStandingPosition();

        for (int i = 0; i < 4; i++)
        {
            float x = Game1.random.Next(-20, 21);
            location.temporarySprites.Add(new TemporaryAnimatedSprite(
                ParticleSheet, SmallParticle, 140f, 5, 1,
                pos + new Vector2(x, Game1.random.Next(-4, 8)),
                false, false, 1f, 0f, Color.LightGreen * 0.8f, 2.5f, 0f, 0f, 0f)
            {
                motion = new Vector2(0f, -1.2f),
                alphaFade = 0.02f,
                delayBeforeAnimationStart = i * 80
            });
        }
    }
}
