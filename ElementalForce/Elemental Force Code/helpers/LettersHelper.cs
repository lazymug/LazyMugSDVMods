using Microsoft.Xna.Framework;
using StardewValley;

namespace ElementalForce.Elemental_Force_Code.helpers
{
    public static class LettersHelper
    {
        public static readonly string GaiaAmphoraShardId = $"{ModEntry.Instance.GetModId()}_GaiaAmphoraShardId";
        public static readonly string GaiaAmphoraSoulId = $"{ModEntry.Instance.GetModId()}_GaiaAmphoraSoulId";
        public static readonly string GaiaWelcomeId = $"{ModEntry.Instance.GetModId()}_GaiaWelcomeId";
        public static readonly string MagnusInvitationId = $"{ModEntry.Instance.GetModId()}_MagnusInvitationId";

        private static readonly string GaiaAmphoraShardContentEntryKey = "letter.gaia.amphora.shard";
        private static readonly string GaiaAmphoraSoulContentEntryKey = "letter.gaia.amphora.soul";
        private static readonly string GaiaWelcomeContentEntryKey = "letter.gaia.welcome";
        private static readonly string MagnusInvitationContentEntryKey = "letter.magnus.invitation";

        public static bool GetMagnusInvitationCondition()
        {
            return IsValid(MagnusInvitationId) 
                   && !Game1.player.mailReceived.Contains(MagnusInvitationId) 
                   && Game1.player.friendshipData.ContainsKey("Wizard") 
                   && Game1.player.friendshipData["Wizard"].Points >= 500;
        }
        
        public static bool GetGaiaAmphoraShardCondition()
        {
            return IsValid(GaiaAmphoraShardId)
                && !Game1.player.mailReceived.Contains(GaiaAmphoraShardId)
                && Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraId())
                && (Game1.player.Items.ContainsId(ItemHelper.GetObjectEssenceCarbuncleId()) ||
                    ToolAttachmentHelper.IsCarbuncleEssenceEquipped())
                && (Game1.player.Items.ContainsId(ItemHelper.GetObjectEssenceIfritId()) ||
                    ToolAttachmentHelper.IsIfritEssenceEquipped())
                && (Game1.player.Items.ContainsId(ItemHelper.GetObjectEssenceKirinId()) ||
                    ToolAttachmentHelper.IsKirinEssenceEquipped())
                && (Game1.player.Items.ContainsId(ItemHelper.GetObjectEssenceLeviathanId()) ||
                    ToolAttachmentHelper.IsLeviathanEssenceEquipped())
                && (Game1.player.Items.ContainsId(ItemHelper.GetObjectEssencePhoenixId()) ||
                    ToolAttachmentHelper.IsPhoenixEssenceEquipped())
                && (Game1.player.Items.ContainsId(ItemHelper.GetObjectEssenceRamuhId()) ||
                    ToolAttachmentHelper.IsRamuhEssenceEquipped())
                && (Game1.player.Items.ContainsId(ItemHelper.GetObjectEssenceShivaId()) ||
                    ToolAttachmentHelper.IsShivaEssenceEquipped())
                && (Game1.player.Items.ContainsId(ItemHelper.GetObjectEssenceTitanId()) ||
                    ToolAttachmentHelper.IsTitanEssenceEquipped());
        }
        
        public static bool GetGaiaAmphoraSoulCondition()
        {
            return IsValid(GaiaAmphoraSoulId)
                   && !Game1.player.mailReceived.Contains(GaiaAmphoraSoulId)
                   && Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraEchoesId())
                   && Game1.player.Items.ContainsId(ItemHelper.GetObjectShardCarbuncleId())
                   && Game1.player.Items.ContainsId(ItemHelper.GetObjectShardIfritId())
                   && Game1.player.Items.ContainsId(ItemHelper.GetObjectShardKirinId())
                   && Game1.player.Items.ContainsId(ItemHelper.GetObjectShardLeviathanId())
                   && Game1.player.Items.ContainsId(ItemHelper.GetObjectShardPhoenixId())
                   && Game1.player.Items.ContainsId(ItemHelper.GetObjectShardRamuhId())
                   && Game1.player.Items.ContainsId(ItemHelper.GetObjectShardShivaId())
                   && Game1.player.Items.ContainsId(ItemHelper.GetObjectShardTitanId());
        }

        public static bool GetWelcomeGaiaCondition()
        {
            return IsValid(GaiaWelcomeId)
                   && !Game1.player.mailReceived.Contains(GaiaWelcomeId)
                   && Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraId());
        }

        public static string GetText(string letterId)
        {
            if (MagnusInvitationId == letterId)
            {
                return GetTextFromContent(MagnusInvitationContentEntryKey);
            }
            if (GaiaWelcomeId == letterId)
            {
                return GetTextFromContent(GaiaWelcomeContentEntryKey);
            }
            if (GaiaAmphoraShardId == letterId)
            {
                return GetTextFromContent(GaiaAmphoraShardContentEntryKey);
            }
            if (GaiaAmphoraSoulId == letterId)
            {
                return GetTextFromContent(GaiaAmphoraSoulContentEntryKey);
            }

            return String.Empty;
        }

        private static bool IsValid(string letterId)
        {
            return letterId.StartsWith(ModEntry.Instance.ModManifest.UniqueID);
        }

        private static string GetTextFromContent(string letterId)
        {
            return ModEntry.Instance.GetTextTranslation(letterId);
        }
    }
}