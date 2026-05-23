using HarmonyLib;
using StardewValley;

namespace WeatheredLetters.Patches
{
    /// <summary>
    /// Patches trash can interactions to potentially yield a weathered letter.
    /// Uses a postfix on GameLocation.CheckGarbage to add the letter item.
    /// </summary>
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.CheckGarbage))]
    public static class TrashCanPatcher
    {
        public static void Postfix(GameLocation __instance, ref bool __result)
        {
            if (!ModEntry.Config.EnableMod)
                return;

            var tracker = ModEntry.Tracker;
            if (tracker == null || tracker.PendingLetterId == null)
                return;

            var letter = Data.LetterRegistry.AllLetters.Find(l => l.Id == tracker.PendingLetterId);
            if (letter == null || letter.LocationType != Models.SpawnLocationType.Trash)
                return;

            // 50% chance per trash can check when a trash letter is pending
            if (Game1.random.NextDouble() > 0.5)
                return;

            var noteItem = ItemRegistry.Create(Data.LetterSpawner.LetterItemId, 1);
            Game1.player.addItemByMenuIfNecessary(noteItem);

            ModEntry.Instance.OnLetterCollected();
            __result = true;
        }
    }
}
