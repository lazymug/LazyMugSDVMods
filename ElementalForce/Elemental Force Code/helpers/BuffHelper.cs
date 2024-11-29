using Microsoft.Xna.Framework.Graphics;

namespace ElementalForce.Elemental_Force_Code.helpers
{
    public static class BuffHelper
    {
        private static readonly string BuffBlizzardSlashId = $"{ModEntry.Instance.GetModId()}_BlizzardSlashBuff";
        private static readonly string BuffCompanionProtectionId = $"{ModEntry.Instance.GetModId()}_CompanionProtectionBuff";
        private static readonly string BuffDragonScaleId = $"{ModEntry.Instance.GetModId()}_DragonScaleBuff";
        private static readonly string BuffExplosionId = $"{ModEntry.Instance.GetModId()}_ExplosionBuff";
        private static readonly string BuffEndlessStaminaId = $"{ModEntry.Instance.GetModId()}_EndlessStaminaBuff";
        private static readonly string BuffFlashSpeedId = $"{ModEntry.Instance.GetModId()}_FlashSpeedBuff";
        private static readonly string BuffFireballId = $"{ModEntry.Instance.GetModId()}_FireballBuff";
        private static readonly string BuffHealingAuraId = $"{ModEntry.Instance.GetModId()}_HealingAuraBuff";
        private static readonly string BuffHeatSpeedId = $"{ModEntry.Instance.GetModId()}_HeatSpeedBuff";
        private static readonly string BuffHeavyBodyId = $"{ModEntry.Instance.GetModId()}_HeavyBodyBuff";
        private static readonly string BuffIceTombId = $"{ModEntry.Instance.GetModId()}_IceTombBuff";
        private static readonly string BuffImmunityBandId = $"{ModEntry.Instance.GetModId()}_ImmunityBandBuff";
        private static readonly string BuffIronBodyId = $"{ModEntry.Instance.GetModId()}_IronBodyBuff";
        private static readonly string BuffJoltingSwingId = $"{ModEntry.Instance.GetModId()}_JoltingSwingBuff";
        private static readonly string BuffLuckDayId = $"{ModEntry.Instance.GetModId()}_LuckDayBuff";
        private static readonly string BuffMirrorReflectionId = $"{ModEntry.Instance.GetModId()}_MirrorReflectionBuff";
        private static readonly string BuffPhoenixDownId = $"{ModEntry.Instance.GetModId()}_MirrorPhoenixDownBuff";
        private static readonly string BuffRegenBlessingId = $"{ModEntry.Instance.GetModId()}_MirrorRegenBlessingBuff";
        private static readonly string BuffRainWishId = $"{ModEntry.Instance.GetModId()}_MirrorRainWishBuff";
        private static readonly string BuffSavageIfritId = $"{ModEntry.Instance.GetModId()}_SavageIfritBuff";
        private static readonly string BuffSnowSpeedId = $"{ModEntry.Instance.GetModId()}_SnowSpeedBuff";
        private static readonly string BuffSunnySpeedId = $"{ModEntry.Instance.GetModId()}_SunnySpeedBuff";
        private static readonly string BuffThunderCallerId = $"{ModEntry.Instance.GetModId()}_ThunderCallerBuff";
        private static readonly string BuffWrathPalmId = $"{ModEntry.Instance.GetModId()}_WrathPalmBuff";
        
        private const string Filepath = "assets/buffs/";
        private const string Filename = "BuffIcons.png";
        private const string AssetPath = $"{Filepath}{Filename}";
        private static readonly Texture2D IconTexture = ModEntry.Instance.Helper.GameContent.Load<Texture2D>(AssetPath);
        
        public static string GetBuffBlizzardSlashId() => BuffBlizzardSlashId;
        
        public static string GetBuffCompanionProtectionId() => BuffCompanionProtectionId;
        
        public static string GetBuffDragonScaleId() => BuffDragonScaleId;

        public static string GetBuffExplosionId() => BuffExplosionId;
        
        public static string GetBuffEndlessStaminaId() => BuffEndlessStaminaId;
        
        public static string GetBuffFlashSpeedId() => BuffFlashSpeedId;
        
        public static string GetBuffFireballId() => BuffFireballId;
        
        public static string GetBuffHealingAuraId() => BuffHealingAuraId;
        
        public static string GetBuffHeatSpeedId() => BuffHeatSpeedId;
        
        public static string GetBuffHeavyBodyId() => BuffHeavyBodyId;
        
        public static string GetBuffIceTombId() => BuffIceTombId;
        
        public static string GetBuffImmunityBandId() => BuffImmunityBandId;
        
        public static string GetBuffIronBodyId() => BuffIronBodyId;
        
        public static string GetBuffJoltingSwingId() => BuffJoltingSwingId;
        
        public static string GetBuffLuckDayId() => BuffLuckDayId;
        
        public static string GetBuffMirrorReflectionId() => BuffMirrorReflectionId;
        
        public static string GetBuffPhoenixDownId() => BuffPhoenixDownId;
        
        public static string GetBuffRegenBlessingId() => BuffRegenBlessingId;
        
        public static string GetBuffRainWishId() => BuffRainWishId;
        
        public static string GetBuffSavageIfritId() => BuffSavageIfritId;
        
        public static string GetBuffSnowSpeedId() => BuffSnowSpeedId;
        
        public static string GetBuffSunnySpeedId() => BuffSunnySpeedId;
        
        public static string GetBuffThunderCallerId() => BuffThunderCallerId;
        
        public static string GetBuffWrathPalmId() => BuffWrathPalmId;
        
        public static Texture2D GetIconTexture()
        {
            return IconTexture;
        }
    }
}