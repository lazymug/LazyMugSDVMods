using StardewValley;

namespace ElementalForce.Elemental_Force_Code.helpers
{
    public static class ToolAttachmentHelper
    {
        public static bool IsIfritEssenceEquipped()
        {
            return IsModItemEquipped(ElementalEnum.Ifrit, ItemEnum.Essence);
        }
        
        public static bool IsIfritShardEquipped() => IsModItemEquipped(ElementalEnum.Ifrit, ItemEnum.Shard);
        
        public static bool IsIfritSoulEquipped() => IsModItemEquipped(ElementalEnum.Ifrit, ItemEnum.Soul);
        
        public static bool IsShivaEssenceEquipped()
        {
            return IsModItemEquipped(ElementalEnum.Shiva, ItemEnum.Essence);
        }
        
        public static bool IsShivaShardEquipped() => IsModItemEquipped(ElementalEnum.Shiva, ItemEnum.Shard);
        
        public static bool IsShivaSoulEquipped() => IsModItemEquipped(ElementalEnum.Shiva, ItemEnum.Soul);
        
        public static bool IsTitanEssenceEquipped()
        {
            return IsModItemEquipped(ElementalEnum.Titan, ItemEnum.Essence);
        }
        
        public static bool IsTitanShardEquipped() => IsModItemEquipped(ElementalEnum.Titan, ItemEnum.Shard);
        
        public static bool IsTitanSoulEquipped() => IsModItemEquipped(ElementalEnum.Titan, ItemEnum.Soul);
        
        public static bool IsLeviathanEssenceEquipped()
        {
            return IsModItemEquipped(ElementalEnum.Leviathan, ItemEnum.Essence);
        }

        public static bool IsLeviathanShardEquipped() => IsModItemEquipped(ElementalEnum.Leviathan, ItemEnum.Shard);
        
        public static bool IsLeviathanSoulEquipped() => IsModItemEquipped(ElementalEnum.Leviathan, ItemEnum.Soul);
        
        public static bool IsCarbuncleEssenceEquipped()
        {
            return IsModItemEquipped(ElementalEnum.Carbuncle, ItemEnum.Essence);
        }
        
        public static bool IsCarbuncleShardEquipped() => IsModItemEquipped(ElementalEnum.Carbuncle, ItemEnum.Shard);
        
        public static bool IsCarbuncleSoulEquipped() => IsModItemEquipped(ElementalEnum.Carbuncle, ItemEnum.Soul);
        
        public static bool IsKirinEssenceEquipped()
        {
            return IsModItemEquipped(ElementalEnum.Kirin, ItemEnum.Essence);
        }
        
        public static bool IsKirinShardEquipped() => IsModItemEquipped(ElementalEnum.Kirin, ItemEnum.Shard);
        
        public static bool IsKirinSoulEquipped() => IsModItemEquipped(ElementalEnum.Kirin, ItemEnum.Soul);
        
        public static bool IsRamuhEssenceEquipped()
        {
            return IsModItemEquipped(ElementalEnum.Ramuh, ItemEnum.Essence);
        }
        
        public static bool IsRamuhShardEquipped() => IsModItemEquipped(ElementalEnum.Ramuh, ItemEnum.Shard);
        
        public static bool IsRamuhSoulEquipped() => IsModItemEquipped(ElementalEnum.Ramuh, ItemEnum.Soul);
        
        public static bool IsPhoenixEssenceEquipped()
        {
            return IsModItemEquipped(ElementalEnum.Phoenix, ItemEnum.Essence);
        }
        
        public static bool IsPhoenixShardEquipped() => IsModItemEquipped(ElementalEnum.Phoenix, ItemEnum.Shard);
        
        public static bool IsPhoenixSoulEquipped() => IsModItemEquipped(ElementalEnum.Phoenix, ItemEnum.Soul);
        
