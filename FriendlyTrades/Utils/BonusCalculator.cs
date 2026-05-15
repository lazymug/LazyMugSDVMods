using System;
using StardewValley;

namespace FriendlyTrades.Utils
{
    public static class BonusCalculator
    {
        /// <summary>
        /// Calculates the friendship-based bonus for a given NPC.
        /// Hearts 1-4: +2.5% each (max 10%)
        /// Hearts 5-6: +5% each (max 20%)
        /// Hearts 7-8: +7.5% each (max 35%)
        /// </summary>
        public static float GetBonus(string npcName)
        {
            if (string.IsNullOrEmpty(npcName) || Game1.player == null)
                return 0f;

            int hearts = Game1.player.getFriendshipHeartLevelForNPC(npcName);
            hearts = Math.Min(hearts, 8);

            float bonus = 0f;
            for (int i = 1; i <= hearts; i++)
            {
                if (i <= 4)
                    bonus += 0.025f;
                else if (i <= 6)
                    bonus += 0.05f;
                else
                    bonus += 0.075f;
            }

            return bonus;
        }

        /// <summary>
        /// Returns the bonus percentage as an integer for display (e.g., 35 for 35%).
        /// </summary>
        public static int GetBonusPercent(string npcName)
        {
            return (int)(GetBonus(npcName) * ModEntry.Config.BonusMultiplier * 100);
        }
    }
}
