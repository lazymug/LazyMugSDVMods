using StardewValley;

namespace ElementalForce.Elemental_Force_Code.helpers
{
    public static class LettersHelper
    {
        public static readonly string MagnusInvitationId = $"{ModEntry.Instance.ModManifest.UniqueID}_MagnusInvitationId";
        public static readonly string GaiaWelcomeId = $"{ModEntry.Instance.ModManifest.UniqueID}_GaiaWelcomeId";

        private static readonly string MagnusInvitationContentEntryKey = "letter.magnus.invitation";
        private static readonly string GaiaWelcomeContentEntryKey = "letter.gaia.welcome";

        public static bool GetMagnusInvitationCondition()
        {
            return IsValid(MagnusInvitationId) 
                   && !Game1.player.mailReceived.Contains(MagnusInvitationId) 
                   && Game1.player.friendshipData.ContainsKey("Wizard") 
                   && Game1.player.friendshipData["Wizard"].Points >= 500;
        }

        public static bool GetWelcomeGaiaCondition()
        {
            return IsValid(GaiaWelcomeId)
                   && !Game1.player.mailReceived.Contains(GaiaWelcomeId)
                   && Game1.player.Items.ContainsId(ItemHelper.GetToolCrucibleId());
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
            return String.Empty;
        }

        private static bool IsValid(string letterId)
        {
            return letterId.StartsWith(ModEntry.Instance.ModManifest.UniqueID);
        }

        private static string GetTextFromContent(string letterId)
        {
            return ModEntry.Instance.Helper.Translation.Get(letterId);
        }
    }
}