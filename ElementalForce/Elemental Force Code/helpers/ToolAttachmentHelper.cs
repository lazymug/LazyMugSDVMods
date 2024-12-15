using StardewValley;

namespace ElementalForce.Elemental_Force_Code.helpers
{
    public static class ToolAttachmentHelper
    {
        public static bool IsIfritEssenceEquipped()
        {
            return IsModEssenceEquipped(ElementalEssenceEnum.Ifrit);
        }
        
        public static bool IsShivaEssenceEquipped()
        {
            return IsModEssenceEquipped(ElementalEssenceEnum.Shiva);
        }
        
        public static bool IsTitanEssenceEquipped()
        {
            return IsModEssenceEquipped(ElementalEssenceEnum.Titan);
        }
        
        public static bool IsLeviathanEssenceEquipped()
        {
            return IsModEssenceEquipped(ElementalEssenceEnum.Leviathan);
        }

        public static bool IsLeviathanShardEquipped()
        {
            return true;
        }
        
        public static bool IsCarbuncleEssenceEquipped()
        {
            return IsModEssenceEquipped(ElementalEssenceEnum.Carbuncle);
        }
        
        public static bool IsKirinEssenceEquipped()
        {
            return IsModEssenceEquipped(ElementalEssenceEnum.Kirin);
        }
        
        public static bool IsRamuhEssenceEquipped()
        {
            return IsModEssenceEquipped(ElementalEssenceEnum.Ramuh);
        }
        
        public static bool IsPhoenixEssenceEquipped()
        {
            return IsModEssenceEquipped(ElementalEssenceEnum.Phoenix);
        }
        
        private static bool IsModEssenceEquipped(ElementalEssenceEnum type)
        {
            foreach (var item in Game1.player.Items)
            {
                if (item is Tool tool && ItemHelper.IsAmphoraTool(tool.ItemId))
                {
                    for (var index = 0; index < tool.attachments.Length; ++index)
                    {
                        var attachment = tool.attachments[index];
                        if (attachment != null)
                        {
                            var hasFind = false;
                            switch (type)
                            {
                                case ElementalEssenceEnum.Ifrit: 
                                    hasFind = ItemHelper.IsIfritElementalEssenceItem(attachment.ItemId);
                                    break;
                                case ElementalEssenceEnum.Shiva: 
                                    hasFind = ItemHelper.IsShivaElementalEssenceItem(attachment.ItemId);
                                    break;
                                case ElementalEssenceEnum.Titan: 
                                    hasFind = ItemHelper.IsTitanElementalEssenceItem(attachment.ItemId);
                                    break;
                                case ElementalEssenceEnum.Leviathan: 
                                    hasFind = ItemHelper.IsLeviathanElementalEssenceItem(attachment.ItemId);
                                    break;
                                case ElementalEssenceEnum.Carbuncle:
                                    hasFind = ItemHelper.IsCarbuncleElementalEssenceItem(attachment.ItemId);
                                    break;
                                case ElementalEssenceEnum.Kirin:
                                    hasFind = ItemHelper.IsKirinElementalEssenceItem(attachment.ItemId);
                                    break;
                                case ElementalEssenceEnum.Ramuh: 
                                    hasFind = ItemHelper.IsRamuhElementalEssenceItem(attachment.ItemId);
                                    break;
                                case ElementalEssenceEnum.Phoenix: 
                                    hasFind = ItemHelper.IsPhoenixElementalEssenceItem(attachment.ItemId);
                                    break;
                                default: continue;
                            }
                            if (hasFind) return true;
                        }
                    }
                    return false;
                }
            }

            return false;
        }
    }
}