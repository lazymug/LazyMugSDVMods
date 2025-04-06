namespace ElementalForce.Elemental_Force_Code.helpers
{
    public static class ItemHelper
    {

        private static readonly string ObjectEssenceCarbuncleId = $"{ModEntry.Instance.GetModId()}.CP_CarbuncleEssence";
        private static readonly string ObjectEssenceIfritId = $"{ModEntry.Instance.GetModId()}.CP_IfritEssence";
        private static readonly string ObjectEssenceKirinId = $"{ModEntry.Instance.GetModId()}.CP_KirinEssence";
        private static readonly string ObjectEssenceLeviathanId = $"{ModEntry.Instance.GetModId()}.CP_LeviathanEssence";
        private static readonly string ObjectEssencePhoenixId = $"{ModEntry.Instance.GetModId()}.CP_PhoenixEssence";
        private static readonly string ObjectEssenceRamuhId = $"{ModEntry.Instance.GetModId()}.CP_RamuhEssence";
        private static readonly string ObjectEssenceShivaId = $"{ModEntry.Instance.GetModId()}.CP_ShivaEssence";
        private static readonly string ObjectEssenceTitanId = $"{ModEntry.Instance.GetModId()}.CP_TitanEssence";

        private static readonly string ObjectShardCarbuncleId = $"{ModEntry.Instance.GetModId()}.CP_CarbuncleShard";
        private static readonly string ObjectShardIfritId = $"{ModEntry.Instance.GetModId()}.CP_IfritShard";
        private static readonly string ObjectShardKirinId = $"{ModEntry.Instance.GetModId()}.CP_KirinShard";
        private static readonly string ObjectShardLeviathanId = $"{ModEntry.Instance.GetModId()}.CP_LeviathanShard";
        private static readonly string ObjectShardPhoenixId = $"{ModEntry.Instance.GetModId()}.CP_PhoenixShard";
        private static readonly string ObjectShardRamuhId = $"{ModEntry.Instance.GetModId()}.CP_RamuhShard";
        private static readonly string ObjectShardShivaId = $"{ModEntry.Instance.GetModId()}.CP_ShivaShard";
        private static readonly string ObjectShardTitanId = $"{ModEntry.Instance.GetModId()}.CP_TitanShard";
        
        private static readonly string ObjectSoulCarbuncleId = $"{ModEntry.Instance.GetModId()}.CP_CarbuncleSoul";
        private static readonly string ObjectSoulIfritId = $"{ModEntry.Instance.GetModId()}.CP_IfritSoul";
        private static readonly string ObjectSoulKirinId = $"{ModEntry.Instance.GetModId()}.CP_KirinSoul";
        private static readonly string ObjectSoulLeviathanId = $"{ModEntry.Instance.GetModId()}.CP_LeviathanSoul";
        private static readonly string ObjectSoulPhoenixId = $"{ModEntry.Instance.GetModId()}.CP_PhoenixSoul";
        private static readonly string ObjectSoulRamuhId = $"{ModEntry.Instance.GetModId()}.CP_RamuhSoul";
        private static readonly string ObjectSoulShivaId = $"{ModEntry.Instance.GetModId()}.CP_ShivaSoul";
        private static readonly string ObjectSoulTitanId = $"{ModEntry.Instance.GetModId()}.CP_TitanSoul";
        
        private static readonly string ToolAmphoraId = $"{ModEntry.Instance.GetModId()}.CP_Amphora";
        private static readonly string ToolAmphoraEchoesId = $"{ModEntry.Instance.GetModId()}.CP_Amphora_Echoes";
        private static readonly string ToolAmphoraSpiritsId = $"{ModEntry.Instance.GetModId()}.CP_Amphora_Spirits";
        
        public static string GetObjectEssenceCarbuncleId() => ObjectEssenceCarbuncleId;
        
        public static string GetObjectEssenceKirinId() => ObjectEssenceKirinId;
        
        public static string GetObjectEssenceIfritId() => ObjectEssenceIfritId;
        
        public static string GetObjectEssenceLeviathanId() => ObjectEssenceLeviathanId;
        
        public static string GetObjectEssencePhoenixId() => ObjectEssencePhoenixId;
        
        public static string GetObjectEssenceRamuhId() => ObjectEssenceRamuhId;
        
        public static string GetObjectEssenceShivaId() => ObjectEssenceShivaId;
        
        public static string GetObjectEssenceTitanId() => ObjectEssenceTitanId;
        
        public static string GetObjectShardCarbuncleId() => ObjectShardCarbuncleId;
        
        public static string GetObjectShardIfritId() => ObjectShardIfritId;
        
        public static string GetObjectShardKirinId() => ObjectShardKirinId;
        
        public static string GetObjectShardLeviathanId() => ObjectShardLeviathanId;
        
        public static string GetObjectShardPhoenixId() => ObjectShardPhoenixId;
        
        public static string GetObjectShardRamuhId() => ObjectShardRamuhId;
        
        public static string GetObjectShardShivaId() => ObjectShardShivaId;
        
        public static string GetObjectShardTitanId() => ObjectShardTitanId;
        
        public static string GetObjectSoulCarbuncleId() => ObjectSoulCarbuncleId;
        
        public static string GetObjectSoulIfritId() => ObjectSoulIfritId;
        
        public static string GetObjectSoulKirinId() => ObjectSoulKirinId;
        
        public static string GetObjectSoulLeviathanId() => ObjectSoulLeviathanId;
        
        public static string GetObjectSoulPhoenixId() => ObjectSoulPhoenixId;
        
        public static string GetObjectSoulRamuhId() => ObjectSoulRamuhId;
        
        public static string GetObjectSoulShivaId() => ObjectSoulShivaId;
        
        public static string GetObjectSoulTitanId() => ObjectSoulTitanId;
        
        public static string GetToolAmphoraId() => ToolAmphoraId;
        public static string GetToolAmphoraEchoesId() => ToolAmphoraEchoesId;
        public static string GetToolAmphoraSpiritsId() => ToolAmphoraSpiritsId;
        
        public static bool IsAmphoraTool(string itemId)
        {
            return itemId.StartsWith($"{ModEntry.Instance.GetModId()}") &&
                   itemId.EndsWith("Amphora");
        }
        
        public static bool IsAmphoraLevel2Tool(string itemId) =>
            itemId.StartsWith($"{ModEntry.Instance.GetModId()}") &&
            itemId.EndsWith("Amphora_Echoes");
        
        public static bool IsAmphoraLevel3Tool(string itemId) =>
            itemId.StartsWith($"{ModEntry.Instance.GetModId()}") &&
            itemId.EndsWith("Amphora_Spirits");

        public static bool IsElementalEssenceItem(string itemId)
        {
            return itemId.StartsWith($"{ModEntry.Instance.GetModId()}") &&
                   itemId.EndsWith("Essence");
        }
        
        public static bool IsElementalShardItem(string itemId) =>
            itemId.StartsWith($"{ModEntry.Instance.GetModId()}") &&
            itemId.EndsWith("Shard");
        
        public static bool IsElementalSoulItem(string itemId) =>
            itemId.StartsWith($"{ModEntry.Instance.GetModId()}") &&
            itemId.EndsWith("Soul");

        public static bool IsIfritElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEnum.Ifrit);
        }

        public static bool IsIfritElementalShardItem(string itemId) =>
            IsAnyElementalShardItem(itemId, ElementalEnum.Ifrit);
        
        public static bool IsIfritElementalSoulItem(string itemId) =>
            IsAnyElementalSoulItem(itemId, ElementalEnum.Ifrit);
        
        public static bool IsShivaElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEnum.Shiva);
        }
        
        public static bool IsShivaElementalShardItem(string itemId) =>
            IsAnyElementalShardItem(itemId, ElementalEnum.Shiva);
        
        public static bool IsShivaElementalSoulItem(string itemId) =>
            IsAnyElementalSoulItem(itemId, ElementalEnum.Shiva);
        
        public static bool IsTitanElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEnum.Titan);
        }
        
        public static bool IsTitanElementalShardItem(string itemId) =>
            IsAnyElementalShardItem(itemId, ElementalEnum.Titan);
        
        public static bool IsTitanElementalSoulItem(string itemId) =>
            IsAnyElementalSoulItem(itemId, ElementalEnum.Titan);
        
        public static bool IsLeviathanElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEnum.Leviathan);
        }
        
        public static bool IsLeviathanElementalShardItem(string itemId) =>
            IsAnyElementalShardItem(itemId, ElementalEnum.Leviathan);
        
        public static bool IsLeviathanElementalSoulItem(string itemId) =>
            IsAnyElementalSoulItem(itemId, ElementalEnum.Leviathan);
        
        public static bool IsCarbuncleElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEnum.Carbuncle);
        }
        
        public static bool IsCarbuncleElementalShardItem(string itemId) =>
            IsAnyElementalShardItem(itemId, ElementalEnum.Carbuncle);
        
        public static bool IsCarbuncleElementalSoulItem(string itemId) =>
            IsAnyElementalSoulItem(itemId, ElementalEnum.Carbuncle);
        
        public static bool IsKirinElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEnum.Kirin);
        }
        
        public static bool IsKirinElementalShardItem(string itemId) =>
            IsAnyElementalShardItem(itemId, ElementalEnum.Kirin);
        
        public static bool IsKirinElementalSoulItem(string itemId) =>
            IsAnyElementalSoulItem(itemId, ElementalEnum.Kirin);
        
        public static bool IsRamuhElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEnum.Ramuh);
        }
        
        public static bool IsRamuhElementalShardItem(string itemId) =>
            IsAnyElementalShardItem(itemId, ElementalEnum.Ramuh);
        
        public static bool IsRamuhElementalSoulItem(string itemId) =>
            IsAnyElementalSoulItem(itemId, ElementalEnum.Ramuh);
        
        public static bool IsPhoenixElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEnum.Phoenix);
        }
        
        public static bool IsPhoenixElementalShardItem(string itemId) =>
            IsAnyElementalShardItem(itemId, ElementalEnum.Phoenix);
        
        public static bool IsPhoenixElementalSoulItem(string itemId) =>
            IsAnyElementalSoulItem(itemId, ElementalEnum.Phoenix);
        
        public static bool IsCactuarElementalEssenceItem(string itemId) =>
            IsAnyElementalEssenceItem(itemId, ElementalEnum.Cactuar);
        
        public static bool IsCactuarElementalShardItem(string itemId) =>
            IsAnyElementalShardItem(itemId, ElementalEnum.Cactuar);
        
        public static bool IsCactuarElementalSoulItem(string itemId) =>
            IsAnyElementalSoulItem(itemId, ElementalEnum.Cactuar);

        private static bool IsAnyElementalEssenceItem(string itemId, ElementalEnum type)
        {
            return $"{ModEntry.Instance.GetModId()}.CP_{Enum.GetName(typeof(ElementalEnum), type)}Essence".Equals(itemId);
        }
        
        private static bool IsAnyElementalShardItem(string itemId, ElementalEnum type) =>
            $"{ModEntry.Instance.GetModId()}.CP_{Enum.GetName(typeof(ElementalEnum), type)}Shard".Equals(itemId);
        
        private static bool IsAnyElementalSoulItem(string itemId, ElementalEnum type) =>
            $"{ModEntry.Instance.GetModId()}.CP_{Enum.GetName(typeof(ElementalEnum), type)}Soul".Equals(itemId);
    }
}