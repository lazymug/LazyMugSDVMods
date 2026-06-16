using StardewValley;

namespace ElementalForce.Elemental_Force_Code.helpers
{
    public static class ToolAttachmentHelper
    {
        public static bool IsIfritEssenceEquipped() => IsModItemEquipped(ElementalEnum.Ifrit, ItemEnum.Essence);
        public static bool IsIfritShardEquipped() => IsModItemEquipped(ElementalEnum.Ifrit, ItemEnum.Shard);
        public static bool IsIfritSoulEquipped() => IsModItemEquipped(ElementalEnum.Ifrit, ItemEnum.Soul);

        public static bool IsShivaEssenceEquipped() => IsModItemEquipped(ElementalEnum.Shiva, ItemEnum.Essence);
        public static bool IsShivaShardEquipped() => IsModItemEquipped(ElementalEnum.Shiva, ItemEnum.Shard);
        public static bool IsShivaSoulEquipped() => IsModItemEquipped(ElementalEnum.Shiva, ItemEnum.Soul);

        public static bool IsTitanEssenceEquipped() => IsModItemEquipped(ElementalEnum.Titan, ItemEnum.Essence);
        public static bool IsTitanShardEquipped() => IsModItemEquipped(ElementalEnum.Titan, ItemEnum.Shard);
        public static bool IsTitanSoulEquipped() => IsModItemEquipped(ElementalEnum.Titan, ItemEnum.Soul);

        public static bool IsLeviathanEssenceEquipped() => IsModItemEquipped(ElementalEnum.Leviathan, ItemEnum.Essence);
        public static bool IsLeviathanShardEquipped() => IsModItemEquipped(ElementalEnum.Leviathan, ItemEnum.Shard);
        public static bool IsLeviathanSoulEquipped() => IsModItemEquipped(ElementalEnum.Leviathan, ItemEnum.Soul);

        public static bool IsCarbuncleEssenceEquipped() => IsModItemEquipped(ElementalEnum.Carbuncle, ItemEnum.Essence);
        public static bool IsCarbuncleShardEquipped() => IsModItemEquipped(ElementalEnum.Carbuncle, ItemEnum.Shard);
        public static bool IsCarbuncleSoulEquipped() => IsModItemEquipped(ElementalEnum.Carbuncle, ItemEnum.Soul);

        public static bool IsKirinEssenceEquipped() => IsModItemEquipped(ElementalEnum.Kirin, ItemEnum.Essence);
        public static bool IsKirinShardEquipped() => IsModItemEquipped(ElementalEnum.Kirin, ItemEnum.Shard);
        public static bool IsKirinSoulEquipped() => IsModItemEquipped(ElementalEnum.Kirin, ItemEnum.Soul);

        public static bool IsRamuhEssenceEquipped() => IsModItemEquipped(ElementalEnum.Ramuh, ItemEnum.Essence);
        public static bool IsRamuhShardEquipped() => IsModItemEquipped(ElementalEnum.Ramuh, ItemEnum.Shard);
        public static bool IsRamuhSoulEquipped() => IsModItemEquipped(ElementalEnum.Ramuh, ItemEnum.Soul);

        public static bool IsPhoenixEssenceEquipped() => IsModItemEquipped(ElementalEnum.Phoenix, ItemEnum.Essence);
        public static bool IsPhoenixShardEquipped() => IsModItemEquipped(ElementalEnum.Phoenix, ItemEnum.Shard);
        public static bool IsPhoenixSoulEquipped() => IsModItemEquipped(ElementalEnum.Phoenix, ItemEnum.Soul);

        public static bool IsCactuarEssenceEquipped() => IsModItemEquipped(ElementalEnum.Cactuar, ItemEnum.Essence);
        public static bool IsCactuarShardEquipped() => IsModItemEquipped(ElementalEnum.Cactuar, ItemEnum.Shard);
        public static bool IsCactuarSoulEquipped() => IsModItemEquipped(ElementalEnum.Cactuar, ItemEnum.Soul);

        private static bool IsModItemEquipped(ElementalEnum type, ItemEnum itemEnum)
        {
            if (Game1.player?.Items == null)
                return false;

            foreach (var item in Game1.player.Items)
            {
                if (item is not Tool tool || !ItemHelper.IsAnyAmphoraTool(tool.ItemId))
                    continue;

                if (tool.attachments == null)
                    return false;

                for (var index = 0; index < tool.attachments.Length; index++)
                {
                    var attachment = tool.attachments[index];
                    if (attachment != null && ItemHelper.IsElementalItem(attachment.ItemId, type, itemEnum))
                        return true;
                }
                return false;
            }

            return false;
        }
    }
}