        private static bool IsModItemEquipped(ElementalEnum type, ItemEnum itemEnum)
        {
            foreach (var item in Game1.player.Items)
            {
                if (item is Tool tool && (ItemHelper.IsAmphoraTool(tool.ItemId) || ItemHelper.IsAmphoraLevel2Tool(tool.ItemId) || ItemHelper.IsAmphoraLevel3Tool(tool.ItemId)))
                {
                    for (var index = 0; index < tool.attachments.Length; ++index)
                    {
                        var attachment = tool.attachments[index];
                        if (attachment != null)
                        {
                            var hasFind = false;
                            switch (type)
                            {
                                case ElementalEnum.Ifrit:
                                    if (itemEnum == ItemEnum.Essence)
                                    {
                                        hasFind = ItemHelper.IsIfritElementalEssenceItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Shard)
                                    {
                                        hasFind = ItemHelper.IsIfritElementalShardItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Soul)
                                    {
                                        hasFind = ItemHelper.IsIfritElementalSoulItem(attachment.ItemId);
                                    }
                                    break;
                                case ElementalEnum.Shiva: 
                                    if (itemEnum == ItemEnum.Essence)
                                    {
                                        hasFind = ItemHelper.IsShivaElementalEssenceItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Shard)
                                    {
                                        hasFind = ItemHelper.IsShivaElementalShardItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Soul)
                                    {
                                        hasFind = ItemHelper.IsShivaElementalSoulItem(attachment.ItemId);
                                    }
                                    break;
                                case ElementalEnum.Titan: 
                                    if (itemEnum == ItemEnum.Essence)
                                    {
                                        hasFind = ItemHelper.IsTitanElementalEssenceItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Shard)
                                    {
                                        hasFind = ItemHelper.IsTitanElementalShardItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Soul)
                                    {
                                        hasFind = ItemHelper.IsTitanElementalSoulItem(attachment.ItemId);
                                    }
                                    break;
                                case ElementalEnum.Leviathan: 
                                    if (itemEnum == ItemEnum.Essence)
                                    {
                                        hasFind = ItemHelper.IsLeviathanElementalEssenceItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Shard)
                                    {
                                        hasFind = ItemHelper.IsLeviathanElementalShardItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Soul)
                                    {
                                        hasFind = ItemHelper.IsLeviathanElementalSoulItem(attachment.ItemId);
                                    }
                                    break;
                                case ElementalEnum.Carbuncle:
                                    if (itemEnum == ItemEnum.Essence)
                                    {
                                        hasFind = ItemHelper.IsCarbuncleElementalEssenceItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Shard)
                                    {
                                        hasFind = ItemHelper.IsCarbuncleElementalShardItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Soul)
                                    {
                                        hasFind = ItemHelper.IsCarbuncleElementalSoulItem(attachment.ItemId);
                                    }
                                    break;
                                case ElementalEnum.Kirin:
                                    if (itemEnum == ItemEnum.Essence)
                                    {
                                        hasFind = ItemHelper.IsKirinElementalEssenceItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Shard)
                                    {
                                        hasFind = ItemHelper.IsKirinElementalShardItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Soul)
                                    {
                                        hasFind = ItemHelper.IsKirinElementalSoulItem(attachment.ItemId);
                                    }
                                    break;
                                case ElementalEnum.Ramuh: 
                                    if (itemEnum == ItemEnum.Essence)
                                    {
                                        hasFind = ItemHelper.IsRamuhElementalEssenceItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Shard)
                                    {
                                        hasFind = ItemHelper.IsRamuhElementalShardItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Soul)
                                    {
                                        hasFind = ItemHelper.IsRamuhElementalSoulItem(attachment.ItemId);
                                    }
                                    break;
                                case ElementalEnum.Phoenix: 
                                    if (itemEnum == ItemEnum.Essence)
                                    {
                                        hasFind = ItemHelper.IsPhoenixElementalEssenceItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Shard)
                                    {
                                        hasFind = ItemHelper.IsPhoenixElementalShardItem(attachment.ItemId);
                                    } else if (itemEnum == ItemEnum.Soul)
                                    {
                                        hasFind = ItemHelper.IsPhoenixElementalSoulItem(attachment.ItemId);
                                    }
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