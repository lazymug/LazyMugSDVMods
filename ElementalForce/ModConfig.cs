using ElementalForce.Elemental_Force_Code.helpers;

namespace ElementalForce
{
    public class ModConfig
    {
        // Fireball (Ifrit Soul)
        public int FireballChancePercent { get; set; } = BuffConstants.FireballChancePercent;
        public int FireballDamage { get; set; } = BuffConstants.FireballDamage;

        // Companion Protection (Carbuncle Shard)
        public int CompanionProtectionChancePercent { get; set; } = BuffConstants.CompanionProtectionChancePercent;

        // Ice Tomb (Shiva Soul)
        public int IceTombFreezeChancePercent { get; set; } = BuffConstants.IceTombFreezeChancePercent;

        // Regen Blessing (Kirin Soul)
        public float RegenBlessingRate { get; set; } = BuffConstants.RegenBlessingRate;

        // Phoenix Healing Aura (Essence)
        public float HealingAuraRecoveryRate { get; set; } = BuffConstants.HealingAuraRecoveryRate;

        // Phoenix Down (Soul)
        public float PhoenixDownHealthRecovery { get; set; } = BuffConstants.PhoenixDownHealthRecovery;
        public float PhoenixDownStaminaRecovery { get; set; } = BuffConstants.PhoenixDownStaminaRecovery;

        // Wary Speed (Cactuar Essence)
        public int WarySpeedDurationMs { get; set; } = BuffConstants.WarySpeedDurationMs;

        // Explosion (Phoenix Shard)
        public int ExplosionDamage { get; set; } = BuffConstants.ExplosionDamage;
    }
}
