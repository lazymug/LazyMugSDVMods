namespace ElementalForce.Elemental_Force_Code.helpers
{
    public static class ItemHelper
    {
        
        private static readonly string ObjectEssenceTitanId = $"{ModEntry.Instance.GetModId()}.CP_TitanEssence";
        
        private static readonly string ToolCrucibleId = $"{ModEntry.Instance.GetModId()}.CP_Crucible";
        
        public static string GetObjectEssenceTitanId() => ObjectEssenceTitanId;
        
        public static string GetToolCrucibleId() => ToolCrucibleId;
        
        public static bool IsCrucibleTool(string itemId)
        {
            return itemId.StartsWith($"{ModEntry.Instance.GetModId()}") &&
                   itemId.EndsWith("Crucible");
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