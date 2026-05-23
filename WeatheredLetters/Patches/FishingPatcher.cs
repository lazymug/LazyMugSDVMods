using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

namespace WeatheredLetters.Patches
{
    /// <summary>
    /// Patches FishingRod.openTreasureMenuEndFunction to inject a weathered letter into treasure chests.
    /// </summary>
    [HarmonyPatch(typeof(FishingRod), nameof(FishingRod.openTreasureMenuEndFunction))]
    public static class FishingPatcher
    {
        public static void Postfix(FishingRod __instance)
        {
            if (!ModEntry.Config.EnableMod)
                return;

            var tracker = ModEntry.Tracker;
            if (tracker == null || tracker.PendingLetterId == null)
                return;

            var letter = Data.LetterRegistry.AllLetters.Find(l => l.Id == tracker.PendingLetterId);
            if (letter == null || letter.LocationType != Models.SpawnLocationType.Fishing)
                return;

            // 40% chance per treasure chest
            if (Game1.random.NextDouble() > 0.4)
                return;

            var noteItem = ItemRegistry.Create(Data.LetterSpawner.LetterItemId, 1);
            Game1.player.addItemByMenuIfNecessary(noteItem);

            ModEntry.Instance.OnLetterCollected();
        }
    }
}
