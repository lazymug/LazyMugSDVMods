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
        
        private static readonly string ToolAmphoraId = $"{ModEntry.Instance.GetModId()}.CP_Amphora";
        
        public static string GetObjectEssenceCarbuncleId() => ObjectEssenceCarbuncleId;
        
        public static string GetObjectEssenceKirinId() => ObjectEssenceKirinId;
        
        public static string GetObjectEssenceIfritId() => ObjectEssenceIfritId;
        
        public static string GetObjectEssenceLeviathanId() => ObjectEssenceLeviathanId;
        
        public static string GetObjectEssencePhoenixId() => ObjectEssencePhoenixId;
        
        public static string GetObjectEssenceRamuhId() => ObjectEssenceRamuhId;
        
        public static string GetObjectEssenceShivaId() => ObjectEssenceShivaId;
        
        public static string GetObjectEssenceTitanId() => ObjectEssenceTitanId;
        
        public static string GetToolAmphoraId() => ToolAmphoraId;
        
        public static bool IsAmphoraTool(string itemId)
        {
            return itemId.StartsWith($"{ModEntry.Instance.GetModId()}") &&
                   itemId.EndsWith("Amphora");
        }

        public static bool IsElementalEssenceItem(string itemId)
        {
            return itemId.StartsWith($"{ModEntry.Instance.GetModId()}") &&
                   itemId.EndsWith("Essence");
        }

        public static bool IsIfritElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEssenceEnum.Ifrit);
        }
        
        public static bool IsShivaElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEssenceEnum.Shiva);
        }
        
        public static bool IsTitanElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEssenceEnum.Titan);
        }
        
        public static bool IsLeviathanElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEssenceEnum.Leviathan);
        }
        
        public static bool IsCarbuncleElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEssenceEnum.Carbuncle);
        }
        
        public static bool IsKirinElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEssenceEnum.Kirin);
        }
        
        public static bool IsRamuhElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEssenceEnum.Ramuh);
        }
        
        public static bool IsPhoenixElementalEssenceItem(string itemId)
        {
            return IsAnyElementalEssenceItem(itemId, ElementalEssenceEnum.Phoenix);
        }

        private static bool IsAnyElementalEssenceItem(string itemId, ElementalEssenceEnum type)
        {
            return $"{ModEntry.Instance.GetModId()}.CP_{Enum.GetName(typeof(ElementalEssenceEnum), type)}Essence".Equals(itemId);
        }
    }
}