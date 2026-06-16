namespace ElementalForce.Elemental_Force_Code.helpers
{
    public static class BuffConstants
    {
        // Phoenix Healing Aura
        public const float HealingAuraHealthThreshold = 0.3f;
        public const float HealingAuraStaminaThreshold = 0.2f;
        public const float HealingAuraRecoveryRate = 0.4f;

        // Phoenix Down
        public const float PhoenixDownHealthRecovery = 0.3f;
        public const float PhoenixDownStaminaRecovery = 0.2f;

        // Regen Blessing (Kirin Soul)
        public const float RegenBlessingRate = 0.015f;
        public const int RegenBlessingTickMs = 7000;
        public const int RegenIntervalDefaultMs = 42000;
        public const int RegenIntervalSkullCavernMs = 27000;

        // Fireball (Ifrit Soul)
        public const int FireballChancePercent = 60;
        public const int FireballDetectionRange = 640;
        public const int FireballDamage = 30;
        public const int FireballMaxTravelDistance = 960;
        public const float FireballSpeed = 12.0f;
        public const float FireballMaxVelocity = 16.0f;
        public const float FireballHeight = 32.0f;

        // Companion Protection (Carbuncle Shard)
        public const int CompanionProtectionChancePercent = 35;

        // Wary Speed (Cactuar Essence)
        public const int WarySpeedDurationMs = 5000;

        // Ice Tomb (Shiva Soul)
        public const int IceTombStunDurationMs = 3000;
        public const int IceTombDetectionRange = 128;

        // Explosion (Phoenix Shard)
        public const int ExplosionDetectionRange = 32;
        public const int ExplosionRadius = 2;
        public const int ExplosionDamage = 20;
    }
}
