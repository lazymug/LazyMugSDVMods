using Microsoft.Xna.Framework;
using StardewValley;

namespace ElementalForce.Elemental_Force_Code.helpers
{
    public static class LettersHelper
    {
        public static readonly string DwarfGiftId = $"{ModEntry.Instance.GetModId()}_DwarfId";
        public static readonly string EmilyAquamarineId = $"{ModEntry.Instance.GetModId()}.EmilyAquamarineId"; // shiva
        public static readonly string GaiaWelcomeId = $"{ModEntry.Instance.GetModId()}_GaiaWelcomeId";
        public static readonly string MagnusInvitationId = $"{ModEntry.Instance.GetModId()}_MagnusInvitationId";
        public static readonly string PennyFoundId = $"{ModEntry.Instance.GetModId()}_PennyFoundId"; // carbuncle
        public static readonly string SandyQiRoomId = $"{ModEntry.Instance.GetModId()}_SandyQiRoomId"; // ifrit sandy when player opened Qi Room She sends a letter telling that the man who was blocking the way had an Ifrit Essence with him and he forgot
        public static readonly string WillyFishingId = $"{ModEntry.Instance.GetModId()}_WillyFishingId"; // leviathan | fish achievements completed

        private static readonly string DwarfGiftContentEntryKey = "letter.dwarf.gift";
        private static readonly string EmilyAquamarineContentEntryKey = "letter.emily.aquamarine";
        private static readonly string GaiaWelcomeContentEntryKey = "letter.gaia.welcome";
        private static readonly string MagnusInvitationContentEntryKey = "letter.magnus.invitation";
        private static readonly string PennyFoundContentEntryKey = "letter.penny.found";
        private static readonly string SandyQiRoomContentEntryKey = "letter.sandy.qi.room";
        private static readonly string WillyFishingContentEntryKey = "letter.willy.fishing";

        public static bool GetDwarfGiftCondition()
        {
            return IsValid(DwarfGiftId)
                   && Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraId())
                   && Game1.player.canUnderstandDwarves;
        }
        
        public static bool GetEmilyAquamarineCondition()
        {
            return IsValid(EmilyAquamarineId)
                && !Game1.player.mailReceived.Contains(EmilyAquamarineId)
                && Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraId())
                // && Game1.player.eventsSeen.Contains() // check for the event which Emily allows the player to use the sew machine
                && Game1.player.friendshipData.ContainsKey("Emily")
                && Game1.player.friendshipData["Emily"].Points >= 1500;
        }

        public static bool GetMagnusInvitationCondition()
        {
            return IsValid(MagnusInvitationId) 
                   && !Game1.player.mailReceived.Contains(MagnusInvitationId) 
                   && Game1.player.friendshipData.ContainsKey("Wizard") 
                   && Game1.player.friendshipData["Wizard"].Points >= 500;
        }
        
        public static bool GetPennyFoundCondition()
        {
            return IsValid(PennyFoundId)
                && !Game1.player.mailReceived.Contains(PennyFoundId)
                && Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraId())
                && Game1.player.friendshipData.ContainsKey("Penny")
                && Game1.player.friendshipData["Penny"].Points >= 1500;
        }
        
        public static bool GetSandyQiRoomCondition()
        {
            return IsValid(SandyQiRoomId)
                && !Game1.player.mailReceived.Contains(SandyQiRoomId)
                && Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraId())
                && Game1.player.hasClubCard
                && Game1.player.friendshipData.ContainsKey("Sandy")
                && Game1.player.friendshipData["Sandy"].Points >= 500;
        }

        public static bool GetWelcomeGaiaCondition()
        {
            return IsValid(GaiaWelcomeId)
                   && !Game1.player.mailReceived.Contains(GaiaWelcomeId)
                   && Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraId());
        }

        public static bool GetWillyFishingCondition()
        {
            return IsValid(WillyFishingId)
                && !Game1.player.mailReceived.Contains(WillyFishingId)
                && Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraId())
                && Game1.player.friendshipData.ContainsKey("Willy")
                && Game1.player.friendshipData["Willy"].Points >= 1500; // add fishes types
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

            if (DwarfGiftId == letterId)
            {
                return GetTextFromContent(DwarfGiftContentEntryKey);
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