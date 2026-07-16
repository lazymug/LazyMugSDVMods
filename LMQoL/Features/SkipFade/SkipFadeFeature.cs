using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace LMQoL.Features.SkipFade
{
    /// <summary>
    /// Makes the local screen fade used for door/warp transitions resolve near-instantly.
    /// Global fades (sleeping, cutscenes, festivals, end-of-day) are deliberately left alone,
    /// since their timing carries meaning and skipping them can break events.
    /// </summary>
    public class SkipFadeFeature : IFeature
    {
        public string Id => "SkipFade";

        public void Register(IModHelper helper, IMonitor monitor)
        {
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!ModEntry.Config.SkipFadeEnabled || !Context.IsWorldReady)
                return;

            // Never touch global/event fades — only the plain warp fade.
            if (Game1.globalFade || Game1.eventUp || Game1.farmEvent != null || Game1.isFestival())
                return;

            if (!Game1.fadeToBlack)
                return;

            // Push the fade alpha to its end state so the game's own fade state machine
            // completes on the next tick: fading in -> fully clear, fading out -> fully black
            // (which immediately triggers the pending warp).
            Game1.fadeToBlackAlpha = Game1.fadeIn ? 0f : 1f;
        }
    }
}
